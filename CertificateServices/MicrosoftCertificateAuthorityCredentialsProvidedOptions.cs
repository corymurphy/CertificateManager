using System;

namespace CertificateServices
{
    public class MicrosoftCertificateAuthorityCredentialsProvidedOptions
    {
        public Guid Id { get; set; }
        public string ServerName { get; set; }
        public string CommonName { get; set; }
        public HashAlgorithm HashAlgorithm { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
