using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertificateServices.Interfaces
{
    public interface ICertificateProvider
    {
        CertificateRequest CreateCsrKeyPair(CertificateSubject subject, CipherAlgorithm cipher, int keysize, WindowsApi api, SigningRequestProtocol protocol);

        CertificateRequest InitializeFromEncodedCsr(string encodedCsr);

        X509Certificate2 InstallIssuedCertificate(string cert);

        X509Certificate2 CreateSelfSignedCertificate(CertificateSubject subject, CipherAlgorithm cipher, int keysize, WindowsApi api);

        PrivateKeyStorageMetadata GetPrivateKeyStorageMetadata(X509Certificate2 cert);

        CngKey GetCngKey(X509Certificate2 cert);
    }
}
