using CertificateManager.Entities;
using CertificateManager.Repository;
using CertificateServices;
using CertificateServices.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateManager.Logic
{
    public class WorkstationDevelopment : ICertificateManagerHostingEnvironment
    {
        private string caServerName = "srv14.cm.local";
        private string caCommonName = "CertificateAuthority";
        private string username = "Administrator";
        private string domain = "cm.local";
        private string password = "Password1@";
        private string configPath = string.Empty;
        private LiteDbConfigurationRepository configDb;
        public WorkstationDevelopment(string configPath)
        {
            configDb = new LiteDbConfigurationRepository(configPath);
        }

        public void Setup()
        {
            AdcsTemplate result = null;
            try
            {
                result = configDb.Get<AdcsTemplate>(x => x.Cipher == CipherAlgorithm.RSA && x.WindowsApi == WindowsApi.CryptoApi && x.KeyUsage == KeyUsage.ServerAuthentication).First();
                //result = configDb.GetAdcsTemplate(HashAlgorithm.SHA256, CipherAlgorithm.RSA, WindowsApi.CryptoApi, KeyUsage.ServerAuthentication);
            }
            catch
            {
                if (result == null)
                {
                    configDb.Insert<AdcsTemplate>(new AdcsTemplate()
                    {
                        WindowsApi = WindowsApi.CryptoApi,
                        Cipher = CipherAlgorithm.RSA,
                        //Hash = HashAlgorithm.SHA256,
                        KeyUsage = KeyUsage.ServerAuthentication,
                        Name = "ServerAuthentication-CapiRsa"
                    });
                }
            }
            ActiveDirectoryMetadata idp = configDb.GetAll<ActiveDirectoryMetadata>().FirstOrDefault();

            configDb.DropCollection<PrivateCertificateAuthorityConfig>();

            PrivateCertificateAuthorityConfig caConfig = new PrivateCertificateAuthorityConfig()
            {
                CommonName = caCommonName,
                ServerName = caServerName,
                HashAlgorithm = HashAlgorithm.SHA256,
                IdentityProviderId = idp.Id
            };
        }
    }
}
