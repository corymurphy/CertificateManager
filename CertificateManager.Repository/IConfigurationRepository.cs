using CertificateManager.Entities;
using CertificateServices;
using CertificateServices.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CertificateManager.Repository
{
    public interface IConfigurationRepository
    {
        T Get<T>(Guid id);
        void Delete<T>(Guid id);
        void Insert<T>(T item);

        void Update<T>(T item);

        IEnumerable<T> GetAll<T>();

        void DropCollection<T>();

        bool Exists<T>(Expression<Func<T, bool>> query);

        bool Exists<T>(Guid id);

        IEnumerable<T> Get<T>(Expression<Func<T, bool>> query);

        MicrosoftCertificateAuthorityOptions GetPrivateCertificateAuthorityOptions(HashAlgorithm hash);
        MicrosoftCertificateAuthority GetPrivateCertificateAuthority(HashAlgorithm hash);
        AuthenticablePrincipal GetAuthenticablePrincipal(string upn);
        bool UserPrincipalNameExists(string upn, Guid ignoreUserId);
        //bool UserPrincipalNameExists(string upn);
        IEnumerable<SecurityRole> GetAuthenticablePrincipalMemberOf(Guid id);




        IEnumerable<Scope> GetAvailibleScopes();
        void InsertScopes(List<Scope> scopes);

        AppConfig GetAppConfig();
        void SetAppConfig(AppConfig appConfig);

    }
}
