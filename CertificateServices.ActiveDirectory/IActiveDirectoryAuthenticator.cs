namespace CertificateServices.ActiveDirectory
{
    public interface IActiveDirectoryAuthenticator
    {
        bool Authenticate(string username, string password, string domain);
    }
}
