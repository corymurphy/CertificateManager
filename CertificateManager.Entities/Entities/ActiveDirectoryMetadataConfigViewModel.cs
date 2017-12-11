using CertificateManager.Entities.Enumerations;
using System;

namespace CertificateManager.Entities
{
    public class ActiveDirectoryMetadataConfigViewModel
    {
        public ActiveDirectoryMetadataConfigViewModel(ActiveDirectoryMetadata source)
        {
            id = source.Id;
            name = source.Name;
            domain = source.Domain;
            enabled = source.Enabled;
            ActiveDirectoryMetadataType = source.ActiveDirectoryMetadataType;
        }

        public Guid id { get; set; }
        public string name { get; set; }
        public string text { get { return this.name; } }
        public string domain { get; set; }
        public bool enabled { get; set; }
        public ActiveDirectoryMetadataType ActiveDirectoryMetadataType { get; set; }
    }
}
