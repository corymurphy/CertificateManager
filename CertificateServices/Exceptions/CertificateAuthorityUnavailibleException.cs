﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices
{
    public class CertificateAuthorityUnavailibleException : System.Exception
    {
        public CertificateAuthorityUnavailibleException(string message) : base(message) { }
    }
}
