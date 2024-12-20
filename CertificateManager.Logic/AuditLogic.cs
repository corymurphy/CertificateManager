﻿using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using CertificateManager.Entities.Extensions;
using CertificateManager.Entities.Interfaces;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Security.Claims;
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

        public void LogSecurityAuditSuccess(ClaimsPrincipal userContext, ILoggableEntity entity, EventCategory category)
        {
            AppConfig appConfig = configurationRepository.GetAppConfig();

            if (appConfig.SecurityAuditingState == SecurityAuditingState.Success)
            {
                AuditEvent auditEvent = new AuditEvent
                {
                    Id = Guid.NewGuid(),
                    Target = entity.GetId(),
                    TargetDescription = entity.GetDescription(),
                    EventCategory = category,
                    UserId = userContext.GetUserId(),
                    UserDisplay = userContext.GetName(),
                    Time = DateTime.Now,
                    EventResult = EventResult.Success
                };
                Task.Run(() => auditRepository.InsertAuditEvent(auditEvent));
            }
        }

        public void LogSecurityAuditFailure(ClaimsPrincipal userContext, ILoggableEntity entity, EventCategory category)
        {
            AppConfig appConfig = configurationRepository.GetAppConfig();

            if (appConfig.SecurityAuditingState == SecurityAuditingState.Success)
            {
                AuditEvent auditEvent = new AuditEvent
                {
                    Id = Guid.NewGuid(),
                    Target = userContext.GetName(),
                    EventCategory = category,
                    UserId = userContext.GetUserId(),
                    UserDisplay = userContext.GetName(),
                    Time = DateTime.Now,
                    EventResult = EventResult.Failure
                };
                Task.Run(() => auditRepository.InsertAuditEvent(auditEvent));
            }
        }

        public void LogOpsError(ClaimsPrincipal userContext, string target, EventCategory category)
        {
            AppConfig appConfig = configurationRepository.GetAppConfig();

            if (appConfig.OperationsLoggingState == OperationsLoggingState.Errors)
            {
                AuditEvent auditEvent = new AuditEvent
                {
                    Id = Guid.NewGuid(),
                    Target = target,
                    EventCategory = category,
                    UserId = userContext.GetUserId(),
                    UserDisplay = userContext.GetName(),
                    Time = DateTime.Now,
                    EventResult = EventResult.Error
                };
                Task.Run(() => auditRepository.InsertAuditEvent(auditEvent));
            }
        }

        public void LogOpsError(ClaimsPrincipal userContext, string target, EventCategory category, string message)
        {
            AppConfig appConfig = configurationRepository.GetAppConfig();

            if (appConfig.OperationsLoggingState == OperationsLoggingState.Errors)
            {
                AuditEvent auditEvent = new AuditEvent
                {
                    Id = Guid.NewGuid(),
                    Target = target,
                    EventCategory = category,
                    UserId = userContext.GetUserId(),
                    UserDisplay = userContext.GetName(),
                    Time = DateTime.Now,
                    EventResult = EventResult.Error,
                    Message = message
                };
                Task.Run(() => auditRepository.InsertAuditEvent(auditEvent));
            }
        }

        public void LogOpsSuccess(ClaimsPrincipal userContext, string target, EventCategory category, string message)
        {
            AppConfig appConfig = configurationRepository.GetAppConfig();

            AuditEvent auditEvent = new AuditEvent
            {
                Id = Guid.NewGuid(),
                Target = target,
                EventCategory = category,
                UserId = userContext.GetUserId(),
                UserDisplay = userContext.GetName(),
                Time = DateTime.Now,
                EventResult = EventResult.Success,
                Message = message
            };
            auditRepository.InsertAuditEvent(auditEvent);
            //Task.Run(() => auditRepository.InsertAuditEvent(auditEvent));

            if (appConfig.OperationsLoggingState == OperationsLoggingState.Errors)
            {

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

        public void InitializeMockData()
        {
            for (int i = -10; i <= 0; i++)
            {
                DateTime day = DateTime.Now;
                day = day.AddDays(i);

                int certCount = new Random().Next(100, 5000);

                int index = 0;
                while (index < certCount)
                {
                    AuditEvent log = new AuditEvent()
                    {
                        Time = day
                    };

                    auditRepository.InsertAuditEvent(log);
                    //certificateRepository.Insert<Certificate>(newCert);

                    index++;
                }

            }
        }

        public void ClearLogs(ClaimsPrincipal user)
        {
            auditRepository.DeleteAll();

            AuditEvent log = new AuditEvent()
            {
                EventCategory = EventCategory.LogCleared,
                EventResult = EventResult.Success,
                UserId = user.GetUserId(),
                Message = "Log was successfully cleared",
                UserDisplay = user.GetName(),
                Target = "Audit Logs",
                Time = DateTime.Now
            };

            this.Log(log);
        }
    }
}
