using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    public class SecurityRoleSelectView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Display { get { return this.Name; } }
    }
}
