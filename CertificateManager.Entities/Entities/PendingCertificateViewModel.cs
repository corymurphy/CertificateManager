using CertificateManager.Entities.Attributes;
using CertificateManager.Entities.Enumerations;
using CertificateServices;
using System;

namespace CertificateManager.Entities
{
    [Repository("PendingCerts")]
    public class PendingCertificateViewModel
    {

        public Guid Id { get; set; }
        public Guid RequestingUserId { get; set; }
        public DateTime RequestDate { get; set; }
        public string SubjectCommonName { get; set; }
        public CipherAlgorithm CipherAlgorithm { get; set; }
        public HashAlgorithm HashAlgorithm { get; set; }
        public string KeyUsage { get; set; }

    }
}
