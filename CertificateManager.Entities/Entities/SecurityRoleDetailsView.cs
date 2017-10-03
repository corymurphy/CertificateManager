using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    public class SecurityRoleDetailsView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<AuthenticablePrincipal> Members { get; set; }
    }
}
