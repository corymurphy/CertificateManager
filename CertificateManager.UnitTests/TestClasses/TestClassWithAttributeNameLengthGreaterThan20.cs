using CertificateManager.Entities.Attributes;

namespace CertificateManager.UnitTests.TestClasses
{
    [Repository("ThisClassAttributeNameIsGreaterThan20Characters")]
    public class TestClassWithAttributeNameLengthGreaterThan20
    {
        public string Name { get; set; }
    }
}
