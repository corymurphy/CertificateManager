using CertificateManager.Entities;
using System;
using System.Collections.Generic;

namespace CertificateManager.Repository
{
    public interface ICertificateRepository
    {
        void Insert<T>(T item);

        StoredCertificateEntity GetCertificate();

        T Get<T>(Guid id);

        IEnumerable<T> GetAll<T>();

        void DeleteCertificate();

        void Update<T>(T item);

        void DeleteAllCertificates();

        void Delete<T>(Guid id);

        IEnumerable<SearchCertificatesEntity> FindCertificates(string query);

        IEnumerable<SearchCertificatesEntity> FindCertificatesAmbiguousNameResolution(string query);
    }
}
