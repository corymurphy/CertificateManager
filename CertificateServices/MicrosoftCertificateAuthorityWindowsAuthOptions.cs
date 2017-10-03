using System;

namespace CertificateServices
{
    public class MicrosoftCertificateAuthorityWindowsAuthOptions
    {
        public Guid Id { get; set; }
        public string ServerName { get; set; }
        public string CommonName { get; set; }
        public HashAlgorithm HashAlgorithm { get; set; }
    }
}
