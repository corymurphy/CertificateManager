using CertificateServices.Interfaces;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertificateServices
{
    public static class X509Certificate2Extensions
    {
        public static CngKey GetCngKey(this X509Certificate2 cert)
        {
            ICertificateProvider provider = new Win32CertificateProvider();
            return provider.GetCngKey(cert);
        }
    }
}
