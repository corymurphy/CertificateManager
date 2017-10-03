using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateManager.Logic;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class SecretKeyProviderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SecretKeyProvider_NewSecret_Length2_ThrowArgumentOutOfRangeException()
        {
            int length = 2;

            SecretKeyProvider secrets = new SecretKeyProvider();

            secrets.NewSecret(length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SecretKeyProvider_NewSecret_Length130_ThrowArgumentOutOfRangeException()
        {
            int length = 130;

            SecretKeyProvider secrets = new SecretKeyProvider();

            secrets.NewSecret(length);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SecretKeyProvider_NewSecret_LengthNegative1_ThrowArgumentOutOfRangeException()
        {
            int length = -1;

            SecretKeyProvider secrets = new SecretKeyProvider();

            secrets.NewSecret(length);
        }


        [TestMethod]
        public void SecretKeyProvider_NewSecret_Length30_ReturnKeyWith30Length()
        {
            int length = 30;

            SecretKeyProvider secrets = new SecretKeyProvider();

            string secret = secrets.NewSecret(length);

            Assert.AreEqual(length, secret.Length);
        }

        [TestMethod]
        public void SecretKeyProvider_NewSecret_Length64_ReturnKeyWith64Length()
        {
            int length = 64;

            SecretKeyProvider secrets = new SecretKeyProvider();

            string secret = secrets.NewSecret(length);

            Assert.AreEqual(length, secret.Length);
        }
    }
}
