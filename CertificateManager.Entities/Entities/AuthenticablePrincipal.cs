using CertificateManager.Entities.Interfaces;
using System;
using System.Collections.Generic;
using CertificateManager.Entities.Enumerations;
using LiteDB;

namespace CertificateManager.Entities
{
    public class AuthenticablePrincipal : ISecurityPrincipal
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
    }
}
