using System;

namespace CertificateManager.Entities.Exceptions
{
    public class InsufficientDataException : Exception
    {
        public InsufficientDataException(string message) : base(message) { }
    }
}
