using System;

namespace CertificateManager.Entities.Exceptions
{
    public class ReferencedObjectNotUniqueException : Exception
    {
        public ReferencedObjectNotUniqueException(string message) : base(message) { }
    }
}
