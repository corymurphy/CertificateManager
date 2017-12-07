using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateServices.ActiveDirectory;
using CertificateServices.ActiveDirectory.Entities;
using CertificateManager.Repository;
using CertificateManager.Entities;
using System.Collections.Generic;
using System.Linq;
namespace CertificateManager.IntegrationTests
{
    [TestClass]
    public class ActiveDirectoryRepositoryTests
    {
        private string domain = "cm.local";
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
            ActiveDirectoryRepository activeDirectory = new ActiveDirectoryRepository(domain, domain, username, password);

            List<ActiveDirectoryAuthenticablePrincipal> results = activeDirectory.Search<ActiveDirectoryAuthenticablePrincipal>(NamingContext.Default);

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void ActiveDirectoryRepositoryTests_Search_SearchForAdministrator_ReturnsAdminAccount()
        {
            ActiveDirectoryRepository activeDirectory = new ActiveDirectoryRepository(domain, domain, username, password);

            List<ActiveDirectoryAuthenticablePrincipal> results = activeDirectory.Search<ActiveDirectoryAuthenticablePrincipal>("anr", username, NamingContext.Default);

            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public void ActiveDirectoryRepositoryTests_Search_SearchForAdministrator_ReturnAdminAccount_CheckName()
        {
            ActiveDirectoryRepository activeDirectory = new ActiveDirectoryRepository(domain, domain, username, password);

            List<ActiveDirectoryAuthenticablePrincipal> results = activeDirectory.Search<ActiveDirectoryAuthenticablePrincipal>("anr", username, NamingContext.Default);

            ActiveDirectoryAuthenticablePrincipal adminAccount = results.FirstOrDefault();

            Assert.AreEqual(username, adminAccount.SamAccountName);

        }
    }
}
