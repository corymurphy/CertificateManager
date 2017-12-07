using CertificateManager.Entities;
using System;
using LiteDB;
using CertificateServices;
using CertificateManager.Entities.Exceptions;
using System.Collections.Generic;
using CertificateServices.Enumerations;
using System.Linq;

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
        private const string externalIdentitySourceCollectionName = "extid";
        private const string authApiCertificateCollectionName = "authcer";
        private const string appConfigCollectionName = "appcfg";
        private const string scopesCollectionName = "scopes";
        private string path;

        private LiteDatabase db;
        private CollectionDiscoveryLogic collectionDiscoveryLogic;

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

        public bool Exists<T>(Guid id)
        {
            LiteCollection<T> col = db.GetCollection<T>(collectionDiscoveryLogic.GetName<T>());
            return col.Exists(Query.EQ("Id", id));
        }





        public AdcsTemplate GetAdcsTemplate(HashAlgorithm hash, CipherAlgorithm cipher, WindowsApi api, KeyUsage keyUsage)
        {
            AdcsTemplate template;

            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<AdcsTemplate> col = db.GetCollection<AdcsTemplate>();
                {
                    template = col.FindOne(
                        Query.And(

                            Query.EQ("Cipher", cipher.ToString()),

                            Query.And(
                                Query.EQ("WindowsApi", api.ToString()),
                                Query.EQ("KeyUsage", keyUsage.ToString())
                            )

                        ));

                    if(template == null)
                    {
                        throw new ConfigurationItemNotFoundException(string.Format(adcsNotFoundExceptionMessage, hash, cipher));
                    }
                    else
                    {
                        return template;
                    }
                }
            }
        }

        public MicrosoftCertificateAuthorityOptions GetPrivateCertificateAuthorityOptions(HashAlgorithm hash)
        {

            PrivateCertificateAuthorityConfig caConfig = this.GetPrivateCertificateAuthorityConfigByHash(hash);

            ExternalIdentitySource idp = this.Get<ExternalIdentitySource>(caConfig.IdentityProviderId);

            MicrosoftCertificateAuthorityAuthenticationType authType;

            if (idp.ExternalIdentitySourceType == Entities.Enumerations.ExternalIdentitySourceType.ActiveDirectoryBasic)
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





        private AppConfig InitializeAppConfig(LiteCollection<AppConfig> col)
        {
            AppConfig appConfig = new AppConfig();
            col.Insert(appConfig);
            return appConfig;
        }

        public AppConfig GetAppConfig()
        {
            
            LiteCollection<AppConfig> col = db.GetCollection<AppConfig>(appConfigCollectionName);

            AppConfig appConfig = col.FindOne(Query.All());

            if (appConfig != null)
                return appConfig;
            else
                return InitializeAppConfig(col);
        }

        public void SetAppConfig(AppConfig appConfig)
        {
            LiteCollection<AppConfig> col = db.GetCollection<AppConfig>(appConfigCollectionName);
            col.Upsert(appConfig);
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
