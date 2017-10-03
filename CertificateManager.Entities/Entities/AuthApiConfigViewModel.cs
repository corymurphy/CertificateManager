using System;

namespace CertificateManager.Entities
{
    public class AuthApiConfigViewModel
    {
        public Guid Id { get; set; }
        public bool Primary { get; set; }
        public bool HasPrivateKey { get; set; }
        public string CommonName { get; set; }
        public string Thumbprint { get; set; }
        
    }
}
