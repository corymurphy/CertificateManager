using CertificateManager.Entities.Enumerations;
using System;

namespace CertificateManager.Entities
{
    public class ManagedCertificate
    {
        public Guid Id { get; set; }
        public ManagedCertificateType ManagedCertificateType { get; set; }
        public DateTime DiscoveryDate { get; set; }
        public DateTime LastRenewal { get; set; }
        public DateTime MustRenewBy { get; set; }
        public string Thumbprint { get; set; }
        public Guid CertificateManagerId { get; set; }
        public string X509Content { get; set; }

    }
}
