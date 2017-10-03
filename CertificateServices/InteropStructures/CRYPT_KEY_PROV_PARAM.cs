using System;
using System.Runtime.InteropServices;

namespace CertificateServices.InteropStructures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CRYPT_KEY_PROV_PARAM
    {
        public uint dwParam;
        public IntPtr pbData;
        public uint cbData;
        public uint dwFlags;
    }
}
