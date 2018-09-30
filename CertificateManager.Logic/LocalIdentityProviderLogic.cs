using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CertificateManager.Logic
{
    public class LocalIdentityProviderLogic
    {
        public delegate void AuthenticationLogicEventRaiser();
        //public event AuthenticationLogicEventRaiser OnSuccessfulUserAuthentication;

        IConfigurationRepository configurationRepository;

        public static Guid SystemUid { get { return new Guid("53a90eeb-7f59-4e03-b109-38b20faf3877");  } }

        public LocalIdentityProviderLogic(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        public string GetLocalIdpIdentifier()
        {
            AppConfig appConfig = configurationRepository.GetAppConfig();
            return appConfig.LocalIdpIdentifier;
        }

        public static ClaimsPrincipal GetSystemIdentity()
        {
            Claim name = new Claim(WellKnownClaim.Name, "SYSTEM");
            Claim uid = new Claim(WellKnownClaim.Uid, LocalIdentityProviderLogic.SystemUid.ToString());
            List<Claim> claims = new List<Claim>() { name, uid };

            return new ClaimsPrincipal( new ClaimsIdentity(claims) );
        }

        public AuthenticablePrincipal InitializeEmergencyAccess(string secret)
        {
            AuthenticablePrincipal authenticablePrincipal = new AuthenticablePrincipal()
            {
                LocalLogonEnabled = true,
                Id = Guid.NewGuid(),
                Enabled = true,
                Name = "emergencyaccess@certificatemanager.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(secret)
            };

            configurationRepository.Insert<AuthenticablePrincipal>(authenticablePrincipal);

            return authenticablePrincipal;
        }

        public AuthenticablePrincipal InitializeLocalAdministrator(string secret)
        {
            AuthenticablePrincipal authenticablePrincipal = new AuthenticablePrincipal()
            {
                LocalLogonEnabled = true,
                Id = Guid.NewGuid(),
                Enabled = true,
                Name = "administrator@certificatemanager.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(secret)
            };

            configurationRepository.Insert<AuthenticablePrincipal>(authenticablePrincipal);

            return authenticablePrincipal;
        }

        public AuthenticablePrincipal Authenticate(string upn, string password)
        {
            AuthenticablePrincipal authenticablePrincipal = configurationRepository.GetAuthenticablePrincipal(upn);

            if (authenticablePrincipal == null)
                throw new AuthenticablePrincipalDoesNotExistException();

            if (!authenticablePrincipal.Enabled)
                throw new AuthenticablePrincipalDeniedLoginByPolicyException();

            if (!BCrypt.Net.BCrypt.Verify(password, authenticablePrincipal.PasswordHash))
                throw new CredentialsInvalidForAuthenticablePrincipalException("Authentication failed");

            //OnSuccessfulUserAuthentication();

            return authenticablePrincipal;
        }

        public AuthenticablePrincipal Authenticate(string upn)
        {
            AuthenticablePrincipal authenticablePrincipal = configurationRepository.GetAuthenticablePrincipal(upn);

            if (authenticablePrincipal == null)
                throw new Exception("User does not exist");

            //OnSuccessfulUserAuthentication();

            return authenticablePrincipal;
        }

        //public 


    }
}
