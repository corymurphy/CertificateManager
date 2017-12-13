using CertificateManager.Entities.Interfaces;
using System;
using System.Collections.Generic;
using CertificateManager.Entities.Enumerations;
using LiteDB;
using CertificateManager.Entities.Attributes;

namespace CertificateManager.Entities
{
    [Repository("usr")]
    public class AuthenticablePrincipal : ISecurityPrincipal, ILoggableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> AlternativeNames { get; set; }
        public DateTime LastLogonDate { get; set; }
        public bool Enabled { get; set; }
        public Guid LastLogonRealm { get; set; }
        public bool LocalLogonEnabled { get; set; }
        public string PasswordHash { get; set; }

        [BsonIgnore]
        public IdentityType IdentityType { get { return IdentityType.User; } }

        public string GetDescription()
        {
            return Name;
        }

        public string GetId()
        {
            return Id.ToString();
        }
    }
}
