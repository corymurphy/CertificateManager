using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices
{
    public class KeySizeUnsupportedException : System.Exception
    {
        public KeySizeUnsupportedException(string message) : base(message) { }
    }
}
