using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateManager.Logic;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class EncryptionProviderTests
    {
        private string validPlaintext = "secretmessage";
        private string validKey = "YOVNoX+UDrLSGrcmD6a1DANau+kceq+7C9UkFX+NKb4=";
        private string validNonce = "DXxWipThPczNqEzAdP8KHw==";
        private string validCiphertext = "8EXRUG1mi6u6YybAkJ642A==";
        private string invalidKeyWithText = "zzzz$";
        private string invalidCiphertextWithText = "zzzz$";
        private string invalidNonce = "zzzz$";

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void EncryptionProvider_Constructor_InvalidBase64_ThrowsException()
        {
            EncryptionProvider cipher = new EncryptionProvider(invalidKeyWithText);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EncryptionProvider_Constructor_NullKey_ThrowsException()
        {
            EncryptionProvider cipher = new EncryptionProvider(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EncryptionProvider_Constructor_EmptyKey_ThrowsException()
        {
            EncryptionProvider cipher = new EncryptionProvider(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void EncryptionProvider_Decrypt_InvalidCipherText_ThrowsException()
        {
            EncryptionProvider cipher = new EncryptionProvider(validKey);

            cipher.Decrypt(invalidCiphertextWithText, validNonce);
        }


        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void EncryptionProvider_Decrypt_InvalidNonce_ThrowsException()
        {
            EncryptionProvider cipher = new EncryptionProvider(validKey);

            cipher.Decrypt(validCiphertext, invalidNonce);
        }


        [TestMethod]
        public void EncryptionProvider_Encrypt_ReturnsExpectedCiphertext()
        {
            EncryptionProvider cipher = new EncryptionProvider(validKey);
            string ciphertext = cipher.Encrypt(validPlaintext, validNonce);
            Assert.AreEqual(validCiphertext, ciphertext);
        }

        [TestMethod]
        public void EncryptionProvider_Decrypt_ReturnsExpectedPlaintext()
        {
            EncryptionProvider cipher = new EncryptionProvider(validKey);
            string plaintext = cipher.Decrypt(validCiphertext, validNonce);
            Assert.AreEqual(validPlaintext, plaintext);
        }

        [TestMethod]
        public void EncryptionProvider_DecryptSuccessfullyDecryptsTheEncryptMethod()
        {
            EncryptionProvider cipher = new EncryptionProvider(validKey);

            string plaintext = cipher.Decrypt(validCiphertext, validNonce);

            Assert.AreEqual(validPlaintext, plaintext);
        }

        [TestMethod]
        public void EncryptionProvider_EncryptSuccessfullyDecrypts_EncryptMethod()
        {
            EncryptionProvider cipher = new EncryptionProvider(validKey);

            string ciphertext = cipher.Encrypt(validPlaintext, validNonce);

            Assert.AreEqual(validCiphertext, ciphertext);
        }
    }
}
