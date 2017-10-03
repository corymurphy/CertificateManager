using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices
{
    public class ProviderDoesNotSupportHashAlgorithm : System.Exception
    {
        public ProviderDoesNotSupportHashAlgorithm(string message) : base(message) { }
    }
}
