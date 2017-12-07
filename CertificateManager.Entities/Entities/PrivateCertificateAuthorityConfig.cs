using CertificateManager.Entities.Attributes;
using CertificateServices;
using System;

namespace CertificateManager.Entities
{
    [Repository("PrivateCa")]
    public class PrivateCertificateAuthorityConfig
    {
        public Guid Id { get; set; }
        public string ServerName { get; set; }
        public string CommonName { get; set; }
        public HashAlgorithm HashAlgorithm { get; set; }
        public Guid IdentityProviderId { get; set; }
    }
}
