using CertificateManager.Entities;
using CertificateManager.Repository;
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

        IConfigurationRepository configurationRepository;

        public IdentityAuthenticationLogic(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        private ClaimsPrincipal ConstructClaimsPrincipal(AuthenticablePrincipal authenticablePrincipal, string authScheme)
        {
            ClaimsIdentity id = new ClaimsIdentity(authScheme, nameClaim, roleClaim);

            id.AddClaim(new Claim(nameClaim, authenticablePrincipal.UserPrincipalName));

            foreach (string altUpn in authenticablePrincipal.AlternativeUserPrincipalNames)
            {
                id.AddClaim(new Claim(altNameClaim, altUpn));
            }

            foreach (SecurityRole role in configurationRepository.GetAuthenticablePrincipalMemberOf(authenticablePrincipal.Id))
            {
                id.AddClaim(new Claim(roleClaim, role.Id.ToString()));
            }

            ClaimsPrincipal principal = new ClaimsPrincipal(id);

            return principal;
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

        public ClaimsPrincipal AuthenticateActiveDirectory(string upn, string domain, string password)
        {
            AuthenticablePrincipal authenticablePrincipal = configurationRepository.GetAuthenticablePrincipal(upn);

            return ConstructClaimsPrincipal(authenticablePrincipal, localAuthenticationScheme);
        }

    }
}
