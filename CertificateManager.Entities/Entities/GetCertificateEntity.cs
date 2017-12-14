using System;
using CertificateServices.Enumerations;
using CertificateServices;
using System.Collections.Generic;
using CertificateManager.Entities.Attributes;
using CertificateManager.Entities.Interfaces;
using CertificateManager.Entities.Extensions;

namespace CertificateManager.Entities
{
    [Repository("Certificate")]
    public class GetCertificateEntity : ILoggableEntity
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

        public string GetDescription()
        {
            return string.Format("Certificate viewed thumbprint: {0};id: {1};displayname: {2}", Thumbprint, Id, DisplayName);
        }

        public string GetId()
        {
            return Id.GetId();
        }
    }
}
