using CertificateManager.Entities;
using CertificateManager.Entities.Exceptions;
using CertificateManager.Logic;
using CertificateManager.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace CertificateManager.UnitTests
{
    [TestClass]
    public class NodeLogicTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NodeLogic_Get_NullId_ArgumentNullException()
        {
            NodeLogic nodeLogic = new NodeLogic(null, null, null, null, null, null);

            string id = null;

            nodeLogic.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NodeLogic_NodeIdInvalidGuid_ArgumentOutOfRangeException()
        {
            NodeLogic nodeLogic = new NodeLogic(null, null, null, null, null, null);

            string id = "zzz";

            nodeLogic.Get(id);
        }

        [TestMethod]
        [ExpectedException(typeof(ReferencedObjectDoesNotExistException))]
        public void NodeLogic_ConfigurationRepository_ReturnsNull()
        {
            string id = "27904fa2-fdde-4bde-942a-11628a2a48e5";
            Guid idGuid = Guid.Parse(id);

            Mock<IConfigurationRepository> configRepo = new Mock<IConfigurationRepository>();
            configRepo.Setup(m => m.Get<NodeDetails>(It.IsAny<Guid>())).Returns((NodeDetails)null);

            NodeLogic nodeLogic = new NodeLogic(configRepo.Object, null, null, null, null, null);

            nodeLogic.Get(id);
        }
    }
}
