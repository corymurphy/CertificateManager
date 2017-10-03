using System;
using System.Runtime.InteropServices;

namespace CertificateServices
{
    public static class CngNativeProvider
    {
        [DllImport("Crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CertGetCertificateContextProperty(IntPtr pCertContext, uint dwPropId, IntPtr pvData, ref int pcbData);

        [DllImport("ncrypt.dll", SetLastError = true)]
        public static extern int NCryptOpenStorageProvider(ref IntPtr phProvider, [MarshalAs(UnmanagedType.LPWStr)]string pszProviderName, uint dwFlags);

        [DllImport("Ncrypt.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern int NCryptOpenKey(IntPtr hProvider, out IntPtr hKey,  [MarshalAs(UnmanagedType.LPWStr)] string szKeyName, int legacykeyspec, int flags);

        [DllImport("ncrypt.dll", SetLastError = true)]
        public static extern int NCryptGetProperty(IntPtr hObject, [MarshalAs(UnmanagedType.LPWStr)]string pszProperty, byte[] pbOutput, int cbOutput, ref uint pcbResult, int dwFlags);

        [DllImport("ncrypt.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int NCryptFreeObject(IntPtr hObject);
    }
}
