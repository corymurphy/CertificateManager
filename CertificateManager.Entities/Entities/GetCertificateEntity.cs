using System;
using CertificateServices.Enumerations;
using CertificateServices;
using System.Collections.Generic;
using CertificateManager.Entities.Attributes;

namespace CertificateManager.Entities
{
    [Repository("Certificate")]
    public class GetCertificateEntity
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Thumbprint { get; set; }
        public HashAlgorithm HashAlgorithm { get; set; }
        public CipherAlgorithm CipherAlgorithm { get; set; }
        public WindowsApi WindowsApi { get; set; }
        public KeyUsage KeyUsage { get; set; }
        public int KeySize { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime ValidFrom { get; set; }
        public CertificateStorageFormat CertificateStorageFormat { get; set; }
        public CertificateSubject Subject { get; set; }
        public bool HasPrivateKey { get; set; }
        public List<AccessControlEntry> Acl { get; set; }
    }
}
