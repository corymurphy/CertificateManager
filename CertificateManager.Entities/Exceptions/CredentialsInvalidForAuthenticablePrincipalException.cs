using System;

namespace CertificateManager.Entities.Exceptions
{
    public class CredentialsInvalidForAuthenticablePrincipalException : Exception
    {
        public CredentialsInvalidForAuthenticablePrincipalException() : base() { }
        public CredentialsInvalidForAuthenticablePrincipalException(string message) : base(message) { }
    }
}
