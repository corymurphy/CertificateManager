namespace CertificateServices.Exceptions
{

    public class PrivateKeyDoesNotExistException : System.Exception
    {
        public PrivateKeyDoesNotExistException(string message) : base(message) { }
    }
}
