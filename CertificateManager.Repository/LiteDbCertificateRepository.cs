using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CertificateManager.Repository
{
    public class LiteDbCertificateRepository : ICertificateRepository
    {
        private string path;

        private LiteDatabase db;
        private CollectionDiscoveryLogic collectionDiscoveryLogic;

        public LiteDbCertificateRepository(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            db = new LiteDatabase(path);
            this.path = path;

            this.collectionDiscoveryLogic = new CollectionDiscoveryLogic();
        }

        public void DeleteAllCertificates()
        {
            db.DropCollection(collectionDiscoveryLogic.GetName<Certificate>());
        }

        public void DeleteCertificate()
        {
            throw new NotImplementedException();
        }

        public StoredCertificateEntity GetCertificate()
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T item)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            col.Update(item);
        }

        public void Insert<T>(T item)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            col.Insert(item);
        }

        public void UpdateCertificate()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SearchCertificatesEntity> FindCertificates(string query)
        {
            LiteCollection<SearchCertificatesEntity> col = db.GetCollection<SearchCertificatesEntity>(collectionDiscoveryLogic.GetName<SearchCertificatesEntity>());
            return col.FindAll();
        }

        public IEnumerable<SearchCertificatesEntity> FindCertificatesAmbiguousNameResolution(string args)
        {
            Query left = Query.Or(
                    Query.Contains("Thumbprint", args),
                    Query.Contains("DisplayName", args)
                );

            LiteCollection<SearchCertificatesEntity> col = db.GetCollection<SearchCertificatesEntity>(collectionDiscoveryLogic.GetName<SearchCertificatesEntity>());

            return col.Find(left);

        }

        public T Get<T>(Guid id)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            T item = col.FindById(id);

            if(item == null)
            {
                throw new ReferencedObjectDoesNotExistException();
            }
            else
            {
                return item;
            }
        }

        public IEnumerable<T> GetAll<T>()
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            return col.FindAll();
        }

        public void Delete<T>(Guid id)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            col.Delete(id);
        }

        public IEnumerable<T> Get<T>(Expression<Func<T, bool>> query)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            return col.Find(query);
        }

    }
}
