using CertificateManager.Entities.Attributes;
using CertificateManager.Entities.Enumerations;
using System;

namespace CertificateManager.Entities
{

    [Repository("extid")]
    public class ActiveDirectoryMetadata
    {
        public ActiveDirectoryMetadata() { }

        public ActiveDirectoryMetadata(string domain, string username, string password)
        {
            this.Username = username;
            this.Domain = domain;
            this.Password = password;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string SearchBase { get; set; }
        public bool Enabled { get; set; }
        public ActiveDirectoryMetadataType ActiveDirectoryMetadataType { get; set; }      
    }
}
