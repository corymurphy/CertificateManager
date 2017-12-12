using CertificateManager.Entities;
using System;
using System.Collections.Generic;

namespace CertificateManager.Logic.Interfaces
{
    public interface IAuditLogic
    {
        IEnumerable<AuditEvent> GetAllEvents();
        void LogSecurityAuditSuccess(Guid userid, string upn, string target, EventCategory category);
        void LogSecurityAuditFailure(string target, string source);
        void LogOpsError(Guid userid, string upn, string target, EventCategory category);

    }
}
