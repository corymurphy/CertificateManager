using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateManager.Logic.ActiveDirectory;
using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateServices.ActiveDirectory.Entities;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class AdcsTemplateLogicTests
    {
        private string domain = "cm.local";
        private string domainDn = "DC=cm,DC=local";
        private string domainController = "srv14.cm.local";
        private string username = "Administrator";
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
        public void AdcsTemplateLogic_GetActiveDirectoryPublishedTemplate_ValidName_ReturnsObjectWithMatchingName()
        {
            string name = "ServerAuthentication-CngRsa";

            AdcsTemplateLogic templateLogic = new AdcsTemplateLogic(null, activeDirectory);

            AdcsCertificateTemplate template = templateLogic.GetActiveDirectoryPublishedTemplate(name, metadata);

            Assert.AreEqual(name, template.Name);
        }
    }
}
