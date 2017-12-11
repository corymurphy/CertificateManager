namespace CertificateManager.Logic.ActiveDirectory.Interfaces
{
    public interface IActiveDirectoryAuthenticator
    {
        bool Authenticate(string username, string password, string domain);
    }
}
