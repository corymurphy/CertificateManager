using CertificateManager.Entities;
using CertificateManager.Repository;
using CertificateServices.ActiveDirectory;
using System;
using System.Security.Claims;

namespace CertificateManager.Logic
{
    public class IdentityAuthenticationLogic
    {
        private const string nameClaim = "http://certificatemanager/upn";
        private const string roleClaim = "http://certificatemanager/role";
        private const string altNameClaim = "http://certificatemanager/alternative-upn";
        private const string localAuthenticationScheme = "Local";
        private const string devAuthBypass = "DevelopmentAuthority";
        private Guid localIdentityProviderId = new Guid("02abeb4c-e0b6-4231-b836-268aa40c3f1c");

        IConfigurationRepository configurationRepository;
        IActiveDirectoryAuthenticator activeDirectoryAuthenticator;

        public IdentityAuthenticationLogic(IConfigurationRepository configurationRepository, IActiveDirectoryAuthenticator activeDirectoryAuthenticator)
        {
            this.configurationRepository = configurationRepository;
            this.activeDirectoryAuthenticator = activeDirectoryAuthenticator;
        }

        private ClaimsPrincipal ConstructClaimsPrincipal(AuthenticablePrincipal authenticablePrincipal, string authScheme)
        {
            ClaimsIdentity id = new ClaimsIdentity(authScheme, nameClaim, roleClaim);

            id.AddClaim(new Claim(nameClaim, authenticablePrincipal.UserPrincipalName));

            if(authenticablePrincipal.AlternativeUserPrincipalNames != null)
            {
                foreach (string altUpn in authenticablePrincipal.AlternativeUserPrincipalNames)
                {
                    id.AddClaim(new Claim(altNameClaim, altUpn));
                }
            }

            var roles = configurationRepository.GetAuthenticablePrincipalMemberOf(authenticablePrincipal.Id);

            if (roles != null)
            {
                foreach (SecurityRole role in configurationRepository.GetAuthenticablePrincipalMemberOf(authenticablePrincipal.Id))
                {
                    id.AddClaim(new Claim(roleClaim, role.Id.ToString()));
                }
            }

            

            ClaimsPrincipal principal = new ClaimsPrincipal(id);

            return principal;
        }

        public ClaimsPrincipal Authenticate(LoginLocalViewModel model)
        {
            if (model.Domain == localIdentityProviderId)
                return this.AuthenticateLocal(model.UserPrincipalName, model.Password);
            else
                return this.AuthenticateActiveDirectory(model.UserPrincipalName, model.Domain, model.Password);
        }


        public ClaimsPrincipal AuthenticateLocal(string upn, string password)
        {
            AuthenticablePrincipal authenticablePrincipal = configurationRepository.GetAuthenticablePrincipal(upn);

            if (authenticablePrincipal == null)
                throw new Exception("User does not exist");


            if (!BCrypt.Net.BCrypt.Verify(password, authenticablePrincipal.PasswordHash))
                throw new Exception("Authentication failed");

            return ConstructClaimsPrincipal(authenticablePrincipal, localAuthenticationScheme);
        }

        public ClaimsPrincipal AuthenticateActiveDirectory(string upn, Guid domain, string password)
        {
            AuthenticablePrincipal authenticablePrincipal = configurationRepository.GetAuthenticablePrincipal(upn);

            ExternalIdentitySource externalIdentitySource = configurationRepository.GetExternalIdentitySource(domain);

            if (externalIdentitySource == null || externalIdentitySource.Enabled != true)
                throw new Exception("Authentication failed");


            if(activeDirectoryAuthenticator.Authenticate(upn, password, externalIdentitySource.Domain))
                return ConstructClaimsPrincipal(authenticablePrincipal, externalIdentitySource.Name);
            else
                throw new Exception("Authentication failed");
        }


        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public ClaimsPrincipal Authenticate(string upn)
        {
            AuthenticablePrincipal authenticablePrincipal = configurationRepository.GetAuthenticablePrincipal(upn);

            return ConstructClaimsPrincipal(authenticablePrincipal, devAuthBypass);
        }

    }
}
