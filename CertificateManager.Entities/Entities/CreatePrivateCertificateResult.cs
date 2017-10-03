using System;

namespace CertificateManager.Entities
{
    public class CreatePrivateCertificateResult
    {
        public Guid Id { get; set; }
        public PrivateCertificateRequestStatus Status { get; set; }
        public string Password { get; set; }
        public string Pfx { get; set; }
        public string Thumbprint { get; set; }
        public string Message { get; set; }
        public string RequestId { get; set; }
        public byte[] PfxByte { get { return Convert.FromBase64String(this.Pfx); } }
    }
}
