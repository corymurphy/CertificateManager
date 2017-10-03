using CertificateManager.Entities.Enumerations;
using System;

namespace CertificateManager.Entities
{
    public class ConfigurationAlert
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Closed { get; set; }
        public AlertState AlertState { get; set; }
        public AlertType AlertType { get; set; }
        public AlertSeverity AlertSeverity { get; set; }
        public string Message { get; set; }
    }
}
