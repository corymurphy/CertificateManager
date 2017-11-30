using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    public class AddAuthenticablePrincipalEntity
    {
        public AddAuthenticablePrincipalEntity() { }

        public AddAuthenticablePrincipalEntity(AuthenticablePrincipal entity)
        {
            this.Id = entity.Id;
            this.Name = entity.Name;
            this.LocalLogonEnabled = entity.LocalLogonEnabled;
            this.Enabled = entity.Enabled;
            this.AlternativeNames = entity.AlternativeNames;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> AlternativeNames { get; set; }
        public bool Enabled { get; set; }
        public bool LocalLogonEnabled { get; set; }
    }
}
