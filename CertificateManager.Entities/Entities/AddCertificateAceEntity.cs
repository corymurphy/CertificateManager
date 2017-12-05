using CertificateManager.Entities.Enumerations;
using System;

namespace CertificateManager.Entities
{
    public class AddCertificateAceEntity
    {
        public string IdentityDisplayName { get; set; }
        public Guid Id { get; set; }
        public AceType AceType { get; set; }
        public IdentityType IdentityType { get; set; }
        public string Identity { get; set; }
    }
}
