using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateManager.Logic;
using System.Security.Cryptography.X509Certificates;
using CertificateServices.Enumerations;
using System.Reflection;
using System.IO;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class X509NormalizationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void X509Normalization_GetKeyUsage_NullCertificate_ThrowArgumentNullException()
        {
            X509Normalization x509Normalization = new X509Normalization();

            X509Certificate2 cert = null;

            x509Normalization.GetKeyUsage(cert);
        }

        [TestMethod]
        public void X509Normalization_GetKeyUsage_NoKeyUsage_ReturnNoneKeyUsage()
        {
            KeyUsage expectedKeyUsage = KeyUsage.None;

            X509Normalization x509Normalization = new X509Normalization();

            X509Certificate2 cert = GetNoKeyUsageCapiRsaTestCertificate();

            KeyUsage actualKeyUsage = x509Normalization.GetKeyUsage(cert);

            Assert.AreEqual(expectedKeyUsage, actualKeyUsage);
        }

        [TestMethod]
        public void X509Normalization_GetKeyUsage_ServerOid_ReturnServerAuthenticationKeyUsage()
        {
            var test = GetTestCertificatePath();

            KeyUsage expectedKeyUsage = KeyUsage.ServerAuthentication;

            X509Normalization x509Normalization = new X509Normalization();

            X509Certificate2 cert = GetServerAuthenticationCngRsaTestCertificate();

            KeyUsage actualKeyUsage = x509Normalization.GetKeyUsage(cert);

            Assert.AreEqual(expectedKeyUsage, actualKeyUsage);
        }

        [TestMethod]
        public void X509Normalization_GetKeyUsage_CertificateAuthority_ReturnCertificateAuthorityKeyUsage()
        {
            var test = GetTestCertificatePath();

            KeyUsage expectedKeyUsage = KeyUsage.CertificateAuthority;

            X509Normalization x509Normalization = new X509Normalization();

            X509Certificate2 cert = GetCertificateAuthorityCngEcdsaTestCertificate();

            KeyUsage actualKeyUsage = x509Normalization.GetKeyUsage(cert);

            Assert.AreEqual(expectedKeyUsage, actualKeyUsage);
        }

        private string GetTestCertificatePath()
        {
            string executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Locati‌​on);

            return Path.Combine(executingDirectory, @"..\..\TestCertificates");
        }

        private X509Certificate2 GetClientServerAuthenticationCngRsaTestCertificate()
        {
            return new X509Certificate2(Path.Combine(GetTestCertificatePath(), "ClientServerAuthentication-CngRsa.cer"));
        }

        private X509Certificate2 GetServerAuthenticationCapiRsaTestCertificate()
        {
            return new X509Certificate2(Path.Combine(GetTestCertificatePath(), "ServerAuthentication-CapiRsa.cer"));
        }

        private X509Certificate2 GetServerAuthenticationCngEcdhTestCertificate()
        {
            return new X509Certificate2(Path.Combine(GetTestCertificatePath(), "ServerAuthentication-CngEcdh.cer"));
        }

        private X509Certificate2 GetServerAuthenticationCngEcdsaTestCertificate()
        {
            return new X509Certificate2(Path.Combine(GetTestCertificatePath(), "ServerAuthentication-CngEcdsa.cer"));
        }

        private X509Certificate2 GetServerAuthenticationCngRsaTestCertificate()
        {
            return new X509Certificate2(Path.Combine(GetTestCertificatePath(), "ServerAuthentication-CngRsa.cer"));
        }

        private X509Certificate2 GetNoKeyUsageCapiRsaTestCertificate()
        {
            return new X509Certificate2(Path.Combine(GetTestCertificatePath(), "NoKeyUsage-CapiRsa.cer"));
        }

        private X509Certificate2 GetCertificateAuthorityCngEcdsaTestCertificate()
        {
            return new X509Certificate2(Path.Combine(GetTestCertificatePath(), "CertificateAuthority.cer"));
        }
    }
}
