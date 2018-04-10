using CertificateManager.Entities;
using CertificateManager.Logic.ActiveDirectory;
using CertificateServices.ActiveDirectory.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CertificateManager.IntegrationTests
{
    [TestClass]
    public class ActiveDirectoryRepositoryTests
    {
        private string domain = "certmgr.local";
        private string domainDn = "DC=certmgr,DC=local";
        private string domainController = "dc01.certmgr.local";
        //private string caServerName = "srv14.cm.local";
        //private string caCommonName = "CertificateAuthority";
        private string username = "Administrator";
        //private string domain = "cm.local";
        private string password = "Password1@";

        private ActiveDirectoryRepository activeDirectory;
        private ActiveDirectoryMetadata metadata;

        [TestInitialize]
        public void InitializeTest()
        {
            activeDirectory = new ActiveDirectoryRepository();
            metadata = new ActiveDirectoryMetadata(domain, username, password);
        }

        [TestMethod]
        public void ActiveDirectoryRepository_Search_ActiveDirectoryAuthenticablePrincipal_ReturnsResults_Success()
        {
            List<ActiveDirectoryAuthenticablePrincipal> results = activeDirectory.Search<ActiveDirectoryAuthenticablePrincipal>(NamingContext.Default, metadata);

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void ActiveDirectoryRepository_Search_SearchForAdministrator_ReturnsAdminAccount()
        {
            List<ActiveDirectoryAuthenticablePrincipal> results = activeDirectory.Search<ActiveDirectoryAuthenticablePrincipal>("anr", username, NamingContext.Default, metadata);

            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void ActiveDirectoryRepository_Search_SearchForAdministrator_ReturnAdminAccount_CheckName()
        {
            List<ActiveDirectoryAuthenticablePrincipal> results = activeDirectory.Search<ActiveDirectoryAuthenticablePrincipal>("anr", username, NamingContext.Default, metadata);

            ActiveDirectoryAuthenticablePrincipal adminAccount = results.FirstOrDefault();

            Assert.AreEqual(username, adminAccount.SamAccountName);

        }

        [TestMethod]
        public void ActiveDirectoryRepository_Search_FindAllAdcsTemplates_DoesNotReturnNull()
        {
            List<AdcsCertificateTemplate> templates = activeDirectory.Search<AdcsCertificateTemplate>(NamingContext.Configuration, metadata);

            Assert.IsNotNull(templates);
        }
    }
}
