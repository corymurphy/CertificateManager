using CertificateManager.Entities;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CertificateManager.Logic
{
    public class InitialSetupLogic
    {
        //IRuntimeConfigurationState runtimeConfigurationState;
        IConfigurationRepository configurationRepository;
        LocalIdentityProviderLogic localIdpLogic;
        ActiveDirectoryIdentityProviderLogic idpLogic;
        CertificateAuthorityConfigurationLogic certificateAuthorityConfigurationLogic;
        AdcsTemplateLogic templateLogic;
        RoleManagementLogic roleManagement;
        IAuthorizationLogic authorizationLogic;
        //SecretKeyProvider keyProvider;

        public InitialSetupLogic(IConfigurationRepository configurationRepository)
        {
            //this.runtimeConfigurationState = runtimeConfigurationState;
            this.configurationRepository = configurationRepository;
            this.templateLogic = new AdcsTemplateLogic(configurationRepository);
            this.certificateAuthorityConfigurationLogic = new CertificateAuthorityConfigurationLogic(configurationRepository);
            this.idpLogic = new ActiveDirectoryIdentityProviderLogic(configurationRepository);
            this.authorizationLogic = new AuthorizeAllLogic(configurationRepository);
            this.roleManagement = new RoleManagementLogic(configurationRepository, authorizationLogic);
            this.localIdpLogic = new LocalIdentityProviderLogic(configurationRepository);
        }
        

        public void Complete(InitialSetupConfigModel config)
        {

            ExternalIdentitySource idp = idpLogic.Add(config.AdName, config.AdServer, config.AdSearchBase, config.AdServiceAccountUsername, config.AdServiceAccountPassword, config.AdUseAppPoolIdentity);
            certificateAuthorityConfigurationLogic.AddPrivateCertificateAuthority(config.AdcsServerName, config.AdcsCommonName, config.AdcsHashAlgorithm, idp.Id);
            AdcsTemplate adcsTemplate = templateLogic.AddTemplate(config.AdcsTemplateName, config.AdcsTemplateCipher, config.AdcsTemplateKeyUsage, config.AdcsTemplateWindowsApi, RoleManagementLogic.DefaultTemplateIssuerRoles);
            AuthenticablePrincipal emergencyAccess = localIdpLogic.InitializeEmergencyAccess(config.EmergencyAccessKey);
            AuthenticablePrincipal admin = localIdpLogic.InitializeLocalAdministrator(config.LocalAdminPassword);
            authorizationLogic.InitializeScopes();
            roleManagement.InitializeRoles(new List<Guid>() { emergencyAccess.Id, admin.Id });

            AppConfig appConfig = new AppConfig()
            {
                EncryptionKey = config.EncryptionKey,
                Id = Guid.NewGuid()
            };

            configurationRepository.SetAppConfig(appConfig);
        }

    }
}
