using CertificateManager.Entities.Interfaces;
using System;
using System.Collections.Generic;
using CertificateManager.Entities.Enumerations;
using LiteDB;
using CertificateManager.Entities.Attributes;

namespace CertificateManager.Entities
{
    [Repository("secroles")]
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
