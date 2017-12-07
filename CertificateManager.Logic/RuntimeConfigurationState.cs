using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using CertificateManager.Repository;
using CertificateServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CertificateManager.Logic
{
    public class RuntimeConfigurationState : IRuntimeConfigurationState
    {
        IConfigurationRepository configurationRepository;
        IRuntimeCacheRepository runtimeCacheRepository;

        public bool IsDevelopment { get; set; }

        public bool InitialSetupComplete { get; set; }

        public RuntimeConfigurationState(IConfigurationRepository configurationRepository, IRuntimeCacheRepository runtimeCacheRepository)
        {
            this.configurationRepository = configurationRepository;
            this.runtimeCacheRepository = runtimeCacheRepository;
        }

        public IEnumerable<ConfigurationAlert> GetAlerts()
        {
            return runtimeCacheRepository.GetConfigurationAlerts();
        }

        public void AlertApplicationStarted()
        {
            if (runtimeCacheRepository.ConfigurationAlertExists(AlertType.ApplicationStartedSuccessfully))
            {
                foreach (var item in runtimeCacheRepository.GetConfigurationAlerts())
                {
                    runtimeCacheRepository.DeleteConfigurationAlert(item);
                }
            }

            runtimeCacheRepository.InsertConfigurationAlert(new ConfigurationAlert()
            {
                AlertSeverity = AlertSeverity.Information,
                AlertState = AlertState.Open,
                AlertType = AlertType.ApplicationStartedSuccessfully,
                Created = DateTime.Now,
                Id = Guid.NewGuid(),
                Message = "Application Started Successfully"
            });
        }

        public void Validate()
        {


            //Ensure there is at least one certificate authority stored
            this.ValidateAtLeastOneCertificateAuthority();

            //Test the connection to the certificate authority
            this.TestCertificateAuthorityConnectivity();

            //Test authorization for certificate authority

            //Check if there is at least one template availible
            this.ValidateAtLeastOneTemplate();

            //Ensure the templates are published by all certificate authorities
            //Ensure the stored configuration for the templates match the actual templates published

            //Test template enroll authorization
            this.TestTemplateEnrollAuthorization();

        }

        public void ValidateAtLeastOneCertificateAuthority()
        {
            IEnumerable<PrivateCertificateAuthorityConfig> found = configurationRepository.GetAll<PrivateCertificateAuthorityConfig>();

            if(found == null && !runtimeCacheRepository.ConfigurationAlertExists(AlertType.NoCertificateAuthoritiesConfigured))
            {
                ConfigurationAlert alert = new ConfigurationAlert()
                {
                    Created = DateTime.Now,
                    AlertState = AlertState.Open,
                    AlertSeverity = AlertSeverity.Error,
                    AlertType = AlertType.NoCertificateAuthoritiesConfigured,
                    Id = Guid.NewGuid(),
                    Message = "There must be at least one certificate authority before certificate manager can issue new certificates."

                };

                runtimeCacheRepository.InsertConfigurationAlert(alert);
            }

        }

        public void ValidateAtLeastOneTemplate()
        {
            IEnumerable<AdcsTemplate> found = configurationRepository.GetAll<AdcsTemplate>();

            if (!found.Any() && !runtimeCacheRepository.ConfigurationAlertExists(AlertType.NoTemplatesConfigured))
            {
                runtimeCacheRepository.InsertConfigurationAlert(new ConfigurationAlert()
                {
                    Created = DateTime.Now,
                    AlertState = AlertState.Open,
                    AlertSeverity = AlertSeverity.Error,
                    AlertType = AlertType.NoTemplatesConfigured,
                    Id = Guid.NewGuid(),
                    Message = "There must be at least one template before certificate manager can issue new certificates."
                });
            }
        }

        public void TestCertificateAuthorityConnectivity()
        {

        }

        public void TestCertificateAuthorityAuthorization()
        {

        }

        public void TestTemplateEnrollAuthorization()
        {

        }

        public void InitializeCache()
        {
            throw new NotImplementedException();
        }

        public void ClearAlert(AlertType type)
        {
            if(runtimeCacheRepository.ConfigurationAlertExists(type))
            {
                foreach(ConfigurationAlert alert in runtimeCacheRepository.GetConfigurationAlerts(type))
                {
                    runtimeCacheRepository.DeleteConfigurationAlert(alert);
                }
            }
        }
    }
}
