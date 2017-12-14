using CertificateManager.Entities.Attributes;
using CertificateManager.Entities.Extensions;
using CertificateManager.Entities.Interfaces;
using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    [Repository("Certificate")]
    public class GetCertificatePasswordEntity : ICertificatePasswordEntity, ILoggableEntity
    {
        public Guid Id { get; set; }
        public List<AccessControlEntry> Acl { get; set; }
        public string PasswordNonce { get; set; }
        public string PfxPassword { get; set; }

        public string GetDescription()
        {
            return string.Format("Certificate password viewed id: {0};", Id);
        }

        public string GetId()
        {
            return Id.GetId();
        }
    }
}
