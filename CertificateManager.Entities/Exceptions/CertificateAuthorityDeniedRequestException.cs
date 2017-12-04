using System;

namespace CertificateManager.Entities.Exceptions
{
    public class CertificateAuthorityDeniedRequestException : Exception
    {
        public CertificateAuthorityDeniedRequestException(string message) : base(message) { }
    }
}
