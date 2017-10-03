using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateServices;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class CertificateRequestValidationTest
    {
        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_RSA2048_ReturnsTrue()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 2048;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.RSA, keysize);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_RSA4096_ReturnsTrue()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 4096;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.RSA, keysize);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_RSA8192_ReturnsTrue()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 8192;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.RSA, keysize);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_RSA16384_ReturnsTrue()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 16384;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.RSA, keysize);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_RSA9000_ReturnsFalse()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 9000;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.RSA, keysize);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_RSA0_ReturnsFalse()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 0;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.RSA, keysize);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_ECDH0_ReturnsFalse()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 0;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.ECDH, keysize);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_ECDSA0_ReturnsFalse()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 0;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.ECDSA, keysize);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_ECDH9000_ReturnsFalse()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 9000;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.ECDH, keysize);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_ECDSA9000_ReturnsFalse()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 9000;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.ECDSA, keysize);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_ECDH256_ReturnsTrue()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 256;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.ECDH, keysize);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_ECDSA256_ReturnsTrue()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 256;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.ECDSA, keysize);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_ECDH384_ReturnsTrue()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 384;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.ECDH, keysize);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_ECDSA384_ReturnsTrue()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 384;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.ECDSA, keysize);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_ECDH521_ReturnsTrue()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 521;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.ECDH, keysize);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CertificateRequestValidation_IsValidKeySize_ECDSA521_ReturnsTrue()
        {
            CertificateRequestValidation requestValidation = new CertificateRequestValidation();

            int keysize = 521;

            bool result = requestValidation.IsValidKeySize(CipherAlgorithm.ECDSA, keysize);

            Assert.IsTrue(result);
        }
    }
}
