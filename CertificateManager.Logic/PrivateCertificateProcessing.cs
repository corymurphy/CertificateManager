﻿using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Entities.Interfaces;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using CertificateServices;
using CertificateServices.Enumerations;
using CertificateServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace CertificateManager.Logic
{
    public class PrivateCertificateProcessing : IPrivateCertificateProcessing
    {
        IAuditLogic audit;
        ICertificateRepository certificateRepository;
        IConfigurationRepository configurationRepository;
        ICertificateProvider certificateProvider;
        IAuthorizationLogic authorizationLogic;
        DataTransformation dataTransformation;
        SecretKeyProvider secrets;
        EncryptionProvider cipher;
        HashProvider hashProvider;
        AdcsTemplateLogic templateLogic;

        string[] searchDirs = new string[3] { @"C:\ProgramData\Microsoft\Crypto\Keys", @"C:\ProgramData\Microsoft\Crypto\RSA\MachineKeys", @"C:\ProgramData\Microsoft\Crypto\RSA\S-1-5-18" };

        public PrivateCertificateProcessing(ICertificateRepository certificateRepository, IConfigurationRepository configurationRepository, ICertificateProvider certificateProvider, IAuthorizationLogic authorizationLogic, AdcsTemplateLogic templateLogic, IAuditLogic audit)
        {
            this.audit = audit;
            this.configurationRepository = configurationRepository;
            this.certificateRepository = certificateRepository;
            this.certificateProvider = certificateProvider;
            this.authorizationLogic = authorizationLogic;
            this.templateLogic = templateLogic;
            this.dataTransformation = new DataTransformation();
            this.hashProvider = new HashProvider();
            this.secrets = new SecretKeyProvider();
            this.cipher = new EncryptionProvider(configurationRepository.GetAppConfig().EncryptionKey);
        }

        private void Audit(CreatePrivateCertificateResult result, ClaimsPrincipal user)
        {

            switch(result.Status)
            {
                case PrivateCertificateRequestStatus.Success:
                    audit.LogOpsSuccess(user, result.Thumbprint, EventCategory.CertificateIssuance, "Certificate was successfully issued");
                    break;
                case PrivateCertificateRequestStatus.Pending:
                    audit.LogOpsSuccess(user, result.Thumbprint, EventCategory.CertificateIssuance, "Certificate is pending issuance");
                    break;
                case PrivateCertificateRequestStatus.Error:
                    audit.LogOpsError(user, string.Empty, EventCategory.CertificateIssuance, string.Format("Failed to issue certificate", result.Message ) );
                    break;
                default:
                    audit.LogOpsError(user, string.Empty, EventCategory.CertificateIssuance, string.Format("Failed to issue certificate", result.Message));
                    break;
            }
        }

        public SignPrivateCertificateResult SignCertificate(SignPrivateCertificateModel model, ClaimsPrincipal user)
        {
            CertificateRequest csr = certificateProvider.InitializeFromEncodedCsr(model.EncodedCsr);

            AdcsTemplate template = templateLogic.DiscoverTemplate(csr.CipherAlgorithm, WindowsApi.Cng, KeyUsage.ServerAuthentication);

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

        public CreatePrivateCertificateResult CreateCertificateWithPrivateKey(CreatePrivateCertificateModel model, ClaimsPrincipal user)
        {
            model.RequestDate = DateTime.Now;

            KeyUsage keyUsage = dataTransformation.ParseKeyUsage(model.KeyUsage);

            AdcsTemplate template = templateLogic.DiscoverTemplate(model.CipherAlgorithm, model.Provider, keyUsage);

            if(!templateLogic.ValidateTemplateWithRequest(model, template))
            {
                throw new AdcsTemplateValidationException("Certificate request does not meet the requirements of the certificate template");
            }

            if (authorizationLogic.IsAuthorized(template, user))
            {
                CertificateRequest csr = certificateProvider.CreateCsrKeyPair(dataTransformation.NewCertificateSubjectFromModel(model), model.CipherAlgorithm, model.KeySize, model.Provider, SigningRequestProtocol.Pkcs10);

                MicrosoftCertificateAuthority ca = configurationRepository.GetPrivateCertificateAuthority(model.HashAlgorithm);

                CertificateAuthorityRequestResponse response = ca.Sign(csr, template.Name, template.KeyUsage);

                CreatePrivateCertificateResult result = ProcessCertificateAuthorityResponse(model, response, csr.Subject, user);

                this.Audit(result, user);
                
                return result;
            }
            else
            {
                return ProcessNewPendingCertificateWorkflow(model);
            }
  
        }

        public CreatePrivateCertificateResult IssuePendingCertificate(Guid id, ClaimsPrincipal user)
        {
            PendingCertificate pendingCertificate = certificateRepository.Get<PendingCertificate>(id);

            KeyUsage keyUsage = dataTransformation.ParseKeyUsage(pendingCertificate.KeyUsage);

            AdcsTemplate template = templateLogic.DiscoverTemplate(pendingCertificate.CipherAlgorithm, pendingCertificate.Provider, keyUsage);

            if (authorizationLogic.IsAuthorized(template, user))
            {
                CertificateRequest csr = certificateProvider.CreateCsrKeyPair(dataTransformation.NewCertificateSubjectFromModel(pendingCertificate), pendingCertificate.CipherAlgorithm, pendingCertificate.KeySize, pendingCertificate.Provider, SigningRequestProtocol.Pkcs10);

                MicrosoftCertificateAuthority ca = configurationRepository.GetPrivateCertificateAuthority(pendingCertificate.HashAlgorithm);

                CertificateAuthorityRequestResponse response = ca.Sign(csr, template.Name, template.KeyUsage);

                CreatePrivateCertificateResult result = ProcessCertificateAuthorityResponse(pendingCertificate, response, csr.Subject, user);

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

        private void SetPrivateKeyFileSystemAccess(ICertificateRequestPublicPrivateKeyPair model, X509Certificate2 cert, ClaimsPrincipal user)
        {

            audit.LogOpsSuccess(user, cert.Thumbprint, EventCategory.CertificateAccess, "SetPrivateKeyFileSystemAccess");

            string uniqueName = string.Empty;

            audit.LogOpsSuccess(user, cert.Thumbprint, EventCategory.CertificateAccess, model.Provider.ToString());

            if (model.Provider == WindowsApi.Cng)
            {

                uniqueName = cert.GetCngKey().UniqueName;

                audit.LogOpsSuccess(user, cert.Thumbprint, EventCategory.CertificateAccess, uniqueName);

                string keyPath = string.Empty;
                //string searchDir = System.IO.Path.Combine(System.Environment.GetEnvironmentVariable("ProgramData"), @"Microsoft\Crypto");
                //System.IO.Directory.GetFiles(searchDir, uniqueName, System.IO.SearchOption.AllDirectories);

                foreach(string dir in searchDirs)
                {
                    string[] keyPaths = Directory.GetFiles(dir, uniqueName, SearchOption.AllDirectories);

                    if(keyPaths.Any())
                    {
                        keyPath = keyPaths.First();
                        continue;
                        
                    }

                    
                }

                audit.LogOpsSuccess(user, cert.Thumbprint, EventCategory.CertificateAccess, keyPath);

                DirectoryInfo directoryInfo = new DirectoryInfo(keyPath);

                DirectorySecurity acl = directoryInfo.GetAccessControl();

                FileSystemAccessRule ace = new FileSystemAccessRule
                    (
                        new NTAccount(WindowsIdentity.GetCurrent().Name),
                        FileSystemRights.FullControl,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None,
                        AccessControlType.Allow
                    );

                acl.AddAccessRule(ace);

                Directory.SetAccessControl(keyPath, acl);

            }

        }

        private CreatePrivateCertificateResult ProcessCertificateAuthoritySuccessResponse(ICertificateRequestPublicPrivateKeyPair model, CertificateAuthorityRequestResponse response, CertificateSubject subject, ClaimsPrincipal user)
        {
            string nonce = secrets.NewSecretBase64(16);
            string password = secrets.NewSecret(64);
            X509Certificate2 cert = certificateProvider.InstallIssuedCertificate(response.IssuedCertificate);
            
            SetPrivateKeyFileSystemAccess(model, cert, user);

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

        private CreatePrivateCertificateResult ProcessCertificateAuthorityResponse(ICertificateRequestPublicPrivateKeyPair model, CertificateAuthorityRequestResponse response, CertificateSubject subject, ClaimsPrincipal user)
        {
            CreatePrivateCertificateResult result;

            switch (response.CertificateRequestStatus)
            {
                case CertificateRequestStatus.Issued:
                    result = ProcessCertificateAuthoritySuccessResponse(model, response, subject, user);
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
