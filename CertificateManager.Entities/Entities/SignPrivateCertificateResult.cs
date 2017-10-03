namespace CertificateManager.Entities
{
    public class SignPrivateCertificateResult
    {
        public PrivateCertificateRequestStatus Status { get; set; }
        public string Thumbprint { get; set; }
        public string EncodedCertificate { get; set; }
    }
}
