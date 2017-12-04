using System;

namespace CertificateManager.Entities.Exceptions
{
    public class RepositoryAttributeIsInvalidException : Exception
    {
        private const string message = "Repository attribute must have a value and it must be less than 20 characters";
        public RepositoryAttributeIsInvalidException() : base(message) { }
    }
}
