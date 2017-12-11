using System;

namespace CertificateManager.Entities.Exceptions
{
    public class AdcsTemplateValidationException : Exception
    {
        public AdcsTemplateValidationException() : base() { }
        public AdcsTemplateValidationException(string message) : base(message) { }
    }
}
