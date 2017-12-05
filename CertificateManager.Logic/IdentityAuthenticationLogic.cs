using CertificateManager.Entities;
using CertificateManager.Repository;
using CertificateServices.ActiveDirectory;
using System;
using System.Linq;
using System.Security.Claims;

namespace CertificateManager.Logic
{
    public class IdentityAuthenticationLogic
    {
        public static string RoleClaimIdentifier { get { return roleClaim; } }
        public static string UpnClaimIdentifier { get { return nameClaim; } }
        public static string UidClaimIdentifier { get { return uidClaim; } }



        private const string nameClaim = "http://certificatemanager/upn";
        private const string roleClaim = "http://certificatemanager/role";
        private const string altNameClaim = "http://certificatemanager/alternative-upn";
        private const string uidClaim = "http://certificatemanager/uid";

        private const string devAuthBypass = "DevelopmentAuthority";
        private Guid localIdentityProviderId = new Guid("02abeb4c-e0b6-4231-b836-268aa40c3f1c");

        IConfigurationRepository configurationRepository;
        IActiveDirectoryAuthenticator activeDirectoryAuthenticator;
        LocalIdentityProviderLogic localIdentityProviderLogic;

        public IdentityAuthenticationLogic(IConfigurationRepository configurationRepository, IActiveDirectoryAuthenticator activeDirectoryAuthenticator)
        {
            this.localIdentityProviderLogic = new LocalIdentityProviderLogic(configurationRepository);
            this.activeDirectoryAuthenticator = activeDirectoryAuthenticator;
            this.configurationRepository = configurationRepository;
        }

        private ClaimsPrincipal ConstructClaimsPrincipal(AuthenticablePrincipal authenticablePrincipal, string authScheme)
        {
            ClaimsIdentity id = new ClaimsIdentity(authScheme, nameClaim, roleClaim);

            id.AddClaim(new Claim(nameClaim, authenticablePrincipal.Name));
            id.AddClaim(new Claim(uidClaim, authenticablePrincipal.Id.ToString()));

            if(authenticablePrincipal.AlternativeNames != null)
            {
                foreach (string altUpn in authenticablePrincipal.AlternativeNames)
                {
                    id.AddClaim(new Claim(altNameClaim, altUpn));
                }
            }

            var roles = configurationRepository.GetAuthenticablePrincipalMemberOf(authenticablePrincipal.Id);


            if (roles != null || roles.Any() != false)
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
            AuthenticablePrincipal authenticablePrincipal = localIdentityProviderLogic.Authenticate(upn, password);

            this.IncrementSuccessfulAuthentication(authenticablePrincipal, localIdentityProviderId);

            return ConstructClaimsPrincipal(authenticablePrincipal, localIdentityProviderLogic.GetLocalIdpIdentifier());
        }

        public ClaimsPrincipal AuthenticateActiveDirectory(string upn, Guid domain, string password)
        {
            AuthenticablePrincipal authenticablePrincipal = configurationRepository.GetAuthenticablePrincipal(upn);

            ExternalIdentitySource externalIdentitySource = configurationRepository.GetExternalIdentitySource(domain);

            if (externalIdentitySource == null || externalIdentitySource.Enabled != true)
                throw new Exception("Authentication failed");


            if(activeDirectoryAuthenticator.Authenticate(upn, password, externalIdentitySource.Domain))
            {
                this.IncrementSuccessfulAuthentication(authenticablePrincipal, domain);
                return ConstructClaimsPrincipal(authenticablePrincipal, externalIdentitySource.Name);
            }
            else
            {
                throw new Exception("Authentication failed");
            }
                
        }

        public ClaimsPrincipal Authenticate(string upn)
        {
            AuthenticablePrincipal authenticablePrincipal = localIdentityProviderLogic.Authenticate(upn);

            return ConstructClaimsPrincipal(authenticablePrincipal, devAuthBypass);
        }

        private void IncrementSuccessfulAuthentication(AuthenticablePrincipal authenticablePrincipal, Guid realm)
        {
            authenticablePrincipal.LastLogonDate = DateTime.Now;
            authenticablePrincipal.LastLogonRealm = realm;

            configurationRepository.UpdateAuthenticablePrincipal(authenticablePrincipal);
        }
    }
}
