using CertificateManager.Entities.Attributes;
using System;

namespace CertificateManager.Entities
{
    [Repository("authapicert")]
    public class AuthApiCertificate
    {
        public Guid Id { get; set; }
        public string Thumbprint { get; set; }
        public bool HasPrivateKey { get; set; }
        public bool Primary { get; set; }
        public string DisplayName { get; set; }
    }
}
