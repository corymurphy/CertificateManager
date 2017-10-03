using System;

namespace CertificateManager.Entities
{
    public class ResetUserPasswordViewModel
    {
        public Guid Id { get; set; }
        public string NewPassword { get; set; }
    }
}
