using CertificateManager.Entities.Enumerations;
using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
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
        
        //public Dictionary<ExternalIdentitySourceMetadataOption, string> Metadata { get; set; }
    }
}
