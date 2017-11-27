using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CertificateManager.Entities;
using CertificateManager.Logic;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class DatabaseLocatorTests
    {
        private string configPathExpected = @"d:\db\certificatemanager.config.db";
        private string auditPathExpected = @"d:\db\certificatemanager.audit.db";
        private string certPathExpected = @"d:\db\certificatemanager.certs.db";
        private string datastoreRootPath = @"d:\db";

        [TestMethod]
        public void DatabaseLocator_GetConfigurationRepositoryConnectionString_ReturnsExpectedResult()
        {
            AppSettings appSettings = GetAppSettings();

            DatabaseLocator databaseLocator = new DatabaseLocator(appSettings);

            string configPathActual = databaseLocator.GetConfigurationRepositoryConnectionString();

            Assert.AreEqual(configPathExpected, configPathActual);

        }

        [TestMethod]
        public void DatabaseLocator_GetCertificateRepositoryConnectionString_ReturnsExpectedResult()
        {
            AppSettings appSettings = GetAppSettings();

            DatabaseLocator databaseLocator = new DatabaseLocator(appSettings);

            string certPathActual = databaseLocator.GetCertificateRepositoryConnectionString();

            Assert.AreEqual(certPathExpected, certPathActual);

        }


        [TestMethod]
        public void DatabaseLocator_GetAuditRepositoryConnectionString_ReturnsExpectedResult()
        {
            AppSettings appSettings = GetAppSettings();

            DatabaseLocator databaseLocator = new DatabaseLocator(appSettings);

            string auditPathActual = databaseLocator.GetAuditRepositoryConnectionString();

            Assert.AreEqual(auditPathExpected, auditPathActual);

        }

        private AppSettings GetAppSettings()
        {
            return new AppSettings()
            {
                DatastoreRootPath = datastoreRootPath
            };
        }
    }
}
