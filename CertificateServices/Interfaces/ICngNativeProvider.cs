using System;
using System.Runtime.InteropServices;

namespace CertificateServices.Interfaces
{
    public interface ICngNativeProvider
    {
        bool CertGetCertificateContextProperty(IntPtr pCertContext, uint dwPropId, IntPtr pvData, ref int pcbData);

        int NCryptOpenStorageProvider(ref IntPtr phProvider, [MarshalAs(UnmanagedType.LPWStr)]string pszProviderName, uint dwFlags);

        int NCryptOpenKey(IntPtr hProvider, out IntPtr hKey, [MarshalAs(UnmanagedType.LPWStr)] string szKeyName, int legacykeyspec, int flags);

        int NCryptGetProperty(IntPtr hObject, [MarshalAs(UnmanagedType.LPWStr)]string pszProperty, byte[] pbOutput, int cbOutput, ref uint pcbResult, int dwFlags);

        int NCryptFreeObject(IntPtr hObject);
    }
}
