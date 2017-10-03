using System;
using System.Security.Cryptography;

namespace CertificateManager.Logic
{
    public class SecretKeyProvider
    {
        public string NewSecret(int length)
        {
            if (length <= 4 || length > 128)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater or equal 4 or less than 128");

            byte[] secret = new byte[length / 2];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(secret);
            return BitConverter.ToString(secret).Replace("-", string.Empty);
        }

        public byte[] NewSecretByte(int length)
        {
            if (length <= 4 || length > 128)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater or equal 4 or less than 128");

            byte[] secret = new byte[length];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(secret);
            return secret;
        }
    }
}
