using CertificateManager.Entities;
using CertificateServices;
using CertificateServices.Enumerations;
using System;
using System.Collections.Generic;

namespace CertificateManager.Repository
{
    public interface IConfigurationRepository
    {
        void Insert<T>(T item);
        AdcsTemplate GetAdcsTemplate(Guid id);
        AdcsTemplate GetAdcsTemplate(HashAlgorithm hash, CipherAlgorithm cipher, WindowsApi api, KeyUsage keyUsage);
        IEnumerable<AdcsTemplate> GetAdcsTemplates();
        void DeleteAdcsTemplates(Guid id);
        void UpdateAdcsTemplate(AdcsTemplate template);
        void InsertAdcsTemplate(AdcsTemplate template);
        MicrosoftCertificateAuthorityOptions GetPrivateCertificateAuthorityOptions(HashAlgorithm hash);
        MicrosoftCertificateAuthority GetPrivateCertificateAuthority(HashAlgorithm hash);


        PrivateCertificateAuthorityConfig GetPrivateCertificateAuthority(Guid id);
        IEnumerable<PrivateCertificateAuthorityConfig> GetPrivateCertificateAuthorities();
        void DeletePrivateCertificateAuthority(Guid id);
        void UpdatePrivateCertificateAuthority(PrivateCertificateAuthorityConfig ca);
        void DropPrivateCertificateAuthorityCollection();
        void InsertPrivateCertificateAuthorityConfig(PrivateCertificateAuthorityConfig ca);




        IEnumerable<ExternalIdentitySourceDomains> GetExternalIdentitySourceDomains();
        void UpdateExternalIdentitySource(ExternalIdentitySource entity);
        void InsertExternalIdentitySource(ExternalIdentitySource entity);
        void DeleteExternalIdentitySource(ExternalIdentitySource entity);
        ExternalIdentitySource GetExternalIdentitySource(Guid id);
        IEnumerable<ExternalIdentitySource> GetExternalIdentitySources();
        bool ExternalIdentitySourceExists(Guid id);




        IEnumerable<SearchAuthenticablePrincipalEntity> GetAuthenticablePrincipalsSearch();
        IEnumerable<T> GetAuthenticablePrincipals<T>();
        void UpdateAuthenticablePrincipal(AuthenticablePrincipal entity);
        void InsertAuthenticablePrincipal(AuthenticablePrincipal entity);
        void DeleteAuthenticablePrincipal(AuthenticablePrincipal entity);
        AuthenticablePrincipal GetAuthenticablePrincipal(string upn);
        T GetAuthenticablePrincipal<T>(Guid id);
        bool AuthenticablePrincipalExists(Guid id);
        bool UserPrincipalNameExists(string upn, Guid ignoreUserId);
        bool UserPrincipalNameExists(string upn);

        IEnumerable<SecurityRole> GetAuthenticablePrincipalMemberOf(Guid id);
        IEnumerable<SecurityRole> GetSecurityRoles();
        void UpdateSecurityRole(SecurityRole entity);
        void InsertSecurityRole(SecurityRole entity);
        void DeleteSecurityRole(SecurityRole entity);
        SecurityRole GetSecurityRole(Guid id);
        List<string> GetSecurityRoleNames();




        IEnumerable<AuthApiCertificate> GetAuthApiCertificates();
        void UpdateAuthApiCertificate(AuthApiCertificate entity);
        void DeleteAuthApiCertificate(AuthApiCertificate entity);
        void InsertAuthApiCertificate(AuthApiCertificate entity);
        AuthApiCertificate GetAuthApiCertificate(Guid id);



        IEnumerable<Scope> GetAvailibleScopes();
        void InsertScopes(List<Scope> scopes);

        AppConfig GetAppConfig();
        void SetAppConfig(AppConfig appConfig);

    }
}
