using CertificateManager.Entities;
using CertificateManager.Entities.Enumerations;
using CertificateManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateManager.Logic
{
    public class ActiveDirectoryIdentityProviderLogic
    {
        IConfigurationRepository configurationRepository;

        public ActiveDirectoryIdentityProviderLogic(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }


        public ExternalIdentitySource Add(string displayName, string domain, string searchBase, string username, string password, bool useProcessContext)
        {
            ExternalIdentitySource idp = NewExternalIdentitySource(displayName, domain, searchBase, username, password, useProcessContext);

            configurationRepository.InsertExternalIdentitySource(idp);

            return idp;
        }

        private ExternalIdentitySource NewExternalIdentitySource(string displayName, string domain, string searchBase, string username, string password, bool useProcessContext)
        {
            ExternalIdentitySource idp = null;

            if (useProcessContext)
            {
                idp = new ExternalIdentitySource()
                {
                    Name = displayName,
                    Domain = domain,
                    Enabled = true,
                    ExternalIdentitySourceType = ExternalIdentitySourceType.ActiveDirectoryIwa,
                    Id = Guid.NewGuid(),
                    SearchBase = searchBase
                };
            }
            else
            {
                idp = new ExternalIdentitySource()
                {
                    Name = displayName,
                    Domain = domain,
                    Enabled = true,
                    ExternalIdentitySourceType = ExternalIdentitySourceType.ActiveDirectoryBasic,
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
