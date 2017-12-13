using CertificateManager.Entities;
using CertificateManager.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CertificateManager.Logic.Interfaces
{
    public interface IAuditLogic
    {
        IEnumerable<AuditEvent> GetAllEvents();
        //void LogSecurityAuditSuccess(Guid userid, string upn, string target, EventCategory category);
        //void LogSecurityAuditSuccess(Guid userid, string upn, string target, string targetDisplay, EventCategory category);
        void LogSecurityAuditSuccess(ClaimsPrincipal userContext, ILoggableEntity entity, EventCategory category);
        void LogSecurityAuditFailure(ClaimsPrincipal userContext, ILoggableEntity entity, EventCategory category);
        void LogOpsError(ClaimsPrincipal userContext, string target, EventCategory category);

    }
}
