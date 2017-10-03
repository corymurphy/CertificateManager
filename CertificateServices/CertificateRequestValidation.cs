namespace CertificateServices
{
    public class CertificateRequestValidation
    {

        public bool IsValidKeySize(CipherAlgorithm cipher, int keySize)
        {
            if (cipher == CipherAlgorithm.RSA && IsValidRsaKeySize(keySize))
                return true;

            if (cipher == CipherAlgorithm.ECDH && IsValidEcKeySize(keySize))
                return true;

            if (cipher == CipherAlgorithm.ECDSA && IsValidEcKeySize(keySize))
                return true;

            return false;
        }

        public bool IsValidWindowsApiForCipherAlgorithm(CipherAlgorithm cipher, WindowsApi api)
        {
            if (api == WindowsApi.CryptoApi && cipher == CipherAlgorithm.RSA)
                return true;

            if (api == WindowsApi.Cng && cipher == CipherAlgorithm.RSA)
                return true;

            if (api == WindowsApi.Cng && cipher == CipherAlgorithm.ECDSA)
                return true;

            if (api == WindowsApi.Cng && cipher == CipherAlgorithm.ECDH)
                return true;

            return false;
        }

        private bool IsValidRsaKeySize(int keySize)
        {
            switch (keySize)
            {
                case 2048: return true;
                case 4096: return true;
                case 8192: return true;
                case 16384: return true;
                default: return false;
            }
        }

        private bool IsValidEcKeySize(int keySize)
        {
            switch (keySize)
            {
                case 256: return true;
                case 384: return true;
                case 521: return true;
                default: return false;
            }
        }
    }
}
