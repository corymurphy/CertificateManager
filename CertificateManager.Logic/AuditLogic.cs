using CertificateManager.Entities;
using CertificateManager.Repository;
using System.Threading.Tasks;

namespace CertificateManager.Logic
{
    public class AuditLogic
    {
        IAuditRepository auditRepository;

        public AuditLogic(IAuditRepository auditRepository)
        {
            this.auditRepository = auditRepository;
        }

        public void Log(AuditEvent auditEvent)
        {
            Task.Run(() => auditRepository.InsertAuditEvent(auditEvent));
        }
    }
}
