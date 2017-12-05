using CertificateManager.Entities.Enumerations;
using LiteDB;
using System;

namespace CertificateManager.Entities
{
    public class AccessControlEntry
    {
        public AccessControlEntry() { }

        public AccessControlEntry(string identity)
        {
            this.AceType = AceType.Allow;
            this.IdentityType = IdentityType.User;
            this.Identity = identity;
            this.Expires = DateTime.MaxValue;
            this.Id = Guid.NewGuid();
        }

        public AccessControlEntry(IdentityType identityType, string identity)
        {
            this.AceType = AceType.Allow;
            this.IdentityType = identityType;
            this.Identity = identity;
            this.Expires = DateTime.MaxValue;
            this.Id = Guid.NewGuid();
        }

        public AccessControlEntry(AceType aceType, IdentityType identityType, string identity)
        {
            this.AceType = aceType;
            this.IdentityType = identityType;
            this.Identity = identity;
            this.Expires = DateTime.MaxValue;
            this.Id = Guid.NewGuid();
        }

        public AccessControlEntry(AceType aceType, IdentityType identityType, string identity, DateTime expires)
        {
            this.AceType = aceType;
            this.IdentityType = identityType;
            this.Identity = identity;
            this.Expires = expires;
        }

        public AccessControlEntry(AddCertificateAceEntity ace)
        {
            this.AceType = ace.AceType;
            this.IdentityType = ace.IdentityType;
            this.Identity = ace.Identity;
            this.Expires = DateTime.MaxValue;
            this.Id = Guid.NewGuid();
        }

        public AccessControlEntry(AccessControlEntry ace, string identityDisplayName)
        {
            this.IdentityDisplayName = identityDisplayName;
            this.AceType = ace.AceType;
            this.IdentityType = ace.IdentityType;
            this.Identity = ace.Identity;
            this.Expires = ace.Expires;
            this.Id = ace.Id;
        }

        public AceType AceType { get; set; }
        public IdentityType IdentityType { get; set; }
        public string Identity { get; set; }
        public DateTime Expires { get; set; }
        public Guid Id { get; set; }

        [BsonIgnore]
        public string IdentityDisplayName { get; set; }

        public static AccessControlEntry New()
        {
            return new AccessControlEntry()
            {
                Id = Guid.NewGuid(),
                Expires = DateTime.MinValue,
                AceType = AceType.Deny,
                IdentityType = IdentityType.User,
                Identity = string.Empty
            };

        }
    }
}
