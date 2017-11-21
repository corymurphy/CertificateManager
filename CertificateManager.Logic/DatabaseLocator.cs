using CertificateManager.Entities;
using System.IO;

namespace CertificateManager.Logic
{
    public class DatabaseLocator
    {

        private string configDbName = "certificatemanager.config.db";
        private string certDbName = "certificatemanager.config.db";
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

            if (Directory.Exists(GetConfigurationRepositoryConnectionString()))
                return true;
            else
                return false;

        }

        public string GetConfigurationRepositoryConnectionString()
        {
            return Path.Combine(dbRoot, certDbName);
        }

        
    }
}
