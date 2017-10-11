using CertificateServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateManager.Entities
{
    public class AllCertificatesViewModel
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Thumbprint { get; set; }
        public DateTime ValidTo { get; set; }
        public HashAlgorithm HashAlgorithm { get; set; }
        public CipherAlgorithm CipherAlgorithm { get; set; }
    }
}
