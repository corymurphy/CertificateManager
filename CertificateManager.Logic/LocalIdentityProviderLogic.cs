using CertificateManager.Entities;
using CertificateManager.Repository;
using System;

namespace CertificateManager.Logic
{
    public class LocalIdentityProviderLogic
    {
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
                UserPrincipalName = "emergencyaccess@certificatemanager.local",
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
                UserPrincipalName = "administrator@certificatemanager.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(secret)
            };

            configurationRepository.InsertAuthenticablePrincipal(authenticablePrincipal);

            return authenticablePrincipal;
        }

        public AuthenticablePrincipal Authenticate(string upn, string password)
        {
            AuthenticablePrincipal authenticablePrincipal = configurationRepository.GetAuthenticablePrincipal(upn);

            if (authenticablePrincipal == null)
                throw new Exception("User does not exist");


            if (!BCrypt.Net.BCrypt.Verify(password, authenticablePrincipal.PasswordHash))
                throw new Exception("Authentication failed");

            return authenticablePrincipal;
        }

        public AuthenticablePrincipal Authenticate(string upn)
        {
            AuthenticablePrincipal authenticablePrincipal = configurationRepository.GetAuthenticablePrincipal(upn);

            if (authenticablePrincipal == null)
                throw new Exception("User does not exist");

            return authenticablePrincipal;
        }

        public void InitializeRoles()
        {
            SecurityRole role = new SecurityRole()
            {

            };

        }


    }
}
