using System;

namespace CertificateManager.Entities.Exceptions
{
    public class PrivateCertificateAuthorityDoesNotExistException : Exception
    {
        public PrivateCertificateAuthorityDoesNotExistException(string message) : base(message) { }
    }
}
