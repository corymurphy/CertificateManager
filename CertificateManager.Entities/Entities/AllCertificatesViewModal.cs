using CertificateManager.Entities.Attributes;
using CertificateServices;
using System;

namespace CertificateManager.Entities
{
    [Repository("Certificate")]
    public class AllCertificatesViewModel
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Thumbprint { get; set; }
        public DateTime ValidTo { get; set; }
        public HashAlgorithm HashAlgorithm { get; set; }
        public CipherAlgorithm CipherAlgorithm { get; set; }
    }
}
