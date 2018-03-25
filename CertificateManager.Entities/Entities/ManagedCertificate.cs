using CertificateManager.Entities.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
