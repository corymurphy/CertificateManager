using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateServices.Interfaces;
using System.Collections.Generic;
using Moq;
using CertificateServices;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class CertificateRequestTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateRequest_Constructor_NullSubject_ArgumentNullException()
        {
            ICertificateProvider provider = new Win32CertificateProvider();
            CertificateSubject subject = null;
            List<string> san = new List<string>();

            new CertificateRequest(subject);
        }

        [TestMethod]
        public void CertificateRequest_Constructor_ValidInput_EncodedCsrPublicPropertySet()
        {
            CertificateSubject subject = new CertificateSubject("domain.com");
            ICertificateProvider provider = new Win32CertificateProvider();

        }
    }
}
