using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateManager.Logic;
using CertificateServices.ActiveDirectory.Entities;
using CertificateManager.Logic.ActiveDirectory;
using CertificateManager.Entities;
using CertificateServices;

namespace CertificateManager.IntegrationTests
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
        public void AdcsTemplateLogic_GetActiveDirectoryPublishedTemplate_ValidNameAndCipherRsa_ReturnsObjectWithExpectedCipherAlgorithm()
        {
            string name = "ServerAuthentication-CngRsa";
            CipherAlgorithm expectedCipher = CipherAlgorithm.RSA;

            AdcsTemplateLogic templateLogic = new AdcsTemplateLogic(null, activeDirectory);

            AdcsCertificateTemplate template = templateLogic.GetActiveDirectoryPublishedTemplate(name, metadata);

            Assert.AreEqual(expectedCipher, template.Cipher);
        }

        [TestMethod]
        public void AdcsTemplateLogic_GetActiveDirectoryPublishedTemplate_ValidNameAndCipherEcdh_ReturnsObjectWithExpectedCipherAlgorithm()
        {
            string name = "ServerAuthentication-CngEcdh";
            CipherAlgorithm expectedCipher = CipherAlgorithm.ECDH;

            AdcsTemplateLogic templateLogic = new AdcsTemplateLogic(null, activeDirectory);

            AdcsCertificateTemplate template = templateLogic.GetActiveDirectoryPublishedTemplate(name, metadata);

            Assert.AreEqual(expectedCipher, template.Cipher);
        }

        [TestMethod]
        public void AdcsTemplateLogic_GetActiveDirectoryPublishedTemplate_ValidNameAndCipherEdsa_ReturnsObjectWithExpectedCipherAlgorithm()
        {
            string name = "ServerAuthentication-CngEcdsa";
            CipherAlgorithm expectedCipher = CipherAlgorithm.ECDSA;

            AdcsTemplateLogic templateLogic = new AdcsTemplateLogic(null, activeDirectory);

            AdcsCertificateTemplate template = templateLogic.GetActiveDirectoryPublishedTemplate(name, metadata);

            Assert.AreEqual(expectedCipher, template.Cipher);
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
