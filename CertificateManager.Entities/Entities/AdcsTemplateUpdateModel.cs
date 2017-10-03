using CertificateServices;
using CertificateServices.Enumerations;
using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    public class AdcsTemplateUpdateModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public HashAlgorithm Hash { get; set; }
        public CipherAlgorithm Cipher { get; set; }
        public KeyUsage KeyUsage { get; set; }
        public WindowsApi WindowsApi { get; set; }
        public string RolesAllowedToIssue { get; set; }
    }
}
