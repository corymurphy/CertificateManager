using System;

namespace CertificateManager.Entities.Exceptions
{
    public class ReferencedObjectDoesNotExistException : Exception
    {
        public ReferencedObjectDoesNotExistException(string message) : base(message) { }
        public ReferencedObjectDoesNotExistException() : base() { }
    }
}
