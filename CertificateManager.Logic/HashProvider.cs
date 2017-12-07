using System;
using System.Security.Cryptography;

namespace CertificateManager.Logic
{
    public class HashProvider : IHashProvider
    {
        SHA256Managed shaProvider;

        public HashProvider()
        {
            shaProvider = new SHA256Managed();
        }

        public bool ValidateData(byte[] data, byte[] knownDigest)
        {
            SHA256Managed shaProvider = new SHA256Managed();

            byte[] actualHash = shaProvider.ComputeHash(data);

            return (Convert.ToBase64String(actualHash) == Convert.ToBase64String(knownDigest));
        }

        public byte[] ComputeHash(byte[] data)
        {
            return shaProvider.ComputeHash(data);
        }
    }
}
