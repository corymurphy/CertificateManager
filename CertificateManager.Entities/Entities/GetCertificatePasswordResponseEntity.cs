using CertificateManager.Entities.Attributes;
using LiteDB;
using System;

namespace CertificateManager.Entities
{
    [Repository("Certificate")]
    public class GetCertificatePasswordResponseEntity
    {
        public Guid Id { get; set; }


        [BsonIgnore]
        public string DecryptedPassword { get; set; }
    }
}
