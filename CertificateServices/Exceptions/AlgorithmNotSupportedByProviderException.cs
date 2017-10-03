using System;

namespace CertificateServices
{
    public class AlgorithmNotSupportedByProviderException : System.Exception
    {
        public AlgorithmNotSupportedByProviderException(string message) : base(message) { }
    }
}
