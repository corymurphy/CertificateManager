using CertificateManager.Entities;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CertificateManager.Logic
{
    public class CertificateManagementLogic
    {
        IConfigurationRepository configurationRepository;
        ICertificateRepository certificateRepository;
        IAuthorizationLogic authorizationLogic;
        SecurityPrincipalLogic securityPrincipalLogic;
        EncryptionProvider cipher;
        SecretKeyProvider keygenProvider;

        public CertificateManagementLogic(
            IConfigurationRepository configurationRepository, 
            ICertificateRepository certificateRepository, 
            IAuthorizationLogic authorizationLogic, 
            SecurityPrincipalLogic securityPrincipalLogic,
            EncryptionProvider cipher
        )
        {
            this.securityPrincipalLogic = securityPrincipalLogic;
            this.configurationRepository = configurationRepository;
            this.certificateRepository = certificateRepository;
            this.authorizationLogic = authorizationLogic;
            this.cipher = cipher;
        }

        public GetCertificateEntity GetCertificate(Guid id)
        {
            GetCertificateEntity cert = certificateRepository.Get<GetCertificateEntity>(id);


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

        public AccessControlEntry AddCertificateAce(Guid certId, AddCertificateAceEntity entity)
        {
            Certificate cert = certificateRepository.Get<Certificate>(certId);

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

        public void DeleteCertificateAce(Guid certId, Guid aceId)
        {
            Certificate cert = certificateRepository.Get<Certificate>(certId);

            if (cert.Acl == null)
            {
                return;
            }

            //bool a = cert.Acl[2].Id == aceId;

            cert.Acl = cert.Acl.Where(ace => ace.Id != aceId).ToList();

            certificateRepository.Update<Certificate>(cert);
        }

        public GetCertificatePasswordResponseEntity GetCertificatePassword(Guid id, ClaimsPrincipal user)
        {
            GetCertificatePasswordEntity cert = certificateRepository.Get<GetCertificatePasswordEntity>(id);

            if (authorizationLogic.CanViewPrivateKey(cert, user))
            {
                GetCertificatePasswordResponseEntity response = new GetCertificatePasswordResponseEntity();

                response.DecryptedPassword = cipher.Decrypt(cert.PfxPassword, cert.PasswordNonce);

                return response;
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }


        public void ResetCertificatePassword(Guid id, ClaimsPrincipal user)
        {
            Certificate cert = certificateRepository.Get<Certificate>(id);

            if (authorizationLogic.CanViewPrivateKey(cert, user))
            {

                //cert.
                GetCertificatePasswordResponseEntity response = new GetCertificatePasswordResponseEntity();

                response.DecryptedPassword = cipher.Decrypt(cert.PfxPassword, cert.PasswordNonce);

                //return response;
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        public IEnumerable<AllCertificatesViewModel> GetAllCertificates()
        {
            return certificateRepository.GetAll<AllCertificatesViewModel>();
        }

    }
}
