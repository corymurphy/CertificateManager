using CertificateManager.Entities.Attributes;
using System;

namespace CertificateManager.Entities.Models
{
    [Repository("audit")]
    public class LogDateModel
    {
        public DateTime Time { get; set; }
    }
}
