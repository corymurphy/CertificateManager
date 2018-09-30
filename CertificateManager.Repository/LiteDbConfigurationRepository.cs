using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateServices;
using CertificateServices.Enumerations;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CertificateManager.Repository
{
    public class LiteDbConfigurationRepository : IConfigurationRepository
    {
        private const string securityRoleCollectionName = "secroles";
        private const string authenticablePrincipalCollectionName = "usr";
        private const string adcsNotFoundExceptionMessage = "Could not find adcs template that matched the query {0} and {1}";
        private const string adcsTemplateCollectionName = "Template";
        private const string privateCertificateAuthorityCollectionName = "PrivateCa";
        private const string AdcsTemplateCollectionName = "AdcsTemplate";
        private const string ActiveDirectoryMetadataCollectionName = "extid";
        private const string authApiCertificateCollectionName = "authcer";
        private const string appConfigCollectionName = "appcfg";
        private const string scopesCollectionName = "scopes";
        private string path;

        private LiteDatabase db;
        private CollectionDiscoveryLogic collectionDiscoveryLogic;

        private AppConfig cachedConfig;
        private DateTime cacheRefreshTime;

        public LiteDbConfigurationRepository(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            db = new LiteDatabase(path);
            this.path = path;

            this.collectionDiscoveryLogic = new CollectionDiscoveryLogic();
        }


        public void DropCollection<T>()
        {
            db.DropCollection(collectionDiscoveryLogic.GetName<T>());
        }

        public void Delete<T>(Guid id)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            col.Delete(id);
        }

        public void Insert<T>(T item)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            col.Insert(item);
        }

        public void Update<T>(T item)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            col.Update(item);
        }

        public IEnumerable<T> GetAll<T>()
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            return col.FindAll();
        }

        public T Get<T>(Guid id)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            return col.FindById(id);
        }

        public bool Exists<T>(Expression<Func<T, bool>> query)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            return col.Exists(query);
        }

        public bool Exists<T>(Guid id)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            return col.FindById(id) != null;
        }

        public IEnumerable<T> Get<T>(Expression<Func<T, bool>> query)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            return col.Find(query);
        }

        public MicrosoftCertificateAuthorityOptions GetPrivateCertificateAuthorityOptions(HashAlgorithm hash)
        {

            PrivateCertificateAuthorityConfig caConfig = this.GetPrivateCertificateAuthorityConfigByHash(hash);

            ActiveDirectoryMetadata idp = this.Get<ActiveDirectoryMetadata>(caConfig.IdentityProviderId);

            MicrosoftCertificateAuthorityAuthenticationType authType;

            if (idp.ActiveDirectoryMetadataType == Entities.Enumerations.ActiveDirectoryMetadataType.ActiveDirectoryBasic)
                authType = MicrosoftCertificateAuthorityAuthenticationType.UsernamePassword;
            else
                authType = MicrosoftCertificateAuthorityAuthenticationType.WindowsKerberos;

            MicrosoftCertificateAuthorityOptions options = new MicrosoftCertificateAuthorityOptions()
            {
                AuthenticationRealm = idp.Domain,
                AuthenticationType = authType,
                HashAlgorithm = hash,
                CommonName = caConfig.CommonName,
                ServerName = caConfig.ServerName,
                Id = caConfig.Id,
                Password = idp.Password,
                Username = idp.Username
            };

            return options;

        }

        public MicrosoftCertificateAuthority GetPrivateCertificateAuthority(HashAlgorithm hash)
        {
            MicrosoftCertificateAuthorityOptions options = this.GetPrivateCertificateAuthorityOptions(hash);

            return new MicrosoftCertificateAuthority(options);
        }

        private PrivateCertificateAuthorityConfig GetPrivateCertificateAuthorityConfigByHash(HashAlgorithm hash)
        {
            LiteCollection<PrivateCertificateAuthorityConfig> col = db.GetCollection<PrivateCertificateAuthorityConfig>(privateCertificateAuthorityCollectionName);
            PrivateCertificateAuthorityConfig options = col.FindOne(Query.EQ("HashAlgorithm", hash.ToString()));

            if (options == null)
                throw new ConfigurationItemNotFoundException("could not find valid ca");
            else
                return options;

        }

        public AuthenticablePrincipal GetAuthenticablePrincipal(string upn)
        {
            LiteCollection<AuthenticablePrincipal> col = db.GetCollection<AuthenticablePrincipal>(authenticablePrincipalCollectionName);

            Query query = Query.Or(
                Query.In("AlternativeNames", upn),
                Query.EQ("Name", upn)
            );

            return col.FindOne(query);
        }


        public bool UserPrincipalNameExists(string upn, Guid ignoreUserId)
        {
            LiteCollection<AuthenticablePrincipal> col = db.GetCollection<AuthenticablePrincipal>(authenticablePrincipalCollectionName);

            Query query = Query.Or(
                    Query.In("AlternativeNames", upn),
                    Query.EQ("Name", upn)
                );

            IEnumerable<AuthenticablePrincipal> users = col.Find(query);

            if (!users.Any())
                return false;
            else
                return users.Where(user => user.Id != ignoreUserId).Any();
        }

        public bool UserPrincipalNameExists(string upn)
        {
            LiteCollection<AuthenticablePrincipal> col = db.GetCollection<AuthenticablePrincipal>(authenticablePrincipalCollectionName);


            Query query = Query.Or(
                Query.In("AlternativeNames", upn),
                Query.EQ("Name", upn)
            );

            return col.Exists(query);
        }

        public IEnumerable<SecurityRole> GetAuthenticablePrincipalMemberOf(Guid id)
        {
            LiteCollection<SecurityRole> col = db.GetCollection<SecurityRole>(securityRoleCollectionName);
            return col.FindAll().Where(role => role.Member.Contains(id));
        }


        private void ValidateCacheState()
        {
            //If the cache is expired, refresh
            if(cacheRefreshTime.AddMinutes(cachedConfig.CachePeriod) < DateTime.Now)
            {
                InitializeAppConfig();
            }
        }


        private void InitializeAppConfig()
        {
            LiteCollection<AppConfig> col = db.GetCollection<AppConfig>(appConfigCollectionName);

            cacheRefreshTime = DateTime.Now;

            IEnumerable<AppConfig> config = col.FindAll();

            if (config.Any())
            {
                this.cachedConfig = config.First();
            }
            else
            {
                this.cachedConfig = new AppConfig();
                col.Upsert(this.cachedConfig);
            }

            
        }

        public AppConfig GetAppConfig()
        {
            if(cachedConfig == null)
            {
                InitializeAppConfig();
            }

            ValidateCacheState();

            return cachedConfig;
        }

        public void SetAppConfig(AppConfig appConfig)
        {
            LiteCollection<AppConfig> col = db.GetCollection<AppConfig>(appConfigCollectionName);
            col.Upsert(appConfig);
            cachedConfig = appConfig;
            cacheRefreshTime = DateTime.Now;
        }

        public IEnumerable<Scope> GetAvailibleScopes()
        {
            LiteCollection<Scope> col = db.GetCollection<Scope>(scopesCollectionName);
            return col.FindAll();
        }

        public void InsertScopes(List<Scope> scopes)
        {
            LiteCollection<Scope> col = db.GetCollection<Scope>(scopesCollectionName);
            col.InsertBulk(scopes);
        }


    }
}
