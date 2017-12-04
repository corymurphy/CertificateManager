using System;

namespace CertificateManager.Entities
{
    public class SignPrivateCertificateResult
    {
        public SignPrivateCertificateResult() { }
        public SignPrivateCertificateResult(PrivateCertificateRequestStatus status)
        {
            this.Id = Guid.NewGuid();
            this.Status = status;
        }
        public PrivateCertificateRequestStatus Status { get; set; }
        public Guid Id { get; set; }
        public string Thumbprint { get; set; }
        public string EncodedCertificate { get; set; }
    }
}
