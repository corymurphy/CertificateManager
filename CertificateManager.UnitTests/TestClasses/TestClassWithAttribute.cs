using CertificateManager.Entities.Attributes;

namespace CertificateManager.UnitTests.TestClasses
{
    [Repository("TestRepo")]
    public class TestClassWithAttribute
    {
        public string Name { get; set; }
    }
}
