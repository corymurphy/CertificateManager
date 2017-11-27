using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using CertificateManager.Repository;
using System.Security.Claims;
using System.Linq;

namespace CertificateManager.Logic
{
    public class AuthorizationLogic
    {
        private IConfigurationRepository configurationRepository;

        public AuthorizationLogic(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        public bool AuthorizedIssueCertificate()
        {
            return false;
        }

        public bool CanViewPrivateKey(Certificate certificate, ClaimsPrincipal user)
        {
            if (certificate == null || certificate.Acl == null)
                return false;

            if (user == null)
                return false;

            if (!certificate.Acl.Any())
                return false;
            

            bool isAuthorized = false;

            var roles = user.Claims.Where(claim => claim.Type == IdentityAuthenticationLogic.RoleClaimIdentifier);

            var upn = user.Claims.Where(claim => claim.Type == IdentityAuthenticationLogic.UpnClaimIdentifier).FirstOrDefault();

            var uid = user.Claims.Where(claim => claim.Type == IdentityAuthenticationLogic.UidClaimIdentifier).FirstOrDefault();

            foreach (AccessControlEntry ace in certificate.Acl)
            {

                if(ace.IdentityType == IdentityType.Role)
                {
                    foreach(var role in roles)
                    {
                        if( role.Value == ace.Identity & ace.AceType == AceType.Deny )
                        {
                            return false;
                        }
                        else if(role.Value == ace.Identity & ace.AceType == AceType.Allow)
                        {
                            isAuthorized = true;
                        }
                    }
                }



                if(ace.IdentityType == IdentityType.User)
                {
                    if(ace.Identity == uid.Value & ace.AceType == AceType.Deny)
                    {
                        return false;
                    }
                    else if (ace.Identity == uid.Value & ace.AceType == AceType.Deny)
                    {
                        isAuthorized = true;
                    }
                }

            }
            


            return isAuthorized;
        }
        
    }
}
