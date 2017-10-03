using System;

namespace CertificateManager.Entities.Exceptions
{
    public class ConfigurationItemNotFoundException : Exception
    {
        public ConfigurationItemNotFoundException(string message) : base(message) { }
    }
}
