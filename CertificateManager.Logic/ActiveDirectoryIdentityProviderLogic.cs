using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using CertificateManager.Repository;
using System;

namespace CertificateManager.Logic
{
    public class ActiveDirectoryIdentityProviderLogic
    {
        IConfigurationRepository configurationRepository;

        public ActiveDirectoryIdentityProviderLogic(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }


        public ActiveDirectoryMetadata Add(string displayName, string domain, string searchBase, string username, string password, bool useProcessContext)
        {
            ActiveDirectoryMetadata idp = NewActiveDirectoryMetadata(displayName, domain, searchBase, username, password, useProcessContext);

            configurationRepository.Insert<ActiveDirectoryMetadata>(idp);

            return idp;
        }

        private ActiveDirectoryMetadata NewActiveDirectoryMetadata(string displayName, string domain, string searchBase, string username, string password, bool useProcessContext)
        {
            ActiveDirectoryMetadata idp = null;

            if (useProcessContext)
            {
                idp = new ActiveDirectoryMetadata()
                {
                    Name = displayName,
                    Domain = domain,
                    Enabled = true,
                    ActiveDirectoryMetadataType = ActiveDirectoryMetadataType.ActiveDirectoryIwa,
                    Id = Guid.NewGuid(),
                    SearchBase = searchBase
                };
            }
            else
            {
                idp = new ActiveDirectoryMetadata()
                {
                    Name = displayName,
                    Domain = domain,
                    Enabled = true,
                    ActiveDirectoryMetadataType = ActiveDirectoryMetadataType.ActiveDirectoryBasic,
                    Id = Guid.NewGuid(),
                    SearchBase = searchBase,
                    Username = username,
                    Password = password
                };
            }

            return idp;
        }
    }
}
