using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificateManager.Logic
{
    public class AuditLogic : IAuditLogic
    {
        IAuditRepository auditRepository;
        IConfigurationRepository configurationRepository;

        public AuditLogic(IAuditRepository auditRepository, IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
            this.auditRepository = auditRepository;
        }
        public void LogSecurityAuditSuccess(Guid userid, string upn, string target, EventCategory category)
        {

            AppConfig appConfig = configurationRepository.GetAppConfig();
            
            if(appConfig.SecurityAuditingState == SecurityAuditingState.Success)
            {
                AuditEvent auditEvent = new AuditEvent
                {
                    Target = target,
                    EventCategory = category,
                    UserId = userid,
                    UserPrincipalName = upn,
                    Time = DateTime.Now,
                    EventResult = EventResult.Success
                };
                Task.Run(() => auditRepository.InsertAuditEvent(auditEvent));
            }
            
        }

        public void LogSecurityAuditFailure(string target, string source)
        {

        }

        public void LogOpsError(Guid userid, string upn, string target, EventCategory category)
        {
            AppConfig appConfig = configurationRepository.GetAppConfig();

            if (appConfig.OperationsLoggingState == OperationsLoggingState.Errors)
            {
                AuditEvent auditEvent = new AuditEvent
                {
                    Target = target,
                    EventCategory = category,
                    UserId = userid,
                    UserPrincipalName = upn,
                    Time = DateTime.Now,
                    EventResult = EventResult.Success
                };
                Task.Run(() => auditRepository.InsertAuditEvent(auditEvent));
            }
            
        }

        public void Log(AuditEvent auditEvent)
        {

            Task.Run(() => auditRepository.InsertAuditEvent(auditEvent));
        }

        public IEnumerable<AuditEvent> GetAllEvents()
        {
            return auditRepository.GetAllEvents();
        }
    }
}
