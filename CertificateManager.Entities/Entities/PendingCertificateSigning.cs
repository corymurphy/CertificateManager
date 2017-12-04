using System;

namespace CertificateManager.Entities
{
    public class PendingCertificateSigning
    {
        public PendingCertificateSigning() { }
        public PendingCertificateSigning(SignPrivateCertificateModel requestObject)
        {
            this.Id = Guid.NewGuid();
            this.RequestObject = requestObject;
        }

        public Guid RequestingUserId { get; set; }
        public DateTime RequestDate { get; set; }
        public Guid Id { get; set; }
        public SignPrivateCertificateModel RequestObject { get; set; }
    }
}
