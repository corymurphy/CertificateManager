using CertificateManager.Entities.Enumerations;
using System;

namespace CertificateManager.Entities
{
    public class AccessControlEntry
    {
        public AccessControlEntry()
        {
            this.Identity = string.Empty;
            this.Expires = DateTime.MinValue;
            this.AceType = AceType.Deny;
            this.IdentityType = IdentityType.User;
            this.Id = Guid.NewGuid();
        }

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

        public AceType AceType { get; set; }
        public IdentityType IdentityType { get; set; }
        public string Identity { get; set; }
        public DateTime Expires { get; set; }
        public Guid Id { get; set; }
    }
}
