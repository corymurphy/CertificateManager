using CertificateManager.Entities.Attributes;
using CertificateManager.Entities.Enumerations;
using CertificateManager.Entities.Interfaces;
using CertificateServices;
using System;

namespace CertificateManager.Entities
{
    [Repository("PendingCerts")]
    public class PendingCertificate : ICertificateSubjectRaw, ICertificateRequestPublicPrivateKeyPair
    {
        public PendingCertificate() { }
        public PendingCertificate(CreatePrivateCertificateModel requestObject)
        {
            this.Id = Guid.NewGuid();
            this.SubjectCommonName = requestObject.SubjectCommonName;
            this.SubjectDepartment = requestObject.SubjectDepartment;
            this.SubjectOrganization = requestObject.SubjectOrganization;
            this.SubjectCity = requestObject.SubjectCity;
            this.SubjectState = requestObject.SubjectState;
            this.SubjectCountry = requestObject.SubjectCountry;
            this.SubjectAlternativeNamesRaw = requestObject.SubjectAlternativeNamesRaw;
            this.CipherAlgorithm = requestObject.CipherAlgorithm;
            this.HashAlgorithm = requestObject.HashAlgorithm;
            this.Provider = requestObject.Provider;
            this.KeySize = requestObject.KeySize;
            this.KeyUsage = requestObject.KeyUsage;
            this.PendingCertificateRequestType = PendingCertificateRequestType.NewPublicPrivateKey;

        }

        public PendingCertificate(SignPrivateCertificateModel requestObject)
        {
            this.Id = Guid.NewGuid();
            this.CipherAlgorithm = requestObject.CipherAlgorithm;
            this.HashAlgorithm = requestObject.HashAlgorithm;
            this.EncodedCsr = requestObject.EncodedCsr;
            this.PendingCertificateRequestType = PendingCertificateRequestType.PublicKeySigningPrivateCa;
        }


        public Guid Id { get; set; }
        public Guid RequestingUserId { get; set; }
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
        public string EncodedCsr { get; set; }
        public PendingCertificateRequestType PendingCertificateRequestType { get; set; }

    }
}
