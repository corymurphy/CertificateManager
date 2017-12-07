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
    public class ActiveDirectoryAuthenticatorTests
    {
        private string domain = "cm.local";
        private string domainDn = "DC=cm,DC=local";
        private string domainController = "srv14.cm.local";
        //private string caServerName = "srv14.cm.local";
        //private string caCommonName = "CertificateAuthority";
        private string username = "Administrator";
        //private string domain = "cm.local";
        private string password = "Password1@";
        private string invalidPassword = "derp";


        [TestMethod]
        public void ActiveDirectoryAuthenticator_Authenticate_ProvideValidCredentials_ReturnsTrue()
        {
            ActiveDirectoryAuthenticator activeDirectoryAuthenticator = new ActiveDirectoryAuthenticator();

            bool isAuthenticated = activeDirectoryAuthenticator.Authenticate(username, password, domain);

            Assert.IsTrue(isAuthenticated);
        }

        [TestMethod]
        public void ActiveDirectoryAuthenticator_Authenticate_ProvideInvalidCredentials_ReturnsFalse()
        {
            ActiveDirectoryAuthenticator activeDirectoryAuthenticator = new ActiveDirectoryAuthenticator();

            bool isAuthenticated = activeDirectoryAuthenticator.Authenticate(username, invalidPassword, domain);

            //Authenticate with validate credentials just to reset the bad password count
            activeDirectoryAuthenticator.Authenticate(username, password, domain);

            Assert.IsFalse(isAuthenticated);
        }

    }
}
