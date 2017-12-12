using CertificateManager.Entities;
using System.Collections.Generic;

namespace CertificateManager.Repository
{
    public interface IAuditRepository
    {
        void InsertAuditEvent(AuditEvent entity);

        IEnumerable<AuditEvent> GetAllEvents();
    }
}
