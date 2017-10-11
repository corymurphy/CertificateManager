using System;
using CertificateManager.Entities;
using LiteDB;
using System.Collections;
using System.Collections.Generic;

namespace CertificateManager.Repository
{
    public class LiteDbCertificateRepository : ICertificateRepository
    {
        private string path;
        private const string certificateCollection = "Certificate";

        private LiteDatabase db;

        public LiteDbCertificateRepository(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            db = new LiteDatabase(path);
            this.path = path;
        }

        public void DeleteAllCertificates()
        {
            db.DropCollection(certificateCollection);
        }

        public void DeleteCertificate()
        {
            throw new NotImplementedException();
        }

        public StoredCertificateEntity GetCertificate()
        {
            throw new NotImplementedException();
        }

        public T GetCertificate<T>(Guid id)
        {
            LiteCollection<T> col = db.GetCollection<T>(certificateCollection);
            return col.FindById(id);
        }

        public void Insert<T>(T item)
        {

            LiteCollection<T> col = db.GetCollection<T>();
            col.Insert(item);
        }

        public void UpdateCertificate()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SearchCertificatesEntity> FindCertificates(string query)
        {
            LiteCollection<SearchCertificatesEntity> col = db.GetCollection<SearchCertificatesEntity>(certificateCollection);
            return col.FindAll();
        }

        public IEnumerable<SearchCertificatesEntity> FindCertificatesAmbiguousNameResolution(string args)
        {
            Query left = Query.Or(
                    Query.Contains("Thumbprint", args),
                    Query.Contains("DisplayName", args)
                );

            LiteCollection<SearchCertificatesEntity> col = db.GetCollection<SearchCertificatesEntity>(certificateCollection);

            return col.Find(left);

        }

        public IEnumerable<AllCertificatesViewModel> FindAllCertificates()
        {
            LiteCollection<AllCertificatesViewModel> col = db.GetCollection<AllCertificatesViewModel>(certificateCollection);
            return col.FindAll();
        }
    }
}
