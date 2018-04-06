using CertificateManager.Entities;
using CertificateManager.Entities.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;

namespace CertificateManager.Logic.Interfaces
{
    public interface IAuditLogic
    {
        IEnumerable<AuditEvent> GetAllEvents();
        void LogSecurityAuditSuccess(ClaimsPrincipal userContext, ILoggableEntity entity, EventCategory category);
        void LogSecurityAuditFailure(ClaimsPrincipal userContext, ILoggableEntity entity, EventCategory category);
        void LogOpsSuccess(ClaimsPrincipal userContext, string target, EventCategory category, string message);
        void LogOpsError(ClaimsPrincipal userContext, string target, EventCategory category);
        void LogOpsError(ClaimsPrincipal userContext, string target, EventCategory category, string message);
        void InitializeMockData();
        void ClearLogs(ClaimsPrincipal user);
    }
}
