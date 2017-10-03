namespace CertificateServices
{
    public class ProviderDoesNotSupportCipherAlgorithm : System.Exception
    {
        public ProviderDoesNotSupportCipherAlgorithm(string message) : base(message) { }
    }
}
