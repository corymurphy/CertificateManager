using CertificateManager.Entities;
using LiteDB;
using System.Collections.Generic;

namespace CertificateManager.Repository
{
    public class LiteDbAuditRepository : IAuditRepository
    {
        private string auditCollectionName = "audit";
        private LiteDatabase db;

        public LiteDbAuditRepository(string path)
        {
            db = new LiteDatabase(path);
        }

        public LiteDbAuditRepository()
        {
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
    }
}
