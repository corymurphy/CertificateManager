using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CertificateManager.Logic
{
    public class EncryptionProvider
    {
        private byte[] keyBytes;

        public EncryptionProvider(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            this.keyBytes = DecodeKey(key);
        }

        public string Encrypt(string data, string nonce)
        {
            string encryptedData;
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] nonceBytes = DecodeNonce(nonce);

            HMACSHA256 hmac = new HMACSHA256(keyBytes);
            byte[] derivedKey = hmac.ComputeHash(nonceBytes);

            using (MemoryStream ms = new MemoryStream())
            {
                using (AesManaged aes = new AesManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Key = derivedKey;
                    aes.IV = nonceBytes;
                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataBytes, 0, dataBytes.Length);
                        cs.Close();
                    }
                    encryptedData = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptedData;
        }

        public string Decrypt(string ciphertext, string nonce)
        {
            string decryptedData;
            byte[] ciphertextByte = DecodeCiphertext(ciphertext);
            byte[] nonceBytes = DecodeNonce(nonce);

            HMACSHA256 hmac = new HMACSHA256(keyBytes);
            byte[] derivedKey = hmac.ComputeHash(nonceBytes);

            using (MemoryStream ms = new MemoryStream())
            {
                using (AesManaged aes = new AesManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Key = derivedKey;
                    aes.IV = nonceBytes;
                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(ciphertextByte, 0, ciphertextByte.Length);
                        cs.Close();
                    }
                    decryptedData = Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            return decryptedData;
        }

        private byte[] DecodeCiphertext(string ciphertext)
        {
            try
            {
                return Convert.FromBase64String(ciphertext);
            }
            catch(FormatException)
            {
                throw new FormatException("Invalid ciphertext was provided for cryptographic operation. It was not well formed base64." );
            }
        }

        private byte[] DecodeNonce(string nonce)
        {
            try
            {
                return Convert.FromBase64String(nonce);
            }
            catch (FormatException)
            {
                throw new FormatException("Invalid nonce was provided for cryptographic operation. It was not well formed base64.");
            }
        }

        private byte[] DecodeKey(string key)
        {
            try
            {
                return Convert.FromBase64String(key);
            }
            catch (FormatException)
            {
                throw new FormatException("Invalid key was provided for cryptographic operation. It was not well formed base64.");
            }
        }

    }
}
