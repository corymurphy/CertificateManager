using CertificateManager.Entities.Extensions;
using CertificateManager.Entities.Interfaces;
using System;

namespace CertificateManager.Entities
{
    public class ResetUserPasswordViewModel : ILoggableEntity
    {
        public Guid Id { get; set; }
        public string NewPassword { get; set; }

        public string GetDescription()
        {
            return string.Format("Reset password for user {0}", GetId());
        }

        public string GetId()
        {
            return Id.GetId();
        }
    }
}
