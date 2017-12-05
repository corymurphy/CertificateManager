using CertificateManager.Entities.Enumerations;
using LiteDB;
using System;

namespace CertificateManager.Entities
{
    public class AccessControlEntryViewModel
    {

        public AceType AceType { get; set; }
        public IdentityType IdentityType { get; set; }
        public string Identity { get; set; }
        public DateTime Expires { get; set; }
        public Guid Id { get; set; }

        [BsonIgnore]
        public string IdentityDisplayName { get; set; }

    }
}
