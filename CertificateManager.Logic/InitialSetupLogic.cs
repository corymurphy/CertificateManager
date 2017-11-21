using CertificateManager.Entities;
using CertificateManager.Repository;

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
        SecretKeyProvider keyProvider;

        public InitialSetupLogic(IConfigurationRepository configurationRepository)
        {
            //this.runtimeConfigurationState = runtimeConfigurationState;
            this.configurationRepository = configurationRepository;
            this.templateLogic = new AdcsTemplateLogic(configurationRepository);
            this.certificateAuthorityConfigurationLogic = new CertificateAuthorityConfigurationLogic(configurationRepository);
            this.idpLogic = new ActiveDirectoryIdentityProviderLogic(configurationRepository);
            this.localIdpLogic = new LocalIdentityProviderLogic(configurationRepository);

        }
        

        public void SetConfiguration(InitialSetupConfigModel config)
        {
            ExternalIdentitySource idp = idpLogic.Add(config.AdName, config.AdServer, config.AdSearchBase, config.AdServiceAccountUsername, config.AdServiceAccountPassword, config.AdUseAppPoolIdentity);
            certificateAuthorityConfigurationLogic.AddPrivateCertificateAuthority(config.AdcsServerName, config.AdcsCommonName, config.AdcsHashAlgorithm, idp.Id);
            AdcsTemplate adcsTemplate = templateLogic.AddTemplate(config.AdcsTemplateName, config.AdcsTemplateCipher, config.AdcsTemplateKeyUsage, config.AdcsTemplateWindowsApi);
            localIdpLogic.InitializeEmergencyAccess(config.EmergencyAccessKey);

            AppConfig appConfig = new AppConfig()
            {
                EncryptionKey = config.EncryptionKey
            };

            configurationRepository.SetAppConfig(appConfig);

            //runtimeConfigurationState.InitialSetupComplete = true;
        }
    }
}
