using CertificateManager.Entities.Attributes;
using LiteDB;
using System;

namespace CertificateManager.Entities
{
    [Repository("usr")]
    public class SearchAuthenticablePrincipalEntity
    {
        public Guid Id { get; set; }

        [BsonField("Name")]
        public string Name { get; set; }
    }
}
