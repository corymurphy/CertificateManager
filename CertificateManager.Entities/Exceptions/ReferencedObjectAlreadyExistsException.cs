using System;

namespace CertificateManager.Entities.Exceptions
{
    public class ReferencedObjectAlreadyExistsException : Exception
    {
        public ReferencedObjectAlreadyExistsException(string message) : base(message) { }
    }
}
