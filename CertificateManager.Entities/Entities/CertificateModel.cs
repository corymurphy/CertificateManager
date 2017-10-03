using CertificateManager.Entities.Enumerations;
using System;

namespace CertificateManager.Entities
{
    public class CertificateModel
    {
        CertificateAuthorityType CertificateAuthorityType { get; set; }
        DateTime RequestedOn { get; set; }
        DateTime IssuedOn { get; set; }
        DateTime Expires { get; set; }

    }
}
