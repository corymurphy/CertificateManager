using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices
{
    public class UnsupportedCertificateRequestFormat : System.Exception
    {
        public UnsupportedCertificateRequestFormat(string message) : base(message) { }
    }
}
