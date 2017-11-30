using CertificateManager.Entities.Interfaces;
using System;
using System.Collections.Generic;
using CertificateManager.Entities.Enumerations;
using LiteDB;

namespace CertificateManager.Entities
{
    public class SecurityRole : ISecurityPrincipal
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<Guid> Member { get; set; }
        public List<Guid> Scopes { get; set; }

        [BsonIgnore]
        public IdentityType IdentityType { get { return IdentityType.Role; } }
    }
}
