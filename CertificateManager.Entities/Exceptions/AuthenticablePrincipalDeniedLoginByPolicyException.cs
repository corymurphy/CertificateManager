using System;

namespace CertificateManager.Entities.Exceptions
{
    public class AuthenticablePrincipalDeniedLoginByPolicyException : Exception
    {
        public AuthenticablePrincipalDeniedLoginByPolicyException() : base() { }
        public AuthenticablePrincipalDeniedLoginByPolicyException(string message) : base(message) { }
    }
}
