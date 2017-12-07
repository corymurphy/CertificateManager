using CertificateManager.Entities.Attributes;
using CertificateManager.Entities.Enumerations;
using System;

namespace CertificateManager.Entities
{

    [Repository("extid")]
    public class ExternalIdentitySource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string SearchBase { get; set; }
        public bool Enabled { get; set; }
        public ExternalIdentitySourceType ExternalIdentitySourceType { get; set; }      
    }
}
