using CertificateManager.Entities.Attributes;
using CertificateManager.Entities.Interfaces;
using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    [Repository("Certificate")]
    public class GetCertificatePasswordEntity : ICertificatePasswordEntity
    {
        public Guid Id { get; set; }
        public List<AccessControlEntry> Acl { get; set; }
        public string PasswordNonce { get; set; }
        public string PfxPassword { get; set; }
    }
}
