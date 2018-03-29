using System;
using System.Collections.Generic;
using System.Text;

namespace CertificateManager.Powershell.Runtime
{
    public class HostIISCertificateEntity
    {
        public string Thumbprint { get; set; }
        public string X509Content { get; set; }
        public string ApplicationId { get; set; }
        public string Path { get; set; }
        //public DateTime DiscoveryDate { get; set; }
    }
}
