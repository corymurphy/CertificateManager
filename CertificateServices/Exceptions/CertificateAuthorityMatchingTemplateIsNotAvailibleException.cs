﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices
{
    public class CertificateAuthorityMatchingTemplateIsNotAvailibleException : System.Exception
    {
        public CertificateAuthorityMatchingTemplateIsNotAvailibleException(string message) : base(message) { }
    }
}
