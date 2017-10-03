namespace CertificateServices
{
    public class UnsupportedCipherAlgorithmException : System.Exception
    {
        public UnsupportedCipherAlgorithmException(string message) : base(message) { }
    }
}
