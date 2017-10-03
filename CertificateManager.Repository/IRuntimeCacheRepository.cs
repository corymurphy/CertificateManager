using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using System.Collections.Generic;

namespace CertificateManager.Repository
{
    public interface IRuntimeCacheRepository
    {
        bool ConfigurationAlertExists(AlertType type);
        void InsertConfigurationAlert(ConfigurationAlert alert);
        void UpdateConfigurationAlert(ConfigurationAlert alert);
        void DeleteConfigurationAlert(ConfigurationAlert alert);
        IEnumerable<ConfigurationAlert> GetConfigurationAlerts();
        IEnumerable<ConfigurationAlert> GetConfigurationAlerts(AlertType type);

    }
}
