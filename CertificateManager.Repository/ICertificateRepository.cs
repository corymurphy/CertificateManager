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
        void DeleteCertificate();
        void UpdateCertificate();

        IEnumerable<AllCertificatesViewModel> FindAllCertificates();

        IEnumerable<SearchCertificatesEntity> FindCertificates(string query);

        IEnumerable<SearchCertificatesEntity> FindCertificatesAmbiguousNameResolution(string query);
    }
}
