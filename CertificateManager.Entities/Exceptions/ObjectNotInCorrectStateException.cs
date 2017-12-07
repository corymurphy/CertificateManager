using System;

namespace CertificateManager.Entities.Exceptions
{
    public class ObjectNotInCorrectStateException : Exception
    {
        public ObjectNotInCorrectStateException(string message) : base(message) { }
    }
}
