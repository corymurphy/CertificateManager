using CertificateServices.Enumerations;

namespace CertificateServices
{
    public interface ICertificateAuthorityPrivate
    {
        CertificateAuthorityRequestResponse Sign(CertificateRequest csr, string templateName, KeyUsage keyusage);
        string CommonName { get; }
    }
}
