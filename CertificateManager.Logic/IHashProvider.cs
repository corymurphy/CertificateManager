namespace CertificateManager.Logic
{
    public interface IHashProvider
    {
        bool ValidateData(byte[] data, byte[] knownDigest);
        byte[] ComputeHash(byte[] data);
    }
}
