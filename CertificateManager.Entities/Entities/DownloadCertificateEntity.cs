using CertificateManager.Entities.Attributes;
using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    [Repository("Certificate")]
    public class DownloadCertificateEntity
    {
        public Guid Id { get; set; }
        public string Thumbprint { get; set; }
        public CertificateStorageFormat CertificateStorageFormat { get; set; }
        public string PfxPassword { get; set; }
        public byte[] Content { get; set; }
        public bool HasPrivateKey { get; set; }
        public List<AccessControlEntry> Acl { get; set; }
    }
}
