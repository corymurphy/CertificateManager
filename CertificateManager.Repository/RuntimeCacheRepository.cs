using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CertificateManager.Repository
{
    public class RuntimeCacheRepository : IRuntimeCacheRepository
    {
        string connectionString;

        
        private const string configurationAlertsCollectionName = "ConfigAlerts";
        public RuntimeCacheRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool ConfigurationAlertExists(AlertType type)
        {
            using (LiteDatabase db = new LiteDatabase(connectionString))
            {
                LiteCollection<ConfigurationAlert> col = db.GetCollection<ConfigurationAlert>(configurationAlertsCollectionName);
                return col.Exists(Query.EQ("AlertType", type.ToString()));
            }
        }

        public IEnumerable<ConfigurationAlert> GetConfigurationAlerts()
        {
            using (LiteDatabase db = new LiteDatabase(connectionString))
            {
                LiteCollection<ConfigurationAlert> col = db.GetCollection<ConfigurationAlert>(configurationAlertsCollectionName);
                return col.FindAll();
            }
        }

        public void InsertConfigurationAlert(ConfigurationAlert alert)
        {
            using (LiteDatabase db = new LiteDatabase(connectionString))
            {
                LiteCollection<ConfigurationAlert> col = db.GetCollection<ConfigurationAlert>(configurationAlertsCollectionName);
                col.Insert(alert);
            }
        }

        public void UpdateConfigurationAlert(ConfigurationAlert alert)
        {
            using (LiteDatabase db = new LiteDatabase(connectionString))
            {
                LiteCollection<ConfigurationAlert> col = db.GetCollection<ConfigurationAlert>(configurationAlertsCollectionName);
                col.Update(alert);
            }
        }

        public void DeleteConfigurationAlert(ConfigurationAlert alert)
        {
            using (LiteDatabase db = new LiteDatabase(connectionString))
            {
                LiteCollection<ConfigurationAlert> col = db.GetCollection<ConfigurationAlert>(configurationAlertsCollectionName);
                col.Delete(alert.Id);
            }
        }

        public IEnumerable<ConfigurationAlert> GetConfigurationAlerts(AlertType type)
        {
            using (LiteDatabase db = new LiteDatabase(connectionString))
            {
                LiteCollection<ConfigurationAlert> col = db.GetCollection<ConfigurationAlert>(configurationAlertsCollectionName);
                return col.Find(Query.EQ("AlertType", type.ToString()));
            }
        }
    }
}
