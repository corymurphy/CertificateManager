using CertificateManager.Entities.Attributes;
using System;

namespace CertificateManager.Entities
{
    [Repository("extid")]
    public class ActiveDirectoryMetadataDomains
    {
        public Guid Id { get; set; }
        public string Domain { get; set; }
    }
}
