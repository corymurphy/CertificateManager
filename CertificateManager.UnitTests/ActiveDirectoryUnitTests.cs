using CertificateServices.ActiveDirectory;
using CertificateServices.ActiveDirectory.Entities;
using CertificateServices.ActiveDirectory.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class ActiveDirectoryUnitTests
    {
        [TestMethod]
        public void ActiveDirectoryEntityMapperMetadataResolver_GetSchemaClass_PkiCertificateTemplateWithValidAttribute_ReturnsSchemaClassName()
        {
            string expected = ActiveDirectorySchemaClass.PkiCertificateTemplate;

            ActiveDirectoryEntityMapperMetadataResolver resolver = new ActiveDirectoryEntityMapperMetadataResolver();

            string actual = resolver.GetSchemaClass<AdcsCertificateTemplate>();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeDoesNotHaveEntitySchemaClassAttributeException))]
        public void ActiveDirectoryEntityMapperMetadataResolver_GetSchemaClass_TypeWithoutAttributeException_ThrowException()
        {
            ActiveDirectoryEntityMapperMetadataResolver resolver = new ActiveDirectoryEntityMapperMetadataResolver();

            string actual = resolver.GetSchemaClass<InvalidMetadataResolverTestClass>();
        }

        [TestMethod]
        [ExpectedException(typeof(TypeDoesNotHaveEntitySchemaClassAttributeException))]
        public void ActiveDirectoryEntityMapperMetadataResolver_GetSchemaClass_AttributeWithoutNameValue_ThrowException()
        {
            ActiveDirectoryEntityMapperMetadataResolver resolver = new ActiveDirectoryEntityMapperMetadataResolver();

            string actual = resolver.GetSchemaClass<EmptyAttributeNameMetadataResolverTestClass>();
        }

        [TestMethod]
        public void ActiveDirectoryEntityMapperMetadataResolver_GetPropertiesToLoad_ValidAttributeNameMetadataResolverTestClass_ReturnsProperties()
        {
            string expected = "ldapAttribute";

            ActiveDirectoryEntityMapperMetadataResolver resolver = new ActiveDirectoryEntityMapperMetadataResolver();

            string[] properties = resolver.GetPropertiesToLoad<ValidAttributeNameMetadataResolverTestClass>();

            CollectionAssert.Contains(properties, expected);
        }

        [TestMethod]
        [ExpectedException(typeof(TypeHasInvalidDirectoryAttributeException))]
        public void ActiveDirectoryEntityMapperMetadataResolver_GetPropertiesToLoad_EmptyAttribute_ThrowsArgumentNullException()
        {
            ActiveDirectoryEntityMapperMetadataResolver resolver = new ActiveDirectoryEntityMapperMetadataResolver();

            string[] properties = resolver.GetPropertiesToLoad<EmptyAttributeNameMetadataResolverTestClass>();
        }

        [TestMethod]
        [ExpectedException(typeof(TypeDoesNotHaveDirectoryAttributeException))]
        public void ActiveDirectoryEntityMapperMetadataResolver_GetPropertiesToLoad_NoAttribute_ThrowsArgumentNullException()
        {
            ActiveDirectoryEntityMapperMetadataResolver resolver = new ActiveDirectoryEntityMapperMetadataResolver();

            string[] properties = resolver.GetPropertiesToLoad<InvalidMetadataResolverTestClass>();
        }
    }

    public class InvalidMetadataResolverTestClass
    {
        public string Name { get; set; }
    }

    [EntitySchemaClass("")]
    public class EmptyAttributeNameMetadataResolverTestClass
    {
        [DirectoryAttributeMapping("")]
        public string Name { get; set; }
    }

    [EntitySchemaClass("schemaClass")]
    public class ValidAttributeNameMetadataResolverTestClass
    {
        [DirectoryAttributeMapping("ldapAttribute")]
        public string Name { get; set; }
    }
}
