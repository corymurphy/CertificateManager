using CertificateManager.Entities;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CertificateManager.Repository
{
    public class LiteDbAuditRepository : IAuditRepository
    {
        private string auditCollectionName = "audit";
        private LiteDatabase db;
        private readonly CollectionDiscoveryLogic collectionDiscoveryLogic;

        public LiteDbAuditRepository(string path)
        {
            db = new LiteDatabase(path);
            this.collectionDiscoveryLogic = new CollectionDiscoveryLogic();
        }

        public void InsertAuditEvent(AuditEvent entity)
        {
            LiteCollection<AuditEvent> col =  db.GetCollection<AuditEvent>(auditCollectionName);
            col.Insert(entity);
        }

        public IEnumerable<AuditEvent> GetAllEvents()
        {
            LiteCollection<AuditEvent> col = db.GetCollection<AuditEvent>(auditCollectionName);
            return col.FindAll();
                
        }

        public IEnumerable<T> Get<T>(Expression<Func<T, bool>> query)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            return col.Find(query);
        }
    }
}
