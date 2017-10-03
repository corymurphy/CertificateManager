using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateServices;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CertificateManager.IntegrationTests
{
    [TestClass]
    public class Win32CertificateProviderTests
    {
        List<X509Certificate2> certs = new List<X509Certificate2>();

        //[TestMethod]
        //public void Win32CertificateProvider_CreateSelfSignedCertificate_CngEcda256_Success()
        //{
        //    int keysize = 256;
        //    string commonName = "domain.com";
        //    WindowsApi api = WindowsApi.Cng;
        //    CipherAlgorithm cipher = CipherAlgorithm.ECDSA;
        //    CertificateSubject subject = new CertificateSubject(commonName);

        //    Win32CertificateProvider provider = new Win32CertificateProvider();

        //    X509Certificate2 cert = provider.CreateSelfSignedCertificate(subject, cipher, keysize, api);

        //    Assert.IsNotNull(cert);
        //}

        [TestMethod]
        public void Win32CertificateProvider_CreateCsrKeyPair_CryptoApiRsa2048_Success()
        {
            int keysize = 2048;
            string commonName = "domain.com";
            WindowsApi api = WindowsApi.CryptoApi;
            CipherAlgorithm cipher = CipherAlgorithm.RSA;
            CertificateSubject subject = new CertificateSubject(commonName);

            Win32CertificateProvider provider = new Win32CertificateProvider();

            CertificateRequest csr = provider.CreateCsrKeyPair(subject, cipher, keysize, api, SigningRequestProtocol.Pkcs10);

            Assert.IsNotNull(csr.EncodedCsr);
        }

        [TestMethod]
        public void Win32CertificateProvider_CreateCsrKeyPair_CngRsa2048_Success()
        {
            int keysize = 2048;
            string commonName = "domain.com";
            WindowsApi api = WindowsApi.Cng;
            CipherAlgorithm cipher = CipherAlgorithm.RSA;
            CertificateSubject subject = new CertificateSubject(commonName);

            Win32CertificateProvider provider = new Win32CertificateProvider();

            CertificateRequest csr = provider.CreateCsrKeyPair(subject, cipher, keysize, api, SigningRequestProtocol.Pkcs10);

            Assert.IsNotNull(csr.EncodedCsr);
        }

        [TestMethod]
        public void Win32CertificateProvider_CreateCsrKeyPair_CngEcdsa256_EncodedCsr_IsNotNull()
        {
            int keysize = 256;
            string commonName = "domain.com";
            WindowsApi api = WindowsApi.Cng;
            CipherAlgorithm cipher = CipherAlgorithm.ECDSA;
            CertificateSubject subject = new CertificateSubject(commonName);

            Win32CertificateProvider provider = new Win32CertificateProvider();

            CertificateRequest csr = provider.CreateCsrKeyPair(subject, cipher, keysize, api, SigningRequestProtocol.Pkcs10);

            Assert.IsNotNull(csr.EncodedCsr);
        }

        [TestMethod]
        public void Win32CertificateProvider_CreateCsrKeyPair_CngEcdh256_EncodedCsr_IsNotNull()
        {
            int keysize = 256;
            string commonName = "domain.com";
            WindowsApi api = WindowsApi.Cng;
            CipherAlgorithm cipher = CipherAlgorithm.ECDH;
            CertificateSubject subject = new CertificateSubject(commonName);

            Win32CertificateProvider provider = new Win32CertificateProvider();

            CertificateRequest csr = provider.CreateCsrKeyPair(subject, cipher, keysize, api, SigningRequestProtocol.Pkcs10);

            Assert.IsNotNull(csr.EncodedCsr);
        }

        [TestMethod]
        public void Win32CertificateProvider_CreateCsrKeyPair_ManagedPrivateKey_IsTrue()
        {
            int keysize = 256;
            string commonName = "domain.com";
            WindowsApi api = WindowsApi.Cng;
            CipherAlgorithm cipher = CipherAlgorithm.ECDSA;
            CertificateSubject subject = new CertificateSubject(commonName);

            Win32CertificateProvider provider = new Win32CertificateProvider();

            CertificateRequest csr = provider.CreateCsrKeyPair(subject, cipher, keysize, api, SigningRequestProtocol.Pkcs10);

            Assert.IsTrue(csr.ManagedPrivateKey);
        }

        [TestMethod]
        public void Win32CertificateProvider_CreateCsrKeyPair_EncodedCsr_IsValidBase64()
        {
            int keysize = 256;
            string commonName = "domain.com";
            WindowsApi api = WindowsApi.Cng;
            CipherAlgorithm cipher = CipherAlgorithm.ECDSA;
            CertificateSubject subject = new CertificateSubject(commonName);

            Win32CertificateProvider provider = new Win32CertificateProvider();

            CertificateRequest csr = provider.CreateCsrKeyPair(subject, cipher, keysize, api, SigningRequestProtocol.Pkcs10);

            byte[] csrByte = Convert.FromBase64String(csr.EncodedCsr);

            Assert.IsNotNull(csrByte);
        }

        [TestMethod]
        public void Win32CertificateProvider_GetCngKey_CngRsa_ReturnsCngKey()
        {
            X509Certificate2 cert = GetCertificate("Win32CertificateProviderTests.Cng.Rsa.2048");

            Win32CertificateProvider provider = new Win32CertificateProvider();

            CngKey key = provider.GetCngKey(cert);

            Assert.IsNotNull(key.UniqueName);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Win32CertificateProvider certificateProvider = new Win32CertificateProvider();

            CertificateSubject subject = new CertificateSubject("Win32CertificateProviderTests.Cng.Rsa.2048");

            certificateProvider.CreateSelfSignedCertificate(subject, CipherAlgorithm.RSA, 2048, WindowsApi.Cng);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            X509Store store = new X509Store("REQUEST", StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            foreach (X509Certificate2 cert in store.Certificates)
            {
                store.Remove(cert);
            }


            store = new X509Store("My", StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            foreach (X509Certificate2 cert in store.Certificates)
            {
                if(cert.Subject.StartsWith("CN=Win32CertificateProviderTests"))
                {
                    store.Remove(cert);
                }
                
            }

            store = new X509Store("REQUEST", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            foreach (X509Certificate2 cert in store.Certificates)
            {
                store.Remove(cert);
            }
        }

        public X509Certificate2 GetCertificate(string name)
        {
            string subject = string.Format("CN={0}", name);

            X509Store store = new X509Store("My", StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            foreach (X509Certificate2 cert in store.Certificates)
            {
                if (cert.Subject.StartsWith(subject))
                {
                    return cert;
                }

            }

            throw new Exception("Tests not initialize correctly, couldn't find the specified cert");
        }
    }
}
