using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateServices;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class MicrosoftCertificateAuthorityTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MicrosoftCertificateAuthority_Constructor_0_NullServerName_ArgumentNullException()
        {
            string serverName = null;
            string commonName = "CertificateAuthority";

            new MicrosoftCertificateAuthority(serverName, commonName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MicrosoftCertificateAuthority_Constructor_0_NullCommonName_ArgumentNullException()
        {
            string serverName = "ca.domain.com";
            string commonName = null;

            new MicrosoftCertificateAuthority(serverName, commonName);
        }

    }
}
