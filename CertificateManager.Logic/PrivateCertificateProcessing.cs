using CertificateManager.Entities;
using CertificateManager.Repository;
using CertificateServices;
using CertificateServices.Enumerations;
using CertificateServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager.Logic
{
    public class PrivateCertificateProcessing
    {
        ICertificateRepository certificateRepository;
        IConfigurationRepository configurationRepository;
        ICertificateProvider certificateProvider;
        DataTransformation dataTransformation;
        SecretKeyProvider secrets;
        ClaimsPrincipal user;

        public PrivateCertificateProcessing(ICertificateRepository certificateRepository, IConfigurationRepository configurationRepository, ICertificateProvider certificateProvider, ClaimsPrincipal user)
        {
            this.configurationRepository = configurationRepository;
            this.certificateRepository = certificateRepository;
            this.certificateProvider = certificateProvider;
            this.dataTransformation = new DataTransformation();
            this.secrets = new SecretKeyProvider();
            this.user = user;
        }

        public SignPrivateCertificateResult SignCertificate(SignPrivateCertificateModel model)
        {
            CertificateRequest csr = certificateProvider.InitializeFromEncodedCsr(model.EncodedCsr);

            AdcsTemplate template = configurationRepository.GetAdcsTemplate(model.HashAlgorithm, csr.CipherAlgorithm, WindowsApi.Cng, KeyUsage.ServerAuthentication);

            MicrosoftCertificateAuthority ca = configurationRepository.GetPrivateCertificateAuthority(model.HashAlgorithm);

            CertificateAuthorityRequestResponse response = ca.Sign(csr, template.Name, template.KeyUsage);

            return HandleCertificateAuthorityResponse(model, response, csr.Subject);
        }

        public CreatePrivateCertificateResult CreateCertificateWithPrivateKey(CreatePrivateCertificateModel model)
        {
            model.RequestDate = DateTime.Now;

            KeyUsage keyUsage = dataTransformation.ParseKeyUsage(model.KeyUsage);

            AdcsTemplate template = configurationRepository.GetAdcsTemplate(model.HashAlgorithm, model.CipherAlgorithm, model.Provider, keyUsage);

            CertificateRequest csr = certificateProvider.CreateCsrKeyPair(NewCertificateSubjectFromModel(model), model.CipherAlgorithm, model.KeySize, model.Provider, SigningRequestProtocol.Pkcs10);

            MicrosoftCertificateAuthority ca = configurationRepository.GetPrivateCertificateAuthority(model.HashAlgorithm);

            CertificateAuthorityRequestResponse response = ca.Sign(csr, template.Name, template.KeyUsage);

            return HandleCertificateAuthorityResponse(model, response, csr.Subject);
        }

        private CreatePrivateCertificateResult HandleCertificateAuthorityResponse(CreatePrivateCertificateModel model, CertificateAuthorityRequestResponse response, CertificateSubject subject)
        {
            CreatePrivateCertificateResult result;

            switch (response.CertificateRequestStatus)
            {
                case CertificateRequestStatus.Issued:
                    result = HandleSuccess(model, response, subject);
                    break;
                case CertificateRequestStatus.Pending:
                    result = HandlePending(model, response);
                    break;
                default:
                    result = HandleError(model, response);
                    break;
            }

            return result;
        }

        private CreatePrivateCertificateResult HandleError(CreatePrivateCertificateModel model, CertificateAuthorityRequestResponse response)
        {
            return new CreatePrivateCertificateResult()
            {
                Status = PrivateCertificateRequestStatus.Error
            };
        }

        private CreatePrivateCertificateResult HandleSuccess(CreatePrivateCertificateModel model, CertificateAuthorityRequestResponse response, CertificateSubject subject)
        {
            string password = secrets.NewSecret(64);
            X509Certificate2 cert = certificateProvider.InstallIssuedCertificate(response.IssuedCertificate);

            CreatePrivateCertificateResult result = new CreatePrivateCertificateResult()
            {
                Password = password,
                Pfx = System.Convert.ToBase64String( cert.Export(X509ContentType.Pfx, password) ),
                Status = PrivateCertificateRequestStatus.Success,
                Thumbprint = cert.Thumbprint,
                Id = Guid.NewGuid()
            };

            List<AccessControlEntry> defaultAcl = new List<AccessControlEntry>();

            defaultAcl.Add(new AccessControlEntry()
            {
                Expires = DateTime.MaxValue,
                AceType = Entities.Enumerations.AceType.Allow,
                Id = new Guid(),
                IdentityType = Entities.Enumerations.IdentityType.Role,
                Identity = user.Identity.Name
            });

            Certificate storedCert = new Certificate()
            {
                Id = result.Id,
                Thumbprint = cert.Thumbprint,
                PfxPassword = password,
                WindowsApi = model.Provider,
                Content = result.Pfx,
                CertificateStorageFormat = CertificateStorageFormat.Pfx,
                HashAlgorithm = model.HashAlgorithm,
                CipherAlgorithm = model.CipherAlgorithm,
                DisplayName = model.SubjectCommonName,
                HasPrivateKey = true,
                ValidTo = cert.NotAfter,
                ValidFrom = cert.NotBefore,
                KeySize = model.KeySize,
                KeyUsage = dataTransformation.ParseKeyUsage(model.KeyUsage),
                Subject = subject,
                Acl = defaultAcl
            };

            certificateRepository.Insert(storedCert);

            return result;
        }

        private CreatePrivateCertificateResult HandlePending(CreatePrivateCertificateModel model, CertificateAuthorityRequestResponse response)
        {
            return null;
        }

        private SignPrivateCertificateResult HandleCertificateAuthorityResponse(SignPrivateCertificateModel model, CertificateAuthorityRequestResponse response, CertificateSubject subject)
        {
            SignPrivateCertificateResult result;

            switch (response.CertificateRequestStatus)
            {
                case CertificateRequestStatus.Issued:
                    result = HandleSuccess(model, response, subject);
                    break;
                case CertificateRequestStatus.Pending:
                    result = HandlePending(model, response);
                    break;
                default:
                    result = HandleError(model, response);
                    break;
            }

            return result;
        }

        private SignPrivateCertificateResult HandleError(SignPrivateCertificateModel model, CertificateAuthorityRequestResponse response)
        {
            return null;
        }

        private SignPrivateCertificateResult HandleSuccess(SignPrivateCertificateModel model, CertificateAuthorityRequestResponse response, CertificateSubject subject)
        {
            return null;
        }

        private SignPrivateCertificateResult HandlePending(SignPrivateCertificateModel model, CertificateAuthorityRequestResponse response)
        {
            return null;
        }

        public CertificateSubject NewCertificateSubjectFromModel(CreatePrivateCertificateModel model)
        {
            List<string> san = dataTransformation.ParseSubjectAlternativeName(model.SubjectAlternativeNamesRaw);

            CertificateSubject subject = new CertificateSubject(model.SubjectCommonName, san);

            if (string.IsNullOrWhiteSpace(model.SubjectCity))
                subject.City = model.SubjectCity;

            if (string.IsNullOrWhiteSpace(model.SubjectCountry))
                subject.Country = model.SubjectCountry;

            if (string.IsNullOrWhiteSpace(model.SubjectDepartment))
                subject.Department = model.SubjectDepartment;

            if (string.IsNullOrWhiteSpace(model.SubjectOrganization))
                subject.Organization = model.SubjectOrganization;

            if (string.IsNullOrWhiteSpace(model.SubjectState))
                subject.State = model.SubjectState;

            return subject;
        }
    }
}
