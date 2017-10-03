using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices
{
    public class ActiveDirectoryUnavailibleException : System.Exception
    {
        public ActiveDirectoryUnavailibleException(string message) : base(message) { }
    }
}