using CertificateManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CertificateManager.Repository
{
    public interface IAuditRepository
    {
        void InsertAuditEvent(AuditEvent entity);

        IEnumerable<AuditEvent> GetAllEvents();

        IEnumerable<T> Get<T>(Expression<Func<T, bool>> query);

        void DeleteAll();
    }
}
