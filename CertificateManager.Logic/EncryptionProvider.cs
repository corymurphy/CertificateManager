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

            //TODO: Validate is base64 encoded
            this.keyBytes = Convert.FromBase64String(key);
        }

        public string Encrypt(string data, string nonce)
        {
            string encryptedData;
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] nonceBytes;
            try
            {
                nonceBytes = Convert.FromBase64String(nonce);
            }
            catch
            {
                throw new FormatException("Unable to convert the key or nonce from base64 to byte array.");
            }

            HMACSHA256 hmac = new HMACSHA256(keyBytes);
            byte[] derivedKey = hmac.ComputeHash(nonceBytes);

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aes = new RijndaelManaged())
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
            byte[] ciphertextByte = Convert.FromBase64String(ciphertext);
            byte[] nonceBytes;

            try
            {
                nonceBytes = Convert.FromBase64String(nonce);
            }
            catch
            {
                throw new FormatException("Unable to convert the key or nonce from base64 to byte array.");
            }

            HMACSHA256 hmac = new HMACSHA256(keyBytes);
            byte[] derivedKey = hmac.ComputeHash(nonceBytes);

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aes = new RijndaelManaged())
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
    }
}
