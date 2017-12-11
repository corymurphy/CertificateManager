using CertificateManager.Logic.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            ActiveDirectoryRepository activeDirectoryAuthenticator = new ActiveDirectoryRepository();

            bool isAuthenticated = activeDirectoryAuthenticator.Authenticate(username, password, domain);

            Assert.IsTrue(isAuthenticated);
        }

        [TestMethod]
        public void ActiveDirectoryAuthenticator_Authenticate_ProvideInvalidCredentials_ReturnsFalse()
        {
            ActiveDirectoryRepository activeDirectoryAuthenticator = new ActiveDirectoryRepository();

            bool isAuthenticated = activeDirectoryAuthenticator.Authenticate(username, invalidPassword, domain);

            //Authenticate with validate credentials just to reset the bad password count
            activeDirectoryAuthenticator.Authenticate(username, password, domain);

            Assert.IsFalse(isAuthenticated);
        }

    }
}
