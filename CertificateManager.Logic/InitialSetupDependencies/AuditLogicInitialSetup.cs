using CertificateManager.Entities;
using CertificateManager.Entities.Interfaces;
using CertificateManager.Logic.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;

namespace CertificateManager.Logic.InitialSetupDependencies
{
    public class AuditLogicInitialSetup : IAuditLogic
    {
        public IEnumerable<AuditEvent> GetAllEvents()
        {
            return null;
        }

        public void InitializeMockData()
        {
            throw new System.NotImplementedException();
        }

        public void LogOpsError(ClaimsPrincipal userContext, string target, EventCategory category)
        {
            
        }

        public void LogOpsError(ClaimsPrincipal userContext, string target, EventCategory category, string message)
        {

        }

        public void LogOpsSuccess(ClaimsPrincipal userContext, string target, EventCategory category, string message)
        {

        }

        public void LogSecurityAuditFailure(ClaimsPrincipal userContext, ILoggableEntity entity, EventCategory category)
        {
            
        }

        public void LogSecurityAuditSuccess(ClaimsPrincipal userContext, ILoggableEntity entity, EventCategory category)
        {
            
        }
    }
}
