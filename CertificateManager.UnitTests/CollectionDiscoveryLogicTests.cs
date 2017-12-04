using CertificateManager.Entities.Exceptions;
using CertificateManager.Repository;
using CertificateManager.UnitTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class CollectionDiscoveryLogicTests
    {
        private const string validName = "TestRepo";
        private const string className = "TestClassWithoutAttribute";

        [TestMethod]
        public void CollectionDiscoveryLogic_GetName_ReturnsExpectedName()
        {
            CollectionDiscoveryLogic collectionDiscoveryLogic = new CollectionDiscoveryLogic();

            string name = collectionDiscoveryLogic.GetName<TestClassWithAttribute>();

            Assert.AreEqual(validName, name);
        }

        [TestMethod]
        public void CollectionDiscoveryLogic_GetName_NoAttribute_ReturnsClassName()
        {
            CollectionDiscoveryLogic collectionDiscoveryLogic = new CollectionDiscoveryLogic();

            string name = collectionDiscoveryLogic.GetName<TestClassWithoutAttribute>();

            Assert.AreEqual(className, name);
        }

        [TestMethod]
        [ExpectedException(typeof(RepositoryAttributeIsInvalidException))]
        public void CollectionDiscoveryLogic_GetName_AttributeExistsButIsEmpty_ThrowsRepositoryAttributeIsInvalidException()
        {
            CollectionDiscoveryLogic collectionDiscoveryLogic = new CollectionDiscoveryLogic();

            string name = collectionDiscoveryLogic.GetName<TestClassWithAttributeNameIsEmpty>();
        }

        [TestMethod]
        [ExpectedException(typeof(RepositoryAttributeIsInvalidException))]
        public void CollectionDiscoveryLogic_GetName_AttributeExistsButLengthIsGreaterThan20_ThrowsRepositoryAttributeIsInvalidException()
        {
            CollectionDiscoveryLogic collectionDiscoveryLogic = new CollectionDiscoveryLogic();

            string name = collectionDiscoveryLogic.GetName<TestClassWithAttributeNameLengthGreaterThan20>();
        }
    }
}
