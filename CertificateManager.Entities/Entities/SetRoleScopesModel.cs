using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    public class SetRoleScopesModel
    {
        public Guid Id { get; set; }
        public List<Guid> Scopes { get; set; }
    }
}
