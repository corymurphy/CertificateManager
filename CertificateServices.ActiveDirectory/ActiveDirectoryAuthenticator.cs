using System.DirectoryServices.AccountManagement;

namespace CertificateServices.ActiveDirectory
{
    public class ActiveDirectoryAuthenticator : IActiveDirectoryAuthenticator
    {
        public bool Authenticate(string username, string password, string domain)
        {
            using (PrincipalContext principal = new PrincipalContext(ContextType.Domain, domain))
            {
                return principal.ValidateCredentials(username, password);
            }
        }
    }
}
