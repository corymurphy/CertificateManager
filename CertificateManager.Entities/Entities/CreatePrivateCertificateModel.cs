using CertificateManager.Entities.Interfaces;
using CertificateServices;
using System;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager.Entities
{
    public class CreatePrivateCertificateModel : ICertificateSubjectRaw, ICertificateRequestPublicPrivateKeyPair
    {
        public CreatePrivateCertificateModel()
        {

        }

        public CreatePrivateCertificateModel(X509Certificate2 cert)
        {
            CertificateSubject subject = new CertificateSubject(cert);
            this.SubjectCommonName = subject.CommonName;
            this.CipherAlgorithm = CipherAlgorithm.RSA;
            this.HashAlgorithm = HashAlgorithm.SHA256;
            this.Provider = WindowsApi.Cng;
            this.KeySize = 2048;
        }

        public DateTime RequestDate { get; set; }
        public string SubjectCommonName { get; set; }
        public string SubjectDepartment { get; set; }
        public string SubjectOrganization { get; set; }
        public string SubjectCity { get; set; }
        public string SubjectState { get; set; }
        public string SubjectCountry { get; set; }
        public string SubjectAlternativeNamesRaw { get; set; }
        public CipherAlgorithm CipherAlgorithm { get; set; }
        public WindowsApi Provider { get; set; }
        public HashAlgorithm HashAlgorithm { get; set; }
        public int KeySize { get; set; }
        public string KeyUsage { get; set; }
    }
}
