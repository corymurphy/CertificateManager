using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CertificateManager.Entities;
using CertificateServices;
using CertificateServices.Enumerations;

namespace CertificateManager.Repository
{
    public class ConfigurationRepositoryInitialSetupEmpty : IConfigurationRepository
    {

        public AppConfig GetAppConfig()
        {
            throw new NotImplementedException();
        }



        public AuthenticablePrincipal GetAuthenticablePrincipal(string upn)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<SecurityRole> GetAuthenticablePrincipalMemberOf(Guid id)
        {
            throw new NotImplementedException();
        }


        public MicrosoftCertificateAuthority GetPrivateCertificateAuthority(HashAlgorithm hash)
        {
            throw new NotImplementedException();
        }

        public MicrosoftCertificateAuthorityOptions GetPrivateCertificateAuthorityOptions(HashAlgorithm hash)
        {
            throw new NotImplementedException();
        }


        public void Insert<T>(T item)
        {
            throw new NotImplementedException();
        }


        public void SetAppConfig(AppConfig appConfig)
        {
            throw new NotImplementedException();
        }


        public bool UserPrincipalNameExists(string upn, Guid ignoreUserId)
        {
            throw new NotImplementedException();
        }

        public bool UserPrincipalNameExists(string upn)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Scope> GetAvailibleScopes()
        {
            throw new NotImplementedException();
        }

        public void InsertScopes(List<Scope> scopes)
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll<T>()
        {
            throw new NotImplementedException();
        }

        public void DropCollection<T>()
        {
            throw new NotImplementedException();
        }

        public bool Exists<T>(Guid id)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AdcsTemplate> Get<T>(Expression<Func<AdcsTemplate, bool>> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get<T>(Expression<Func<T, bool>> query)
        {
            throw new NotImplementedException();
        }

        public bool Exists<T>(Expression<Func<T, bool>> query)
        {
            throw new NotImplementedException();
        }
    }
}
