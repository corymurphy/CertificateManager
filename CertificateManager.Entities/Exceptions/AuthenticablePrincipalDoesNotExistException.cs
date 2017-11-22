using System;

namespace CertificateManager.Entities.Exceptions
{
    public class AuthenticablePrincipalDoesNotExistException : Exception
    {
        public AuthenticablePrincipalDoesNotExistException() : base() { }
        public AuthenticablePrincipalDoesNotExistException(string message) : base(message) { }
    }
}
