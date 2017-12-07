using CertificateManager.Entities;
using CertificateManager.Repository;
using CertificateServices;
using System;
using System.Collections.Generic;

namespace CertificateManager.Logic
{
    public class CertificateAuthorityConfigurationLogic
    {
        IConfigurationRepository configurationRepository;

        public CertificateAuthorityConfigurationLogic(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        public IEnumerable<PrivateCertificateAuthorityConfig> GetPrivateCertificateAuthorities()
        {
            return configurationRepository.GetAll<PrivateCertificateAuthorityConfig>();
        }

        public void DeletePrivateCertificateAuthority(Guid id)
        {
            configurationRepository.Delete<PrivateCertificateAuthorityConfig>(id);
        }

        public void UpdatePrivateCertificateAuthority(PrivateCertificateAuthorityConfig ca)
        {
            PrivateCertificateAuthorityConfig existingCa = configurationRepository.Get<PrivateCertificateAuthorityConfig>(ca.Id);

            ca.Id = existingCa.Id;

            configurationRepository.Update<PrivateCertificateAuthorityConfig>(ca);
        }

        public void AddPrivateCertificateAuthority(PrivateCertificateAuthorityConfig ca)
        {
            ca.Id = Guid.NewGuid();
            configurationRepository.Insert<PrivateCertificateAuthorityConfig>(ca);
        }

        public void AddPrivateCertificateAuthority(string serverName, string commonName, HashAlgorithm hash, Guid activeDirectoryIdentityProviderId)
        {
            if (string.IsNullOrWhiteSpace(serverName))
                throw new ArgumentNullException(nameof(serverName));

            if (string.IsNullOrWhiteSpace(commonName))
                throw new ArgumentNullException(nameof(commonName));

            PrivateCertificateAuthorityConfig ca = new PrivateCertificateAuthorityConfig()
            {
                Id = Guid.NewGuid(),
                CommonName = commonName,
                HashAlgorithm = hash,
                ServerName = serverName, 
                IdentityProviderId = activeDirectoryIdentityProviderId
            };

            configurationRepository.Insert<PrivateCertificateAuthorityConfig>(ca);
        }
    }
}
