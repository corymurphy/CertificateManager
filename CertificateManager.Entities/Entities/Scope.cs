using System;

namespace CertificateManager.Entities
{
    public class Scope
    {
        public Scope() { }

        public Scope(string name, Guid id)
        {
            this.Name = name;
            this.Id = id;
        }


        public string Name { get; set; }
        public Guid Id { get; set; }

    }
}
