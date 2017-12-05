using CertificateServices;

namespace CertificateManager.Entities.Interfaces
{
    public interface ICertificateRequestPublicPrivateKeyPair
    {

        CipherAlgorithm CipherAlgorithm { get; }
        WindowsApi Provider { get; }
        HashAlgorithm HashAlgorithm { get; }
        string SubjectCommonName { get; }
        int KeySize { get; }
        string KeyUsage { get; }
    }
}
