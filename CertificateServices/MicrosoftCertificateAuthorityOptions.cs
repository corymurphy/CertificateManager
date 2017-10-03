using CertificateServices.Enumerations;
using System;

namespace CertificateServices
{
    public class MicrosoftCertificateAuthorityOptions
    {
        public Guid Id { get; set; }
        public string ServerName { get; set; }
        public string CommonName { get; set; }
        public HashAlgorithm HashAlgorithm { get; set; }
        public string Username { get; set; }
        public string AuthenticationRealm { get; set; }
        public string Password { get; set; }
        public MicrosoftCertificateAuthorityAuthenticationType AuthenticationType { get; set; }
    }
}
