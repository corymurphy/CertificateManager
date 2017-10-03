using CertificateServices.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace CertificateServices
{
    public class CngNativeProviderProxy : ICngNativeProvider
    {
        public bool CertGetCertificateContextProperty(IntPtr pCertContext, uint dwPropId, IntPtr pvData, ref int pcbData)
        {
            return CngNativeProvider.CertGetCertificateContextProperty(pCertContext, dwPropId, pvData, ref pcbData);
        }

        public int NCryptFreeObject(IntPtr hObject)
        {
            return CngNativeProvider.NCryptFreeObject(hObject);
        }

        public int NCryptGetProperty(IntPtr hObject, [MarshalAs(UnmanagedType.LPWStr)] string pszProperty, byte[] pbOutput, int cbOutput, ref uint pcbResult, int dwFlags)
        {
            return CngNativeProvider.NCryptGetProperty(hObject, pszProperty, pbOutput, cbOutput, ref pcbResult, dwFlags);
        }

        public int NCryptOpenKey(IntPtr hProvider, out IntPtr phKey, [MarshalAs(UnmanagedType.LPWStr)] string pszKeyName, int dwLegacyKeySpec, int dwFlags)
        {
            return CngNativeProvider.NCryptOpenKey(hProvider, out phKey, pszKeyName, dwLegacyKeySpec, dwFlags);
        }

        public int NCryptOpenStorageProvider(ref IntPtr phProvider, [MarshalAs(UnmanagedType.LPWStr)] string pszProviderName, uint dwFlags)
        {
            return CngNativeProvider.NCryptOpenStorageProvider(ref phProvider, pszProviderName, dwFlags);
        }
    }
}
