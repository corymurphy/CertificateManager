using System;

namespace CertificateManager.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntitySchemaClassAttribute : Attribute
    {
        public string Name { get; private set; }

        public EntitySchemaClassAttribute(string name)
        {
            this.Name = name;
        }
    }
}
