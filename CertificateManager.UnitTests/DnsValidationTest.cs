using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateManager.Logic;
using System.Collections.Generic;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class DnsValidationTest
    {

        [TestMethod]
        public void DnsValidation_ValidateDnsName_0_NullName_ReturnsFalse()
        {
            string name = null;

            DnsValidation validator = new DnsValidation();

            Assert.IsFalse(validator.ValidateDnsName(name));
        }

        [TestMethod]
        public void DnsValidation_ValidateDnsName_0_EmptyName_ReturnsFalse()
        {
            string name = string.Empty;

            DnsValidation validator = new DnsValidation();

            Assert.IsFalse(validator.ValidateDnsName(name));
        }

        [TestMethod]
        public void DnsValidation_ValidateDnsName_0_WhitespaceName_ReturnsFalse()
        {
            string name = " ";

            DnsValidation validator = new DnsValidation();

            Assert.IsFalse(validator.ValidateDnsName(name));
        }

        [TestMethod]
        public void DnsValidation_ValidateDnsName_1_NullNameArray_ReturnsFalse()
        {
            string[] name = null;

            DnsValidation validator = new DnsValidation();

            Assert.IsFalse(validator.IsSubjectAlternativeNameValid(name));
        }

        [TestMethod]
        public void DnsValidation_ValidateDnsName_1_OneItemNullInArray_ReturnsFalse()
        {
            string[] name = new string[]
                {
                    "webserver",
                    "databaseserver",
                    null
                };


            DnsValidation validator = new DnsValidation();

            Assert.IsFalse(validator.IsSubjectAlternativeNameValid(name));
        }

        [TestMethod]
        public void DnsValidation_ValidateDnsName_1_ItemInArrayHasQuestionChar_ReturnsFalse()
        {
            string[] name = new string[]
                {
                    "webserver",
                    "databaseserver",
                    "some?server"
                };


            DnsValidation validator = new DnsValidation();

            Assert.IsFalse(validator.IsSubjectAlternativeNameValid(name));
        }

        [TestMethod]
        public void DnsValidation_ValidateDnsName_1_ItemInArrayHasSpaceChar_ReturnsFalse()
        {
            string[] name = new string[]
                {
                    "webserver",
                    "databaseserver",
                    "some server"
                };


            DnsValidation validator = new DnsValidation();

            Assert.IsFalse(validator.IsSubjectAlternativeNameValid(name));
        }

        [TestMethod]
        public void DnsValidation_ValidateDnsName_1_ItemInArrayHasWildcardInMiddleOfEntry_ReturnsFalse()
        {
            string[] name = new string[]
                {
                    "webserver",
                    "databaseserver",
                    "some*server"
                };


            DnsValidation validator = new DnsValidation();

            Assert.IsFalse(validator.IsSubjectAlternativeNameValid(name));
        }

        [TestMethod]
        public void DnsValidation_ValidateDnsName_1_ItemInArrayHasEmptyChar_ReturnsFalse()
        {
            string[] name = new string[]
                {
                    "webserver",
                    "databaseserver",
                    string.Empty
                };


            DnsValidation validator = new DnsValidation();

            Assert.IsFalse(validator.IsSubjectAlternativeNameValid(name));
        }

        [TestMethod]
        public void DnsValidation_ValidateDnsName_1_ItemInArrayGreaterThan253Chars_ReturnsFalse()
        {
            string entry = "thisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253chars" +
                "thisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253chars" +
                "thisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253chars" +
                "thisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253chars";

            string[] name = new string[]
                {
                    "webserver",
                    "databaseserver",
                    entry
                };


            DnsValidation validator = new DnsValidation();

            Assert.IsFalse(validator.IsSubjectAlternativeNameValid(name));

        }


        [TestMethod]
        public void DnsValidation_ValidateDnsName_1_ArraySizeIsGreaterThan4000Bytes_ReturnsFalse()
        {

            List<string> names = new List<string>();
            string entry = "thisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253charsthisdnsnameisgreaterthan253chars";

            for (int i = 0; i <= 32; i++)
            {
                names.Add(entry);
            }

            DnsValidation validator = new DnsValidation();

            Assert.IsFalse(validator.IsSubjectAlternativeNameValid(names.ToArray()));

        }

        [TestMethod]
        public void DnsValidation_ValidateDnsName_0_NameWithOnlyLetters()
        {
            string name = "webserver";

            DnsValidation validator = new DnsValidation();

            Assert.IsTrue(validator.ValidateDnsName(name));
        }


        [TestMethod]
        public void DnsValidation_ValidateDnsName_0_NameWithLettersNumbers()
        {
            string name = "webserver1";

            DnsValidation validator = new DnsValidation();

            Assert.IsTrue(validator.ValidateDnsName(name));
        }

        [TestMethod]
        public void DnsValidation_ValidateDnsName_1_ItemInArrayHasWildcardInFirstChar_ReturnsTrue()
        {
            string[] name = new string[]
                {
                    "webserver",
                    "databaseserver",
                    "*server"
                };

            DnsValidation validator = new DnsValidation();

            Assert.IsTrue(validator.IsSubjectAlternativeNameValid(name));
        }
    }
}
