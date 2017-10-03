using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertificateServices.ActiveDirectory
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
