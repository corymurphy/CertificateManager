using System;
using System.Runtime.InteropServices;

namespace CertificateServices.InteropStructures
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CRYPT_KEY_PROV_INFO
    {
        [MarshalAs(UnmanagedType.LPWStr)] public string pwszContainerName;
        [MarshalAs(UnmanagedType.LPWStr)] public string pwszProvName;
        public uint dwProvType;
        public uint dwFlags;
        public uint cProvParam;
        public CRYPT_KEY_PROV_PARAM rgProvParam;
        public uint dwKeySpec;
    }
}
