using CertificateManager.Entities.Enumerations;

namespace CertificateManager.Entities
{
    public class AuditConfigurationModel
    {
        public SecurityAuditingState SecurityAuditingState { get; set; }
        public OperationsLoggingState OperationsLoggingState { get; set; }
    }
}
