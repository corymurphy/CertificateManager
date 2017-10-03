using System;

namespace CertificateServices.Enumerations
{
    [Flags]
    public enum KeyUsage
    {
        None = 0,
        ServerAuthentication = 1,
        ClientAuthentication = 2,
        DocumentEncryption = 4,
        CodeSigning = 8,
        CertificateAuthority = 16,
        Undetermined = 32
    }
}
