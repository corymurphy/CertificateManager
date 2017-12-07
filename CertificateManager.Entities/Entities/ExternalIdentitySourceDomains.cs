using CertificateManager.Entities.Attributes;
using System;

namespace CertificateManager.Entities
{
    [Repository("extid")]
    public class ExternalIdentitySourceDomains
    {
        public Guid Id { get; set; }
        public string Domain { get; set; }
    }
}
