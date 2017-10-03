using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateServices.ActiveDirectory;
using CertificateServices.ActiveDirectory.Entities;
using CertificateManager.Repository;
using CertificateManager.Entities;
using System.Collections.Generic;

namespace CertificateManager.IntegrationTests
{
    [TestClass]
    public class ActiveDirectoryRepositoryTests
    {
        private string domainDn = "DC=cm,DC=local";
        private string domainController = "srv14.cm.local";
        //private string caServerName = "srv14.cm.local";
        //private string caCommonName = "CertificateAuthority";
        private string username = "Administrator";
        //private string domain = "cm.local";
        private string password = "Password1@";


        [TestMethod]
        public void ActiveDirectoryRepositoryTests_Search_ActiveDirectoryAuthenticablePrincipal_ReturnsResults_Success()
        {
            LiteDbConfigurationRepository configurationRepository = new LiteDbConfigurationRepository(@"D:\db\config.db");

            IEnumerable<ExternalIdentitySource> sources = configurationRepository.GetExternalIdentitySources();
            ExternalIdentitySource source = new ExternalIdentitySource();
            foreach (var a in sources)
            {
                if(a.Name == "cm.local")
                    source = a;
            }
            ActiveDirectoryRepository activeDirectory = new ActiveDirectoryRepository(source.Domain, source.Domain, source.Username, source.Password);

            var results = activeDirectory.Search<ActiveDirectoryAuthenticablePrincipal>(NamingContext.Default);

            Console.Write("a");
        }


        [TestMethod]
        public void ActiveDirectoryRepositoryTests_Search_PkiCertificateTemplate_ReturnsResults_Success()
        {
            ActiveDirectoryRepository ad = new ActiveDirectoryRepository(domainDn, domainController, username, password);
            var results = ad.Search<AdcsCertificateTemplate>(NamingContext.Configuration);

            Assert.IsNotNull(results);

        }

        [TestMethod]
        public void ActiveDirectoryRepositoryTests_SearchWithFilter_PkiCertificateTemplate_ReturnsResults_Success()
        {
            //string schemaClass = ActiveDirectorySchemaClass.PkiCertificateTemplate;
            //string searchKey = "name";
            //string searchValue = "WebServer";
            //ActiveDirectoryRepository ad = new ActiveDirectoryRepository(domainDn, domainController, username, password);
            //var results = ad.Search<PkiCertificateTemplate>(searchKey, searchValue, NamingContext.Configuration);

            //Assert.IsNotNull(results);

        }
    }
}
