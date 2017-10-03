namespace CertificateServices.Exceptions
{

    public class InaccessiblePrivateKeyException : System.Exception
    {
        public InaccessiblePrivateKeyException(string message) : base(message) { }
    }
}
