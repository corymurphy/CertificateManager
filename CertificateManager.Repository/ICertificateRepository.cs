using CertificateManager.Entities;
using System;
using System.Collections.Generic;

namespace CertificateManager.Repository
{
    public interface ICertificateRepository
    {
        void Insert<T>(T item);

        StoredCertificateEntity GetCertificate();

        T GetCertificate<T>(Guid id);

        T Get<T>(Guid id);

        IEnumerable<T> GetAll<T>();

        void DeleteCertificate();

        void UpdateCertificate();

        void DeleteAllCertificates();

        IEnumerable<AllCertificatesViewModel> FindAllCertificates();

        IEnumerable<SearchCertificatesEntity> FindCertificates(string query);

        IEnumerable<SearchCertificatesEntity> FindCertificatesAmbiguousNameResolution(string query);
    }
}
