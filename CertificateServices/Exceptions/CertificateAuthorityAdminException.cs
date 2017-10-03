using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices
{
    public class CertificateAuthorityAdminException : System.Exception
    {
        public CertificateAuthorityAdminException(string message) : base(message) { }
    }
}
