using CertificateManager.Entities;

namespace CertificateManager.Repository
{
    public interface IAuditRepository
    {
        void InsertAuditEvent(AuditEvent entity);
    }
}
