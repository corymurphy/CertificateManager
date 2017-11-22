using System;
using System.Security.Cryptography;

namespace CertificateManager.Logic
{
    public class SecretKeyProvider
    {
        private const int minLength = 4;
        private const int maxLength = 128;
        private const int stringLengthConversationFactor = 2;

        public string NewSecret(int length)
        {
            if (length <= minLength || length > maxLength)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater or equal 4 or less than 128");

            byte[] secret = new byte[length / stringLengthConversationFactor];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(secret);
            return BitConverter.ToString(secret).Replace("-", string.Empty);
        }

        public string NewSecretBase64(int length = 32)
        {
            if (length <= minLength || length > maxLength)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater or equal 4 or less than 128");

            byte[] secret = new byte[length];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(secret);
            return Convert.ToBase64String(secret);
        }

        public byte[] NewSecretByte(int length)
        {
            if (length <= minLength || length > maxLength)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater or equal 4 or less than 128");

            byte[] secret = new byte[length];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(secret);
            return secret;
        }
    }
}
