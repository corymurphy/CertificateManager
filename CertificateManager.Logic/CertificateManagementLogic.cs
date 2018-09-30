using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Entities.Interfaces;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager.Logic
{
    public class CertificateManagementLogic
    {
        IConfigurationRepository configurationRepository;
        ICertificateRepository certificateRepository;
        IAuthorizationLogic authorizationLogic;
        SecurityPrincipalLogic securityPrincipalLogic;
        EncryptionProvider cipher;
        SecretKeyProvider keygen;
        IHashProvider hashProvider;
        IAuditLogic audit;

        public CertificateManagementLogic(
            IConfigurationRepository configurationRepository, 
            ICertificateRepository certificateRepository, 
            IAuthorizationLogic authorizationLogic,
            IAuditLogic audit,
            SecurityPrincipalLogic securityPrincipalLogic,
            EncryptionProvider cipher
        )
        {
            this.securityPrincipalLogic = securityPrincipalLogic;
            this.configurationRepository = configurationRepository;
            this.certificateRepository = certificateRepository;
            this.authorizationLogic = authorizationLogic;
            this.cipher = cipher;
            this.hashProvider = new HashProvider();
            this.keygen = new SecretKeyProvider();
            this.audit = audit;
        }

        public CertificateManagementLogic(
            IConfigurationRepository configurationRepository,
            ICertificateRepository certificateRepository,
            IAuthorizationLogic authorizationLogic,
            IAuditLogic audit,
            SecurityPrincipalLogic securityPrincipalLogic,
            EncryptionProvider cipher,
            IHashProvider hashProvider
        )
        {
            this.securityPrincipalLogic = securityPrincipalLogic;
            this.configurationRepository = configurationRepository;
            this.certificateRepository = certificateRepository;
            this.authorizationLogic = authorizationLogic;
            this.cipher = cipher;
            this.hashProvider = hashProvider;
            this.keygen = new SecretKeyProvider();
            this.audit = audit;
        }

        public GetCertificateEntity GetCertificate(Guid id, ClaimsPrincipal user)
        {
            GetCertificateEntity cert = certificateRepository.Get<GetCertificateEntity>(id);

            audit.LogSecurityAuditSuccess(user, cert, EventCategory.CertificateViewed);

            if(cert.Acl != null && cert.Acl.Count >= 0)
            {
                List<AccessControlEntry> acl = new List<AccessControlEntry>();

                foreach(AccessControlEntry ace in cert.Acl)
                {
                    AccessControlEntry newAce = new AccessControlEntry(ace, securityPrincipalLogic.ResolveSecurityPrincipalDisplayName(ace.Identity));
                    acl.Add(newAce);
                }

                cert.Acl = acl;  
            }

            return cert;
        }

        public T GetCertificate<T>(Guid id) where T: ILoggableEntity
        {
            T cert = certificateRepository.Get<T>(id);

            ClaimsPrincipal system = LocalIdentityProviderLogic.GetSystemIdentity();

            audit.LogSecurityAuditSuccess(system, cert, EventCategory.CertificateViewed);

            return cert;
        }

        public AccessControlEntry AddCertificateAce(Guid certId, AddCertificateAceEntity entity, ClaimsPrincipal user)
        {
            Certificate cert = certificateRepository.Get<Certificate>(certId);

            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.CertificateFullControl, user, cert);

            if (cert.Acl == null)
            {
                cert.Acl = new List<AccessControlEntry>();
            }

            AccessControlEntry ace = new AccessControlEntry(entity);
            
            cert.Acl.Add(ace);

            certificateRepository.Update<Certificate>(cert);

            ace.IdentityDisplayName = securityPrincipalLogic.ResolveSecurityPrincipalDisplayName(ace.Identity);
            ace.Identity = string.Empty;
            return ace;
        }

        public void DeleteCertificateAce(Guid certId, Guid aceId, ClaimsPrincipal user)
        { 
            Certificate cert = certificateRepository.Get<Certificate>(certId);

            authorizationLogic.IsAuthorizedThrowsException(AuthorizationScopes.CertificateFullControl, user, cert);

            if (cert.Acl == null)
            {
                return;
            }

            cert.Acl = cert.Acl.Where(ace => ace.Id != aceId).ToList();

            certificateRepository.Update<Certificate>(cert);
        }

        public GetCertificatePasswordResponseEntity GetCertificatePassword(Guid id, ClaimsPrincipal user)
        {
            GetCertificatePasswordEntity cert = certificateRepository.Get<GetCertificatePasswordEntity>(id);

            if (authorizationLogic.CanViewPrivateKey(cert, user))
            {
                audit.LogSecurityAuditSuccess(user, cert, EventCategory.CertificatePasswordViewed);

                GetCertificatePasswordResponseEntity response = new GetCertificatePasswordResponseEntity();

                response.DecryptedPassword = cipher.Decrypt(cert.PfxPassword, cert.PasswordNonce);

                return response;
            }
            else
            {
                audit.LogSecurityAuditFailure(user, cert, EventCategory.CertificatePasswordViewed);
                throw new UnauthorizedAccessException();
            }
        }

        public void ResetCertificatePassword(Guid id, ClaimsPrincipal user)
        {
            Certificate cert = certificateRepository.Get<Certificate>(id);

            if (!authorizationLogic.CanViewPrivateKey(cert, user))
            {
                audit.LogSecurityAuditFailure(user, cert, EventCategory.CertificatePasswordReset);
                throw new UnauthorizedAccessException();
            }

            if(!hashProvider.ValidateData(cert.Content, cert.ContentDigest))
            {
                throw new InvalidOperationException("Certificate data is corrupt");
            }

            if (!cert.HasPrivateKey || cert.CertificateStorageFormat != CertificateStorageFormat.Pfx)
            {
                throw new ObjectNotInCorrectStateException("Certificate does not have a private key");
            }

            audit.LogSecurityAuditSuccess(user, cert, EventCategory.CertificatePasswordReset);

            byte[] certBytes = cert.Content;

            X509Certificate2 x509 = new X509Certificate2();
            x509.Import(certBytes, cipher.Decrypt(cert.PfxPassword, cert.PasswordNonce), X509KeyStorageFlags.Exportable);

            string nonce = keygen.NewSecretBase64(16);
            string password = keygen.NewSecret(64);

            byte[] newBytes = x509.Export(X509ContentType.Pfx, password);

            cert.ContentDigest = hashProvider.ComputeHash(newBytes);
            cert.Content = newBytes;
            cert.PfxPassword = cipher.Encrypt(password, nonce);
            cert.PasswordNonce = nonce;

            certificateRepository.Update<Certificate>(cert);
        }

        public IEnumerable<AllCertificatesViewModel> GetAllCertificates()
        {

            return certificateRepository.GetAll<AllCertificatesViewModel>();
        }

        public void InitializeMockData()
        {
            for (int i = -10; i <= 0; i++)
            {
                DateTime day = DateTime.Now;
                day = day.AddDays(i);

                int certCount = new Random().Next(5, 30);

                int index = 0;
                while (index < certCount)
                {
                    Certificate newCert = new Certificate()
                    {
                        IssuedOn = day,
                        DisplayName = "fakecert"
                    };

                    certificateRepository.Insert<Certificate>(newCert);

                    index++;
                }

            }
        }

        public DownloadPfxCertificateEntity GetPfxCertificateContent(Guid id)
        {
            DownloadPfxCertificateEntity cert = certificateRepository.Get<DownloadPfxCertificateEntity>(id);


            if (!cert.HasPrivateKey || cert.CertificateStorageFormat != CertificateStorageFormat.Pfx)
            {
                throw new Exception("No private key");
            }

            return cert;
        }

    }
}
