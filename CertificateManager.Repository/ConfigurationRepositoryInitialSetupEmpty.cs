using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CertificateManager.Entities;
using CertificateServices;
using CertificateServices.Enumerations;

namespace CertificateManager.Repository
{
    public class ConfigurationRepositoryInitialSetupEmpty : IConfigurationRepository
    {
        public bool AuthenticablePrincipalExists(Guid id)
        {
            throw new NotImplementedException();
        }

        public void DeleteAdcsTemplates(Guid id)
        {
            throw new NotImplementedException();
        }

        public void DeleteAuthApiCertificate(AuthApiCertificate entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteAuthenticablePrincipal(AuthenticablePrincipal entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteExternalIdentitySource(ExternalIdentitySource entity)
        {
            throw new NotImplementedException();
        }

        public void DeletePrivateCertificateAuthority(Guid id)
        {
            throw new NotImplementedException();
        }

        public void DeleteSecurityRole(SecurityRole entity)
        {
            throw new NotImplementedException();
        }

        public void DropPrivateCertificateAuthorityCollection()
        {
            throw new NotImplementedException();
        }

        public bool ExternalIdentitySourceExists(Guid id)
        {
            throw new NotImplementedException();
        }

        public AdcsTemplate GetAdcsTemplate(Guid id)
        {
            throw new NotImplementedException();
        }

        public AdcsTemplate GetAdcsTemplate(HashAlgorithm hash, CipherAlgorithm cipher, WindowsApi api, KeyUsage keyUsage)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AdcsTemplate> GetAdcsTemplates()
        {
            throw new NotImplementedException();
        }

        public AppConfig GetAppConfig()
        {
            throw new NotImplementedException();
        }

        public AuthApiCertificate GetAuthApiCertificate(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AuthApiCertificate> GetAuthApiCertificates()
        {
            throw new NotImplementedException();
        }

        public AuthenticablePrincipal GetAuthenticablePrincipal(string upn)
        {
            throw new NotImplementedException();
        }

        public AuthenticablePrincipal GetAuthenticablePrincipal(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SecurityRole> GetAuthenticablePrincipalMemberOf(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AuthenticablePrincipal> GetAuthenticablePrincipals()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SearchAuthenticablePrincipalEntity> GetAuthenticablePrincipalsSearch()
        {
            throw new NotImplementedException();
        }

        public ExternalIdentitySource GetExternalIdentitySource(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ExternalIdentitySourceDomains> GetExternalIdentitySourceDomains()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ExternalIdentitySource> GetExternalIdentitySources()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PrivateCertificateAuthorityConfig> GetPrivateCertificateAuthorities()
        {
            throw new NotImplementedException();
        }

        public MicrosoftCertificateAuthority GetPrivateCertificateAuthority(HashAlgorithm hash)
        {
            throw new NotImplementedException();
        }

        public PrivateCertificateAuthorityConfig GetPrivateCertificateAuthority(Guid id)
        {
            throw new NotImplementedException();
        }

        public MicrosoftCertificateAuthorityOptions GetPrivateCertificateAuthorityOptions(HashAlgorithm hash)
        {
            throw new NotImplementedException();
        }

        public SecurityRole GetSecurityRole(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<string> GetSecurityRoleNames()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SecurityRole> GetSecurityRoles()
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(T item)
        {
            throw new NotImplementedException();
        }

        public void InsertAdcsTemplate(AdcsTemplate template)
        {
            throw new NotImplementedException();
        }

        public void InsertAuthApiCertificate(AuthApiCertificate entity)
        {
            throw new NotImplementedException();
        }

        public void InsertAuthenticablePrincipal(AuthenticablePrincipal entity)
        {
            throw new NotImplementedException();
        }

        public void InsertExternalIdentitySource(ExternalIdentitySource entity)
        {
            throw new NotImplementedException();
        }

        public void InsertPrivateCertificateAuthorityConfig(PrivateCertificateAuthorityConfig ca)
        {
            throw new NotImplementedException();
        }

        public void InsertSecurityRole(SecurityRole entity)
        {
            throw new NotImplementedException();
        }

        public void SetAppConfig(AppConfig appConfig)
        {
            throw new NotImplementedException();
        }

        public void UpdateAdcsTemplate(AdcsTemplate template)
        {
            throw new NotImplementedException();
        }

        public void UpdateAuthApiCertificate(AuthApiCertificate entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateAuthenticablePrincipal(AuthenticablePrincipal entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateExternalIdentitySource(ExternalIdentitySource entity)
        {
            throw new NotImplementedException();
        }

        public void UpdatePrivateCertificateAuthority(PrivateCertificateAuthorityConfig ca)
        {
            throw new NotImplementedException();
        }

        public void UpdateSecurityRole(SecurityRole entity)
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
    }
}
