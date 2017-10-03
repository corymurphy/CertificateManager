using CertificateServices;

namespace CertificateManager.Entities
{
    public class SignPrivateCertificateModel
    {
        public HashAlgorithm HashAlgorithm { get; set; }
        public CipherAlgorithm CipherAlgorithm { get; set; }
        public string EncodedCsr { get; set; }
    }
}
