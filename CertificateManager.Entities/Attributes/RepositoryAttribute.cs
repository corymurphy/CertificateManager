using System;

namespace CertificateManager.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RepositoryAttribute : Attribute
    {
        public string Name { get; private set; }

        public RepositoryAttribute(string name)
        {
            this.Name = name;
        }
    }
}
