using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices
{
    public class UnableToInstallCertificateToCertificateStoreException : System.Exception
    {
        public UnableToInstallCertificateToCertificateStoreException(string message) : base(message) { }
    }
}
