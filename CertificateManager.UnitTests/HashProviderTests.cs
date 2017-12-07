using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateManager.Logic;
using System.Text;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class HashProviderTests
    {
        private const string data = "HashProviderUnitTestText";
        private const string invalidData = "###HashProviderUnitTestText###";
        private const string dataExpectedHash = "8E61C8E6FBBD5B558694A93D81437EF1935BD95E26C0DAD7D841B5E3678F3F37";
        private const string dataExpectedHashBase64 = "jmHI5vu9W1WGlKk9gUN+8ZNb2V4mwNrX2EG142ePPzc=";


        [TestMethod]
        public void HashProvider_ComputeHash_ReturnsExpectedByte()
        {
            HashProvider hashProvider = new HashProvider();

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] actualHash = hashProvider.ComputeHash(dataBytes);
            string actualHashString = BitConverter.ToString(actualHash).Replace("-", string.Empty);

            Assert.AreEqual(dataExpectedHash, actualHashString);
        }

        [TestMethod]
        public void HashProvider_ValidateHash_ProvideValidData_ReturnsTrue()
        {
            HashProvider hashProvider = new HashProvider();

            byte[] validHash = Convert.FromBase64String(dataExpectedHashBase64);
            byte[] dataByte = Encoding.UTF8.GetBytes(data);

            bool result = hashProvider.ValidateData(dataByte, validHash);

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void HashProvider_ValidateHash_ProvideInvalidData_ReturnsFalse()
        {
            SecretKeyProvider keygen = new SecretKeyProvider();
            HashProvider hashProvider = new HashProvider();

            byte[] validHash = Convert.FromBase64String(dataExpectedHashBase64);
            byte[] dataByte = Encoding.UTF8.GetBytes(invalidData);

            bool result = hashProvider.ValidateData(dataByte, validHash);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void HashProvider_ValidateHash_ProvideInvalidHash_ReturnsFalse()
        {
            SecretKeyProvider keygen = new SecretKeyProvider();
            HashProvider hashProvider = new HashProvider();

            byte[] invalidHash = keygen.NewSecretByte(32);
            byte[] dataByte = Encoding.UTF8.GetBytes(data);

            bool result = hashProvider.ValidateData(dataByte, invalidHash);

            Assert.IsFalse(result);
        }
    }
}
