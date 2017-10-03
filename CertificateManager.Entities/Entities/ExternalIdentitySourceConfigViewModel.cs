using CertificateManager.Entities.Enumerations;
using System;

namespace CertificateManager.Entities
{
    public class ExternalIdentitySourceConfigViewModel
    {
        public ExternalIdentitySourceConfigViewModel(ExternalIdentitySource source)
        {
            id = source.Id;
            name = source.Name;
            domain = source.Domain;
            enabled = source.Enabled;
            externalIdentitySourceType = source.ExternalIdentitySourceType;
        }

        public Guid id { get; set; }
        public string name { get; set; }
        public string text { get { return this.name; } }
        public string domain { get; set; }
        public bool enabled { get; set; }
        public ExternalIdentitySourceType externalIdentitySourceType { get; set; }
    }
}
