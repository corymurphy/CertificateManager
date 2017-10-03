using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices
{
    public class CertificateAuthorityDoesNotExistInADException : System.Exception
    {
        public CertificateAuthorityDoesNotExistInADException(string message) : base(message) { }
    }
}
