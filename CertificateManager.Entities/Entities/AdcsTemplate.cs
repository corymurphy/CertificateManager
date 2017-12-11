using CertificateManager.Entities.Attributes;
using CertificateServices;
using CertificateServices.Enumerations;
using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    [Repository("AdcsTemplate")]
    public class AdcsTemplate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        //public HashAlgorithm Hash { get; set; }
        public CipherAlgorithm Cipher { get; set; }
        public KeyUsage KeyUsage { get; set; }
        public WindowsApi WindowsApi { get; set; }
        public List<Guid> RolesAllowedToIssue { get; set; }
        public int MinimumKeySize { get; set; }
    }
}
