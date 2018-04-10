using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateServices;
using System.Security.Cryptography.X509Certificates;
using CertificateServices.Enumerations;

namespace CertificateManager.IntegrationTests
{
    [TestClass]
    public class MicrosoftCertificateAuthorityTest
    {
        private string caServerName = "ca02.certmgr.local";
        private string caCommonName = "Certificate Manager Root Certificate Authority G1";
        private string username = "Administrator";
        private string domain = "certmgr.local";
        private string password = "Password1@";

        [TestMethod]
        public void MicrosoftCertificateAuthority_Sign_CryptoApiRsa2048_CertificateAuthorityRequestResponse_Issued()
        {

            string templateName = "ServerAuthentication-CapiRsa";

            int keysize = 2048;
            string commonName = "domain.com";
            WindowsApi api = WindowsApi.CryptoApi;
            CipherAlgorithm cipher = CipherAlgorithm.RSA;
            CertificateSubject subject = new CertificateSubject(commonName);

            Win32CertificateProvider provider = new Win32CertificateProvider();

            CertificateRequest csr = provider.CreateCsrKeyPair(subject, cipher, keysize, api, SigningRequestProtocol.Pkcs10);

            CertificateAuthorityRequestResponse response;

            MicrosoftCertificateAuthority ca = new MicrosoftCertificateAuthority(new MicrosoftCertificateAuthorityOptions()
            {
                AuthenticationRealm = domain,
                AuthenticationType = CertificateServices.Enumerations.MicrosoftCertificateAuthorityAuthenticationType.UsernamePassword,
                HashAlgorithm = HashAlgorithm.SHA256,
                ServerName = caServerName,
                CommonName = caCommonName,
                Username = username, 
                Password = password
            });
            response = ca.Sign(csr, templateName);

            Assert.AreEqual(CertificateRequestStatus.Issued, response.CertificateRequestStatus);
        }

        [TestMethod]
        public void MicrosoftCertificateAuthority_Sign_CngEcdsa256_CertificateAuthorityRequestResponse_Issued()
        {

            string templateName = "ServerAuthentication-CngEcdsa";

            int keysize = 256;
            string commonName = "domain.com";
            WindowsApi api = WindowsApi.Cng;
            CipherAlgorithm cipher = CipherAlgorithm.ECDSA;
            KeyUsage keyUsage = KeyUsage.ServerAuthentication;
            CertificateSubject subject = new CertificateSubject(commonName);

            Win32CertificateProvider provider = new Win32CertificateProvider();

            CertificateRequest csr = provider.CreateCsrKeyPair(subject, cipher, keysize, api, SigningRequestProtocol.Pkcs10);

            MicrosoftCertificateAuthority ca = new MicrosoftCertificateAuthority(new MicrosoftCertificateAuthorityOptions()
            {
                AuthenticationRealm = domain,
                AuthenticationType = MicrosoftCertificateAuthorityAuthenticationType.UsernamePassword,
                HashAlgorithm = HashAlgorithm.SHA256,
                ServerName = caServerName,
                CommonName = caCommonName,
                Username = username,
                Password = password
            });

            CertificateAuthorityRequestResponse response = ca.Sign(csr, templateName, keyUsage);

            Assert.AreEqual(CertificateRequestStatus.Issued, response.CertificateRequestStatus);
        }

        [TestMethod]
        public void MicrosoftCertificateAuthority_Sign_CngEcdh256_CertificateAuthorityRequestResponse_Issued()
        {

            string templateName = "ServerAuthentication-CngEcdh";

            int keysize = 256;
            string commonName = "domain.com";
            WindowsApi api = WindowsApi.Cng;
            CipherAlgorithm cipher = CipherAlgorithm.ECDH;
            KeyUsage keyUsage = KeyUsage.ServerAuthentication;
            CertificateSubject subject = new CertificateSubject(commonName);

            Win32CertificateProvider provider = new Win32CertificateProvider();

            CertificateRequest csr = provider.CreateCsrKeyPair(subject, cipher, keysize, api, SigningRequestProtocol.Pkcs10);

            MicrosoftCertificateAuthority ca = new MicrosoftCertificateAuthority(new MicrosoftCertificateAuthorityOptions()
            {
                AuthenticationRealm = domain,
                AuthenticationType = MicrosoftCertificateAuthorityAuthenticationType.UsernamePassword,
                HashAlgorithm = HashAlgorithm.SHA256,
                ServerName = caServerName,
                CommonName = caCommonName,
                Username = username,
                Password = password
            });

            CertificateAuthorityRequestResponse response = ca.Sign(csr, templateName, keyUsage);

            Assert.AreEqual(CertificateRequestStatus.Issued, response.CertificateRequestStatus);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            X509Store store = new X509Store("REQUEST", StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            foreach (X509Certificate2 cert in store.Certificates)
            {
                store.Remove(cert);
            }

            store = new X509Store("REQUEST", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            foreach (X509Certificate2 cert in store.Certificates)
            {
                store.Remove(cert);
            }
        }
    }
}
