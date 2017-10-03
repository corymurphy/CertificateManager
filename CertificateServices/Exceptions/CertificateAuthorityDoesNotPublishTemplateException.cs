using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices
{
    public class CertificateAuthorityDoesNotPublishTemplateException : System.Exception
    {
        public CertificateAuthorityDoesNotPublishTemplateException(string message) : base(message) { }
    }
}
