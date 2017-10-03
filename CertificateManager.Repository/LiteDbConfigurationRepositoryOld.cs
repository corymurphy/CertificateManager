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
    public class LiteDbConfigurationRepositoryOld
    {
        private const string securityRoleCollectionName = "secroles";
        private const string authenticablePrincipalCollectionName = "usr";
        private const string adcsNotFoundExceptionMessage = "Could not find adcs template that matched the query {0} and {1}";
        private const string adcsTemplateCollectionName = "Template";
        private const string privateCertificateAuthorityCollectionName = "PrivateCa";
        private const string AdcsTemplateCollectionName = "AdcsTemplate";
        private const string externalIdentitySourceCollectionName = "extid";
        private string path;

        public LiteDbConfigurationRepositoryOld(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            this.path = path;
        }

        public void Insert<T>(T item)
        {
            if(item.GetType() == typeof(MicrosoftCertificateAuthorityOptions))
            {
                InsertNamedCollection(item, privateCertificateAuthorityCollectionName);
            }
            else
            {
                InsertAssumedCollection(item);
            }
        }

        private void InsertNamedCollection<T>(T item, string collectionName)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<T> col = db.GetCollection<T>(collectionName);
                col.Insert(item);
            }
        }

        private void InsertAssumedCollection<T>(T item)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<T> col = db.GetCollection<T>();
                col.Insert(item);
            }
        }



        public AdcsTemplate GetAdcsTemplate(Guid id)
        {
            return null;
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

                            Query.And(
                                Query.EQ("Hash", hash.ToString()),
                                Query.EQ("Cipher", cipher.ToString())
                            ),

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

        public IEnumerable<AdcsTemplate> GetAdcsTemplates()
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<AdcsTemplate> col = db.GetCollection<AdcsTemplate>();
                return col.FindAll();
            }
        }

        public void DeleteAdcsTemplates(Guid id)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<AdcsTemplate> col = db.GetCollection<AdcsTemplate>();
                col.Delete(id);
            }
        }

        public void UpdateAdcsTemplate(AdcsTemplate template)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<AdcsTemplate> col = db.GetCollection<AdcsTemplate>();
                col.Update(template);
            }
        }

        public void InsertAdcsTemplate(AdcsTemplate template)
        {
            //template.Id = Guid.NewGuid();
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<AdcsTemplate> col = db.GetCollection<AdcsTemplate>();
                col.Insert(template);
            }
        }





        public MicrosoftCertificateAuthorityOptions GetPrivateCertificateAuthorityOptions(HashAlgorithm hash)
        {
            MicrosoftCertificateAuthorityOptions options;

            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<MicrosoftCertificateAuthorityOptions> col = db.GetCollection<MicrosoftCertificateAuthorityOptions>(privateCertificateAuthorityCollectionName);

                options = col.FindOne(Query.EQ("HashAlgorithm", hash.ToString()));

                if (options == null)
                    throw new ConfigurationItemNotFoundException("could not find valid ca");
                else
                    return options;
            }
        }

        public MicrosoftCertificateAuthority GetPrivateCertificateAuthority(HashAlgorithm hash)
        {
            MicrosoftCertificateAuthorityOptions options = this.GetPrivateCertificateAuthorityOptions(hash);

            return new MicrosoftCertificateAuthority(options);
        }
        public MicrosoftCertificateAuthorityOptions GetPrivateCertificateAuthority(Guid id)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<MicrosoftCertificateAuthorityOptions> col =
                    db.GetCollection<MicrosoftCertificateAuthorityOptions>(privateCertificateAuthorityCollectionName);
                return col.FindById(id);
            }
        }

        public IEnumerable<MicrosoftCertificateAuthorityOptions> GetPrivateCertificateAuthorities()
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<MicrosoftCertificateAuthorityOptions> col =
                    db.GetCollection<MicrosoftCertificateAuthorityOptions>(privateCertificateAuthorityCollectionName);
                return col.FindAll();
            }
        }

        public void DeletePrivateCertificateAuthority(Guid id)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<MicrosoftCertificateAuthorityOptions> col = 
                    db.GetCollection<MicrosoftCertificateAuthorityOptions>(privateCertificateAuthorityCollectionName);
                col.Delete(id);
            }
        }

        public void UpdatePrivateCertificateAuthority(MicrosoftCertificateAuthorityOptions ca)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<MicrosoftCertificateAuthorityOptions> col =
                    db.GetCollection<MicrosoftCertificateAuthorityOptions>(privateCertificateAuthorityCollectionName);
                col.Update(ca);
            }
        }
        





        public void InsertExternalIdentitySource(ExternalIdentitySource entity)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<ExternalIdentitySource> col = db.GetCollection<ExternalIdentitySource>(externalIdentitySourceCollectionName);
                col.Insert(entity);
            }
        }

        public void DeleteExternalIdentitySource(ExternalIdentitySource entity)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<ExternalIdentitySource> col = db.GetCollection<ExternalIdentitySource>(externalIdentitySourceCollectionName);
                col.Delete(entity.Id);
            }
        }

        public ExternalIdentitySource GetExternalIdentitySource(Guid id)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<ExternalIdentitySource> col = db.GetCollection<ExternalIdentitySource>(externalIdentitySourceCollectionName);
                return col.FindById(id);
            }
        }

        public IEnumerable<ExternalIdentitySource> GetExternalIdentitySources()
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<ExternalIdentitySource> col = db.GetCollection<ExternalIdentitySource>(externalIdentitySourceCollectionName);
                return col.FindAll();
            }
        }

        public void UpdateExternalIdentitySource(ExternalIdentitySource entity)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<ExternalIdentitySource> col = db.GetCollection<ExternalIdentitySource>(externalIdentitySourceCollectionName);
                col.Update(entity);
            }
        }

        public IEnumerable<ExternalIdentitySourceDomains> GetExternalIdentitySourceDomains()
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<ExternalIdentitySourceDomains> col = db.GetCollection<ExternalIdentitySourceDomains>(externalIdentitySourceCollectionName);
                return col.FindAll();
            }
        }







        public IEnumerable<AuthenticablePrincipal> GetAuthenticablePrincipals()
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<AuthenticablePrincipal> col = db.GetCollection<AuthenticablePrincipal>(authenticablePrincipalCollectionName);
                return col.FindAll();
            }
        }

        public void UpdateAuthenticablePrincipal(AuthenticablePrincipal entity)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<AuthenticablePrincipal> col = db.GetCollection<AuthenticablePrincipal>(authenticablePrincipalCollectionName);
                col.Update(entity);
            }
        }

        public void InsertAuthenticablePrincipal(AuthenticablePrincipal entity)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<AuthenticablePrincipal> col = db.GetCollection<AuthenticablePrincipal>(authenticablePrincipalCollectionName);
                col.Insert(entity);
            }
        }

        public void DeleteAuthenticablePrincipal(AuthenticablePrincipal entity)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<AuthenticablePrincipal> col = db.GetCollection<AuthenticablePrincipal>(authenticablePrincipalCollectionName);
                col.Delete(entity.Id);
            }
        }

        public AuthenticablePrincipal GetAuthenticablePrincipal(Guid id)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<AuthenticablePrincipal> col = db.GetCollection<AuthenticablePrincipal>(authenticablePrincipalCollectionName);
                return col.FindById(id);
            }
        }












        public IEnumerable<SecurityRole> GetSecurityRoles()
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<SecurityRole> col = db.GetCollection<SecurityRole>(securityRoleCollectionName);
                return col.FindAll();
            }
        }

        public void UpdateSecurityRole(SecurityRole entity)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<SecurityRole> col = db.GetCollection<SecurityRole>(securityRoleCollectionName);
                col.Update(entity);
            }
        }

        public void InsertSecurityRole(SecurityRole entity)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<SecurityRole> col = db.GetCollection<SecurityRole>(securityRoleCollectionName);
                col.Insert(entity);
            }
        }

        public void DeleteSecurityRole(SecurityRole entity)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<SecurityRole> col = db.GetCollection<SecurityRole>(securityRoleCollectionName);
                col.Delete(entity.Id);
            }
        }

        public SecurityRole GetSecurityRole(Guid id)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<SecurityRole> col = db.GetCollection<SecurityRole>(securityRoleCollectionName);
                return col.FindById(id);
            }
        }

        public List<string> GetSecurityRoleNames()
        {
            return this.GetSecurityRoles().Select(x => x.Name).ToList();
        }

        public IEnumerable<SecurityRole> GetAuthenticablePrincipalMemberOf(Guid id)
        {
            using (LiteDatabase db = new LiteDatabase(path))
            {
                LiteCollection<SecurityRole> col = db.GetCollection<SecurityRole>(securityRoleCollectionName);
                return col.Find(Query.In("Member", id));
            }
        }

    }
}
