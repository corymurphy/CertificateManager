using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Entities.Interfaces;
using CertificateManager.Logic.Interfaces;
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
        IAuthorizationLogic authorizationLogic;
        DataTransformation dataTransformation;
        SecretKeyProvider secrets;
        EncryptionProvider cipher;
        HashProvider hashProvider;
        ClaimsPrincipal user;
        AdcsTemplateLogic templateLogic;

        public PrivateCertificateProcessing(ICertificateRepository certificateRepository, IConfigurationRepository configurationRepository, ICertificateProvider certificateProvider, IAuthorizationLogic authorizationLogic, ClaimsPrincipal user, AdcsTemplateLogic templateLogic)
        {
            this.configurationRepository = configurationRepository;
            this.certificateRepository = certificateRepository;
            this.certificateProvider = certificateProvider;
            this.authorizationLogic = authorizationLogic;
            this.templateLogic = templateLogic;
            this.dataTransformation = new DataTransformation();
            this.hashProvider = new HashProvider();
            this.secrets = new SecretKeyProvider();
            this.cipher = new EncryptionProvider(configurationRepository.GetAppConfig().EncryptionKey);
            this.user = user;
        }

        public SignPrivateCertificateResult SignCertificate(SignPrivateCertificateModel model)
        {
            CertificateRequest csr = certificateProvider.InitializeFromEncodedCsr(model.EncodedCsr);

            AdcsTemplate template = configurationRepository.GetAdcsTemplate(model.HashAlgorithm, csr.CipherAlgorithm, WindowsApi.Cng, KeyUsage.ServerAuthentication);

            if (authorizationLogic.IsAuthorized(template, user))
            {
                MicrosoftCertificateAuthority ca = configurationRepository.GetPrivateCertificateAuthority(model.HashAlgorithm);

                CertificateAuthorityRequestResponse response = ca.Sign(csr, template.Name, template.KeyUsage);

                return HandleCertificateAuthorityResponse(model, response, csr.Subject);
            }
            else
            {
                return ProcessPendingSigningWorkflow(model);
            }
        }

        public CreatePrivateCertificateResult CreateCertificateWithPrivateKey(CreatePrivateCertificateModel model)
        {
            model.RequestDate = DateTime.Now;

            KeyUsage keyUsage = dataTransformation.ParseKeyUsage(model.KeyUsage);

            AdcsTemplate template = configurationRepository.GetAdcsTemplate(model.HashAlgorithm, model.CipherAlgorithm, model.Provider, keyUsage);

            if(authorizationLogic.IsAuthorized(template, user))
            {
                CertificateRequest csr = certificateProvider.CreateCsrKeyPair(dataTransformation.NewCertificateSubjectFromModel(model), model.CipherAlgorithm, model.KeySize, model.Provider, SigningRequestProtocol.Pkcs10);

                MicrosoftCertificateAuthority ca = configurationRepository.GetPrivateCertificateAuthority(model.HashAlgorithm);

                CertificateAuthorityRequestResponse response = ca.Sign(csr, template.Name, template.KeyUsage);

                CreatePrivateCertificateResult result = ProcessCertificateAuthorityResponse(model, response, csr.Subject);

                return result;
            }
            else
            {
                return ProcessNewPendingCertificateWorkflow(model);
            }
  
        }

        public CreatePrivateCertificateResult IssuePendingCertificate(Guid id)
        {
            PendingCertificate pendingCertificate = certificateRepository.Get<PendingCertificate>(id);

            KeyUsage keyUsage = dataTransformation.ParseKeyUsage(pendingCertificate.KeyUsage);

            AdcsTemplate template = configurationRepository.GetAdcsTemplate(pendingCertificate.HashAlgorithm, pendingCertificate.CipherAlgorithm, pendingCertificate.Provider, keyUsage);

            if(authorizationLogic.IsAuthorized(template, user))
            {
                CertificateRequest csr = certificateProvider.CreateCsrKeyPair(dataTransformation.NewCertificateSubjectFromModel(pendingCertificate), pendingCertificate.CipherAlgorithm, pendingCertificate.KeySize, pendingCertificate.Provider, SigningRequestProtocol.Pkcs10);

                MicrosoftCertificateAuthority ca = configurationRepository.GetPrivateCertificateAuthority(pendingCertificate.HashAlgorithm);

                CertificateAuthorityRequestResponse response = ca.Sign(csr, template.Name, template.KeyUsage);

                CreatePrivateCertificateResult result = ProcessCertificateAuthorityResponse(pendingCertificate, response, csr.Subject);

                certificateRepository.Delete<PendingCertificate>(id);

                return result;
            }
            else
            {
                throw new UnauthorizedAccessException("Current user is not authorized to issue pending certificates");
            }
        }




        private CreatePrivateCertificateResult ProcessNewPendingCertificateWorkflow(CreatePrivateCertificateModel model)
        {
            CreatePrivateCertificateResult result = new CreatePrivateCertificateResult(PrivateCertificateRequestStatus.Pending, Guid.NewGuid());

            PendingCertificate pendingCertificate = new PendingCertificate(model);

            certificateRepository.Insert<PendingCertificate>(pendingCertificate);

            return result;
        }

        private CreatePrivateCertificateResult ProcessCertificateAuthoritySuccessResponse(ICertificateRequestPublicPrivateKeyPair model, CertificateAuthorityRequestResponse response, CertificateSubject subject)
        {
            string nonce = secrets.NewSecretBase64(16);
            string password = secrets.NewSecret(64);
            X509Certificate2 cert = certificateProvider.InstallIssuedCertificate(response.IssuedCertificate);

            byte[] certContent = cert.Export(X509ContentType.Pfx, password);

            CreatePrivateCertificateResult result = new CreatePrivateCertificateResult()
            {
                Password = password,
                Pfx = Convert.ToBase64String(certContent),
                Status = PrivateCertificateRequestStatus.Success,
                Thumbprint = cert.Thumbprint,
                Id = Guid.NewGuid(),

            };

            List<AccessControlEntry> defaultAcl = authorizationLogic.GetDefaultCertificateAcl(user);

            Certificate storedCert = new Certificate()
            {
                Id = result.Id,
                Thumbprint = cert.Thumbprint,
                PfxPassword = cipher.Encrypt(password, nonce),
                WindowsApi = model.Provider,
                Content = certContent,
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
                Acl = defaultAcl,
                PasswordNonce = nonce,
                ContentDigest = hashProvider.ComputeHash(certContent)

            };

            certificateRepository.Insert(storedCert);

            return result;
        }

        private CreatePrivateCertificateResult ProcessCertificateAuthorityResponse(ICertificateRequestPublicPrivateKeyPair model, CertificateAuthorityRequestResponse response, CertificateSubject subject)
        {
            CreatePrivateCertificateResult result;

            switch (response.CertificateRequestStatus)
            {
                case CertificateRequestStatus.Issued:
                    result = ProcessCertificateAuthoritySuccessResponse(model, response, subject);
                    break;
                case CertificateRequestStatus.Pending:
                    throw new CertificateAuthorityDeniedRequestException(string.Format("Error while processing request id {0}", response.RequestId));
                    break;
                default:
                    throw new CertificateAuthorityDeniedRequestException(string.Format("Error while processing request id {0}", response.RequestId));
                    break;
            }

            return result;
        }

        





        private SignPrivateCertificateResult HandleCertificateAuthorityResponse(SignPrivateCertificateModel model, CertificateAuthorityRequestResponse response, CertificateSubject subject)
        {
            SignPrivateCertificateResult result;

            switch (response.CertificateRequestStatus)
            {
                case CertificateRequestStatus.Issued:
                    result = ProcessCertificateAuthoritySigningResponse(model, response, subject);
                    break;
                case CertificateRequestStatus.Pending:
                    throw new CertificateAuthorityDeniedRequestException(string.Format("Error while processing request id {0}", response.RequestId));
                    break;
                default:
                    throw new CertificateAuthorityDeniedRequestException(string.Format("Error while processing request id {0}", response.RequestId));
                    break;
            }

            return result;
        }

        private SignPrivateCertificateResult ProcessCertificateAuthoritySigningResponse(SignPrivateCertificateModel model, CertificateAuthorityRequestResponse response, CertificateSubject subject)
        {
            return null;
        }

        private SignPrivateCertificateResult ProcessPendingSigningWorkflow(SignPrivateCertificateModel model)
        {
            SignPrivateCertificateResult result = new SignPrivateCertificateResult(PrivateCertificateRequestStatus.Pending);

            PendingCertificate pendingCertificate = new PendingCertificate(model);

            certificateRepository.Insert<PendingCertificate>(pendingCertificate);

            return result;
        }

    }
}
