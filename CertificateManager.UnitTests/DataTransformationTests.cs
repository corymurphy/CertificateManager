using CertificateManager.Logic;
using CertificateServices.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class DataTransformationTests
    {
        [TestMethod]
        public void DataTransformation_ParseKeyUsage_ValidSingleFlag_ServerAuthentication_AsString_Success()
        {
            DataTransformation dataTransformation = new DataTransformation();

            KeyUsage expectedKeyUsage = KeyUsage.ServerAuthentication;
            string keyUsageString = "ServerAuthentication";

            KeyUsage actualKeyUsage = dataTransformation.ParseKeyUsage(keyUsageString);

            Assert.AreEqual(expectedKeyUsage, actualKeyUsage);
        }

        [TestMethod]
        public void DataTransformation_ParseKeyUsage_ValidSingleFlag_ServerAuthentication_IntAsString_Success()
        {
            DataTransformation dataTransformation = new DataTransformation();

            KeyUsage expectedKeyUsage = KeyUsage.ServerAuthentication;
            string keyUsageString = "1";

            KeyUsage actualKeyUsage = dataTransformation.ParseKeyUsage(keyUsageString);

            Assert.AreEqual(expectedKeyUsage, actualKeyUsage);
        }

        [TestMethod]
        public void DataTransformation_ParseKeyUsage_ValidTwoFlags_ServerAuthenticationClientAuthentication_AsString_Success()
        {
            DataTransformation dataTransformation = new DataTransformation();

            KeyUsage expectedKeyUsage = KeyUsage.ServerAuthentication | KeyUsage.ClientAuthentication;
            string keyUsageString = "ServerAuthentication, ClientAuthentication";

            KeyUsage actualKeyUsage = dataTransformation.ParseKeyUsage(keyUsageString);

            Assert.AreEqual(expectedKeyUsage, actualKeyUsage);
        }

        [TestMethod]
        public void DataTransformation_ParseKeyUsage_ValidTwoFlags_ServerAuthenticationClientAuthentication_IntAsString_Success()
        {
            DataTransformation dataTransformation = new DataTransformation();

            KeyUsage expectedKeyUsage = KeyUsage.ServerAuthentication | KeyUsage.ClientAuthentication;
            string keyUsageString = "3";

            KeyUsage actualKeyUsage = dataTransformation.ParseKeyUsage(keyUsageString);

            Assert.AreEqual(expectedKeyUsage, actualKeyUsage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DataTransformation_ParseKeyUsage_InvalidKeyUsage_6point5_ThrowArgumentException()
        {
            DataTransformation dataTransformation = new DataTransformation();

            string keyUsageString = "6.5";

            KeyUsage actualKeyUsage = dataTransformation.ParseKeyUsage(keyUsageString);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DataTransformation_ParseKeyUsage_InvalidKeyUsage_SpecialCharacterDollar_ThrowArgumentException()
        {
            DataTransformation dataTransformation = new DataTransformation();

            string keyUsageString = "$";

            KeyUsage actualKeyUsage = dataTransformation.ParseKeyUsage(keyUsageString);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DataTransformation_ParseKeyUsage_InvalidKeyUsage_Null_ThrowArgumentNullException()
        {
            DataTransformation dataTransformation = new DataTransformation();

            string keyUsageString = null;

            KeyUsage actualKeyUsage = dataTransformation.ParseKeyUsage(keyUsageString);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DataTransformation_ParseKeyUsage_InvalidKeyUsage_Empty_ThrowArgumentNullException()
        {
            DataTransformation dataTransformation = new DataTransformation();

            string keyUsageString = string.Empty;

            KeyUsage actualKeyUsage = dataTransformation.ParseKeyUsage(keyUsageString);

        }
    }
}
