using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateServices;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class Win32CertificateProviderTests
    {
        [TestMethod]
        [ExpectedException(typeof(AlgorithmNotSupportedByProviderException))]
        public void Win32CertificateProvider_CreateCsrKeyPair_CryptoApiEcdh_AlgorithmNotSupportedByProviderException()
        {
            int keysize = 2048;
            string commonName = "domain.com";
            WindowsApi api = WindowsApi.CryptoApi;
            CipherAlgorithm cipher = CipherAlgorithm.ECDH;
            CertificateSubject subject = new CertificateSubject(commonName);

            Win32CertificateProvider provider = new Win32CertificateProvider();

            provider.CreateCsrKeyPair(subject, cipher, keysize, api, SigningRequestProtocol.Pkcs10);
        }

        [TestMethod]
        [ExpectedException(typeof(AlgorithmNotSupportedByProviderException))]
        public void Win32CertificateProvider_CreateCsrKeyPair_CryptoApiEcdsa_AlgorithmNotSupportedByProviderException()
        {
            int keysize = 2048;
            string commonName = "domain.com";
            WindowsApi api = WindowsApi.CryptoApi;
            CipherAlgorithm cipher = CipherAlgorithm.ECDSA;

            CertificateSubject subject = new CertificateSubject(commonName);

            Win32CertificateProvider provider = new Win32CertificateProvider();

            provider.CreateCsrKeyPair(subject, cipher, keysize, api, SigningRequestProtocol.Pkcs10);
        }
    }
}
