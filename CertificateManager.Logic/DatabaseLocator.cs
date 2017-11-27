using CertificateManager.Entities;
using System.IO;

namespace CertificateManager.Logic
{
    public class DatabaseLocator
    {
        private string auditDbName = "certificatemanager.audit.db";
        private string configDbName = "certificatemanager.config.db";
        private string certDbName = "certificatemanager.certs.db";
        private string dbRoot;

        public DatabaseLocator(AppSettings appSettings)
        {
            dbRoot = appSettings.DatastoreRootPath;
        }

        public DatabaseLocator(string dbRoot)
        {
            

            this.dbRoot = dbRoot;
        }

        public bool ConfigurationRepositoryExists()
        {
            if (string.IsNullOrWhiteSpace(dbRoot))
                return false;

            if (File.Exists(GetConfigurationRepositoryConnectionString()))
                return true;
            else
                return false;

        }

        public string GetConfigurationRepositoryConnectionString()
        {
            return Path.Combine(dbRoot, configDbName);
        }

        public string GetCertificateRepositoryConnectionString()
        {
            return Path.Combine(dbRoot, certDbName);
        }

        public string GetAuditRepositoryConnectionString()
        {
            return Path.Combine(dbRoot, auditDbName);
        }
    }
}
