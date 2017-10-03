using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices
{
    public class UnsupportedHashAlgorithmException : System.Exception
    {
        public UnsupportedHashAlgorithmException(string message) : base(message) { }
    }
}
