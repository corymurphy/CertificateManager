using System;

namespace CertificateManager.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DirectoryAttributeMapping : Attribute
    {
        public string Name { get; private set; }

        public DirectoryAttributeMapping(string name)
        {
            this.Name = name;
        }
    }
}
