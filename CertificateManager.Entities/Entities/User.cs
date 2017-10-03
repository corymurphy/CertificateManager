using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserPrincipalName { get; set; }
        public List<string> AlternativeUserPrincipalName { get; set; }
    }
}
