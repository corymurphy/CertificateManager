using CertificateManager.Entities.Attributes;
using CertificateServices;
using LiteDB;
using System;

namespace CertificateManager.Entities
{
    [Repository("Certificate")]
    public class SearchCertificatesEntity
    {
        public string DisplayName { get; set; }
        //public CertificateSubject Subject { get; set; }
        public string Thumbprint { get; set; }
        public Guid Id { get; set; }
        public HashAlgorithm HashAlgorithm { get; set; }
        public CipherAlgorithm CipherAlgorithm { get; set; }

        [BsonField("ValidTo")]
        public DateTime Expiry { get; set; }
    }
}
