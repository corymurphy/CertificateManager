using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CertificateServices;
using System.Collections.Generic;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class CertificateSubjectTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_Constructor_0_NullCommonName_ArgumentNullException()
        {
            string commonName = null;

            new CertificateSubject(commonName);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CertificateSubject_Constructor_0_GreaterThan64CharactersCommonName_ArgumentOutOfRangeException()
        {
            string commonName = "This string must be greater than 64 characters to throw the exception of the common name limit";

            new CertificateSubject(commonName);

        }

        [TestMethod]
        public void CertificateSubject_Constructor_0_AppendSandTrue_SanIsAppendedSuccess()
        {
            string commonName = "domain.com";

            CertificateSubject subject = new CertificateSubject(commonName, true);

            Assert.IsTrue(subject.SubjectAlternativeName.Contains(commonName));
        }

        [TestMethod]
        public void CertificateSubject_Constructor_0_AppendSandTrue_ContainsSubjectAlternativeNameTrue()
        {
            string commonName = "domain.com";

            CertificateSubject subject = new CertificateSubject(commonName, true);

            Assert.IsTrue(subject.ContainsSubjectAlternativeName);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_0_AppendSandFalse_SanIsNotAppendedSuccess()
        {
            bool nullSan = false;
            string commonName = "domain.com";

            CertificateSubject subject = new CertificateSubject(commonName, false);

            if (subject.SubjectAlternativeName == null)
            {
                nullSan = true;
            }
                
            Assert.IsTrue(nullSan);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_0_AppendSandFalse_ContainsSubjectAlternativeNameFalse()
        {
            string commonName = "domain.com";

            CertificateSubject subject = new CertificateSubject(commonName, false);

            Assert.IsFalse(subject.ContainsSubjectAlternativeName);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_0_SanListOverload_SanIsIncluded()
        {
            string commonName = "domain.com";
            List<string> san = new List<string>() { commonName, "corp.domain.com" };

            CertificateSubject subject = new CertificateSubject(commonName, san);

            Assert.AreEqual(san, subject.SubjectAlternativeName);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_0_SanListOverload_NullSan_ContainsSubjectAlternativeNameFalse()
        {
            string commonName = "domain.com";

            CertificateSubject subject = new CertificateSubject(commonName, (List<string>)null);

            Assert.IsFalse(subject.ContainsSubjectAlternativeName);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_0_SanListOverload_ContainsSubjectAlternativeNameTrue()
        {
            string commonName = "domain.com";
            List<string> san = new List<string>() { commonName, "corp.domain.com" };

            CertificateSubject subject = new CertificateSubject(commonName, san);

            Assert.IsTrue(subject.ContainsSubjectAlternativeName);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_0_CommonNameArgumentSetToCommonNamePublicProperty()
        {
            string commonName = "myfakedomain.fake";

            CertificateSubject subject = new CertificateSubject(commonName);

            Assert.AreEqual(commonName, subject.CommonName);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CertificateSubject_Constructor_1_GreaterThan64CharactersCommonName_ArgumentOutOfRangeException()
        {
            string commonName = "This string must be greater than 64 characters to throw the exception of the common name limit";
            string department = "Engineering";
            string organization = "TechCorp";
            new CertificateSubject(commonName, department, organization);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CertificateSubject_Constructor_1_CommonNameNull_ArgumentOutOfRangeException()
        {
            string commonName = null;
            string department = "Engineering";
            string organization = "TechCorp";

            new CertificateSubject(commonName, department, organization);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CertificateSubject_Constructor_1_DepartmentNull_ArgumentOutOfRangeException()
        {
            string commonName = "domain.com";
            string department = null;
            string organization = "TechCorp";

            new CertificateSubject(commonName, department, organization);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CertificateSubject_Constructor_1_OrganizationNull_ArgumentOutOfRangeException()
        {
            string commonName = "domain.com";
            string department = "Engineering";
            string organization = null;

            new CertificateSubject(commonName, department, organization);

        }

        [TestMethod]
        public void CertificateSubject_Constructor_1_CommonNameArgumentSetToCommonNamePublicProperty()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization);

            Assert.AreEqual(commonName, subject.CommonName);

        }

        [TestMethod]
        public void CertificateSubject_Constructor_1_DepartmentArgumentSetToCommonNamePublicProperty()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization);

            Assert.AreEqual(department, subject.Department);

        }

        [TestMethod]
        public void CertificateSubject_Constructor_1_OrganizationArgumentSetToCommonNamePublicProperty()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization);

            Assert.AreEqual(organization, subject.Organization);

        }

        [TestMethod]
        public void CertificateSubject_Constructor_1_AppendSandTrue_SanIsAppendedSuccess()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, true);

            Assert.IsTrue(subject.SubjectAlternativeName.Contains(commonName));
        }

        [TestMethod]
        public void CertificateSubject_Constructor_1_AppendSandTrue_ContainsSubjectAlternativeNameTrue()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, true);

            Assert.IsTrue(subject.ContainsSubjectAlternativeName);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_1_AppendSandFalse_SanIsNotAppendedSuccess()
        {
            bool nullSan = false;
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, false);

            if (subject.SubjectAlternativeName == null)
            {
                nullSan = true;
            }

            Assert.IsTrue(nullSan);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_1_AppendSandFalse_ContainsSubjectAlternativeNameFalse()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, false);

            Assert.IsFalse(subject.ContainsSubjectAlternativeName);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_1_SanListOverload_SanIsIncluded()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            List<string> san = new List<string>() { commonName, "corp.domain.com" };

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, san);

            Assert.AreEqual(san, subject.SubjectAlternativeName);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_1_SanListOverload_ContainsSubjectAlternativeNameTrue()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            List<string> san = new List<string>() { commonName, "corp.domain.com" };

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, san);

            Assert.IsTrue(subject.ContainsSubjectAlternativeName);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_1_SanListOverload_NullSan_ContainsSubjectAlternativeNameFalse()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, (List<string>)null );

            Assert.IsFalse(subject.ContainsSubjectAlternativeName);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_2_SanListOverload_NullSan_ContainsSubjectAlternativeNameFalse()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, (List<string>)null);

            Assert.IsFalse(subject.ContainsSubjectAlternativeName);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_2_AppendSandTrue_SanIsAppendedSuccess()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);

            Assert.IsTrue(subject.SubjectAlternativeName.Contains(commonName));
        }

        [TestMethod]
        public void CertificateSubject_Constructor_2_AppendSandTrue_ContainsSubjectAlternativeNameTrue()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);

            Assert.IsTrue(subject.ContainsSubjectAlternativeName);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_2_AppendSandFalse_SanIsNotAppendedSuccess()
        {
            bool nullSan = false;
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, false);

            if (subject.SubjectAlternativeName == null)
            {
                nullSan = true;
            }

            Assert.IsTrue(nullSan);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_2_AppendSandFalse_ContainsSubjectAlternativeNameFalse()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, false);

            Assert.IsFalse(subject.ContainsSubjectAlternativeName);
        }

        [TestMethod]
        public void CertificateSubject_Constructor_2_SanListOverload_SanIsIncluded()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            List<string> san = new List<string>() { commonName, "corp.domain.com" };

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, san);

            Assert.AreEqual(san, subject.SubjectAlternativeName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "commonName")]
        public void CertificateSubject_Constructor_2_NullCommonName_ThrowsArgumentNullException()
        {
            string commonName = null;
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "commonName")]
        public void CertificateSubject_Constructor_2_EmptyCommonName_ThrowsArgumentNullException()
        {
            string commonName = null;
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "commonName")]
        public void CertificateSubject_Constructor_2_GreaterThan64CharacterCommonName_ThrowsArgumentOutOfRangeException()
        {
            string commonName = "This string must be greater than 64 characters to throw the exception of the common name limit";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_Constructor_2_NullDepartment_ThrowsArgumentNullException()
        {
            string commonName = "myfakedomain.fake";
            string department = null;
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_Constructor_2_EmptyDepartment_ThrowsArgumentNullException()
        {
            string commonName = "myfakedomain.fake";
            string department = string.Empty;
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CertificateSubject_Constructor_2_GreaterThan64CharacterDepartment_ThrowsArgumentOutOfRangeException()
        {
            string commonName = "domain.com";
            string department = "This string must be greater than 64 characters to throw the exception of the parameter limit";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_Constructor_2_NullOrganization_ThrowsArgumentNullException()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = null;
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_Constructor_2_EmptyOrganization_ThrowsArgumentNullException()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = string.Empty;
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CertificateSubject_Constructor_2_GreaterThan64CharacterOrganization_ThrowsArgumentOutOfRangeException()
        {
            string commonName = "domain.com";
            string department = "Engineering";
            string organization = "This string must be greater than 64 characters to throw the exception of the parameter limit";
            string city = "walpole";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_Constructor_2_NullCity_ThrowsArgumentNullException()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = null;
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_Constructor_2_EmptyCity_ThrowsArgumentNullException()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = string.Empty;
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CertificateSubject_Constructor_2_GreaterThan64CharacterCity_ThrowsArgumentOutOfRangeException()
        {
            string commonName = "domain.com";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "This string must be greater than 64 characters to throw the exception of the parameter limit";
            string state = "MA";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_Constructor_2_NullState_ThrowsArgumentNullException()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = null;
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_Constructor_2_EmptyState_ThrowsArgumentNullException()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = string.Empty;
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CertificateSubject_Constructor_2_GreaterThan64CharacterState_ThrowsArgumentOutOfRangeException()
        {
            string commonName = "domain.com";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "This string must be greater than 64 characters to throw the exception of the parameter limit";
            string country = "JP";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_Constructor_2_NullCountry_ThrowsArgumentNullException()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = null;

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_Constructor_2_EmptyCountry_ThrowsArgumentNullException()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = string.Empty;

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CertificateSubject_Constructor_2_Country3Characters_ThrowsArgumentOutOfRangeException()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "USA";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CertificateSubject_Constructor_2_Country1Character_ThrowsArgumentOutOfRangeException()
        {
            string commonName = "myfakedomain.fake";
            string department = "Engineering";
            string organization = "TechCorp";
            string city = "walpole";
            string state = "MA";
            string country = "U";

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country, true);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_CreateFromDistinguishedName_NullDistinguishedName_ArgumentNullException()
        {
            string dn = null;

            CertificateSubject.CreateFromDistinguishedName(dn);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_CreateFromDistinguishedName_EmptyDistinguishedName_ArgumentNullException()
        {
            string dn = string.Empty;

            CertificateSubject.CreateFromDistinguishedName(dn);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CertificateSubject_CreateFromDistinguishedName_WhitespaceDistinguishedName_ArgumentNullException()
        {
            string dn = " ";

            CertificateSubject.CreateFromDistinguishedName(dn);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CertificateSubject_CreateFromDistinguishedName_DistinguishedNameGreaterThan256Characters_ArgumentOutOfRangeException()
        {
            string dn = "CN=this is my really long subject name that must be created than 64 characters," +
                "OU=this is my really long organizational unit name that must be created than 64 characters" +
                "L=this is my really long city name that must be created than 64 characters" +
                "S=this is my really long state name that must be created than 64 characters" +
                "C=this is my really long country name that must be created than 64 characters";

            CertificateSubject.CreateFromDistinguishedName(dn);
        }

        [TestMethod]
        public void CertificateSubject_CreateFromDistinguishedName_CompleteDistinguishedNameSetsCommonNamePublicProperty()
        {
            string commonName = "domain.com";
            string city = "walpole";
            string country = "US";
            string state = "MA";
            string organization = "TechCorp";
            string department = "Engineering";

            string dn = string.Format("CN={0},OU={1},O={2},L={3},S={4},C={5}", commonName, department, organization, city, state, country);

            CertificateSubject subject = CertificateSubject.CreateFromDistinguishedName(dn);

            Assert.AreEqual(commonName, subject.CommonName);
        }

        [TestMethod]
        public void CertificateSubject_CreateFromDistinguishedName_CompleteDistinguishedNameSetsCityPublicProperty()
        {
            string commonName = "domain.com";
            string city = "walpole";
            string country = "US";
            string state = "MA";
            string organization = "TechCorp";
            string department = "Engineering";

            string dn = string.Format("CN={0},OU={1},O={2},L={3},S={4},C={5}", commonName, department, organization, city, state, country);

            CertificateSubject subject = CertificateSubject.CreateFromDistinguishedName(dn);

            Assert.AreEqual(city, subject.City);
        }

        [TestMethod]
        public void CertificateSubject_CreateFromDistinguishedName_CompleteDistinguishedNameSetsCountryPublicProperty()
        {
            string commonName = "domain.com";
            string city = "walpole";
            string country = "US";
            string state = "MA";
            string organization = "TechCorp";
            string department = "Engineering";

            string dn = string.Format("CN={0},OU={1},O={2},L={3},S={4},C={5}", commonName, department, organization, city, state, country);

            CertificateSubject subject = CertificateSubject.CreateFromDistinguishedName(dn);

            Assert.AreEqual(country, subject.Country);
        }

        [TestMethod]
        public void CertificateSubject_CreateFromDistinguishedName_CompleteDistinguishedNameSetsStatePublicProperty()
        {
            string commonName = "domain.com";
            string city = "walpole";
            string country = "US";
            string state = "MA";
            string organization = "TechCorp";
            string department = "Engineering";

            string dn = string.Format("CN={0},OU={1},O={2},L={3},S={4},C={5}", commonName, department, organization, city, state, country);

            CertificateSubject subject = CertificateSubject.CreateFromDistinguishedName(dn);

            Assert.AreEqual(state, subject.State);
        }

        [TestMethod]
        public void CertificateSubject_CreateFromDistinguishedName_CompleteDistinguishedNameSetsOrganizationPublicProperty()
        {
            string commonName = "domain.com";
            string city = "walpole";
            string country = "US";
            string state = "MA";
            string organization = "TechCorp";
            string department = "Engineering";

            string dn = string.Format("CN={0},OU={1},O={2},L={3},S={4},C={5}", commonName, department, organization, city, state, country);

            CertificateSubject subject = CertificateSubject.CreateFromDistinguishedName(dn);

            Assert.AreEqual(organization, subject.Organization);
        }

        [TestMethod]
        public void CertificateSubject_CreateFromDistinguishedName_CompleteDistinguishedNameSetsDepartmentPublicProperty()
        {
            string commonName = "domain.com";
            string city = "walpole";
            string country = "US";
            string state = "MA";
            string organization = "TechCorp";
            string department = "Engineering";

            string dn = string.Format("CN={0},OU={1},O={2},L={3},S={4},C={5}", commonName, department, organization, city, state, country);

            CertificateSubject subject = CertificateSubject.CreateFromDistinguishedName(dn);

            Assert.AreEqual(department, subject.Department);
        }

        [TestMethod]
        public void CertificateSubject_CreateFromDistinguishedName_CommonNameSetsCommonNamePublicProperty()
        {
            string commonName = "domain.com";
            string dn = string.Format("CN={0}", commonName);

            CertificateSubject subject = CertificateSubject.CreateFromDistinguishedName(dn);

            Assert.AreEqual(commonName, subject.CommonName);
        }

        [TestMethod]
        public void CertificateSubject_CreateFromDistinguishedName_DepartmentSetsDepartmentPublicProperty()
        {
            string department = "Engineering";
            string dn = string.Format("OU={0}", department);

            CertificateSubject subject = CertificateSubject.CreateFromDistinguishedName(dn);

            Assert.AreEqual(department, subject.Department);
        }

        [TestMethod]
        public void CertificateSubject_CreateFromDistinguishedName_OrganizationSetsOrganizationPublicProperty()
        {
            string organization = "TechCorp";
            string dn = string.Format("O={0}", organization);

            CertificateSubject subject = CertificateSubject.CreateFromDistinguishedName(dn);

            Assert.AreEqual(organization, subject.Organization);
        }

        [TestMethod]
        public void CertificateSubject_CreateFromDistinguishedName_CitySetsCityPublicProperty()
        {
            string city = "walpole";
            string dn = string.Format("L={0}", city);

            CertificateSubject subject = CertificateSubject.CreateFromDistinguishedName(dn);

            Assert.AreEqual(city, subject.City);
        }

        [TestMethod]
        public void CertificateSubject_CreateFromDistinguishedName_StateSetsStatePublicProperty()
        {
            string state = "ma";
            string dn = string.Format("S={0}", state);

            CertificateSubject subject = CertificateSubject.CreateFromDistinguishedName(dn);

            Assert.AreEqual(state, subject.State);
        }

        [TestMethod]
        public void CertificateSubject_CreateFromDistinguishedName_CountrySetsCountryPublicProperty()
        {
            string country = "US";
            string dn = string.Format("C={0}", country);

            CertificateSubject subject = CertificateSubject.CreateFromDistinguishedName(dn);

            Assert.AreEqual(country, subject.Country);
        }

        [TestMethod]
        public void CertificateSubject_ToString_CommonNameOnly_CreatesValidDistinguishedName()
        {
            string commonName = "domain.com";
            string expectedDn = String.Format("CN={0}", commonName);
            CertificateSubject subject = new CertificateSubject(commonName);

            string dn = subject.ToString();

            Assert.AreEqual(expectedDn, dn);
        }

        [TestMethod]
        public void CertificateSubject_ToString_CommonNameDepartmentOrganization_CreatesValidDistinguishedName()
        {
            string commonName = "domain.com";
            string department = "engineering";
            string organization = "microsoft";
            string expectedDn = String.Format("CN={0},OU={1},O={2}", commonName, department, organization);

            CertificateSubject subject = new CertificateSubject(commonName, department, organization);

            string dn = subject.ToString();

            Assert.AreEqual(expectedDn, dn);
        }

        [TestMethod]
        public void CertificateSubject_ToString_AllComponents_CreatesValidDistinguishedName()
        {
            string commonName = "domain.com";
            string department = "engineering";
            string organization = "microsoft";
            string city = "redmond";
            string state = "WA";
            string country = "US";

            string expectedDn = String.Format("CN={0},OU={1},O={2},L={3},S={4},C={5}", commonName, department, organization, city, state, country);

            CertificateSubject subject = new CertificateSubject(commonName, department, organization, city, state, country);

            string dn = subject.ToString();

            Assert.AreEqual(expectedDn, dn);
        }

        [TestMethod]
        public void CertificateSubject_ToString_FactoryCreateFromDistinguishedName_CreatesValidDistinguishedName()
        {
            string commonName = "domain.com";
            string department = "engineering";
            string organization = "microsoft";
            string city = "redmond";
            string state = "WA";
            string country = "US";

            string inputDn = String.Format("CN={0},OU={1},O={2},L={3},S={4},C={5}", commonName, department, organization, city, state, country);

            CertificateSubject subject = CertificateSubject.CreateFromDistinguishedName(inputDn);

            string resultDn = subject.ToString();

            Assert.AreEqual(inputDn, resultDn);
        }
    }
}
