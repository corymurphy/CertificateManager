using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using System.Collections.Generic;

namespace CertificateManager.Logic
{
    public interface IRuntimeConfigurationState
    {
        bool IsDevelopment { get; set; }
        void AlertApplicationStarted();
        void Validate();
        void ValidateAtLeastOneCertificateAuthority();
        void ValidateAtLeastOneTemplate();
        void TestCertificateAuthorityConnectivity();
        void TestCertificateAuthorityAuthorization();
        void TestTemplateEnrollAuthorization();
        void InitializeCache();

        void ClearAlert(AlertType type);
        IEnumerable<ConfigurationAlert> GetAlerts();
    }
}
