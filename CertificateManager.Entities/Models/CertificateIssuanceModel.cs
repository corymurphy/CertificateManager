using CertificateManager.Entities.Attributes;
using System;

namespace CertificateManager.Entities.Models
{
    [Repository("Certificate")]
    public class CertificateIssuanceModel
    {
        public DateTime IssuedOn { get; set; }
    }
}
