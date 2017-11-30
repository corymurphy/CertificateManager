using LiteDB;
using System;

namespace CertificateManager.Entities
{

    public class SearchAuthenticablePrincipalEntity
    {
        public Guid Id { get; set; }

        [BsonField("Name")]
        public string Name { get; set; }
    }
}
