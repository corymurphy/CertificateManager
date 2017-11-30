using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Repository;
using System;

namespace CertificateManager.Logic
{
    public class LocalIdentityProviderLogic
    {
        public delegate void AuthenticationLogicEventRaiser();
        //public event AuthenticationLogicEventRaiser OnSuccessfulUserAuthentication;

        IConfigurationRepository configurationRepository;

        public LocalIdentityProviderLogic(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        public string GetLocalIdpIdentifier()
        {
            AppConfig appConfig = configurationRepository.GetAppConfig();
            return appConfig.LocalIdpIdentifier;
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

            configurationRepository.InsertAuthenticablePrincipal(authenticablePrincipal);

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

            configurationRepository.InsertAuthenticablePrincipal(authenticablePrincipal);

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
