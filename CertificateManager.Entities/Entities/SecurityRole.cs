using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    public class SecurityRole
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<Guid> Member { get; set; }
    }
}
