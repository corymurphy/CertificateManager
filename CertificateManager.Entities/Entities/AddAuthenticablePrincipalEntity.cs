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
            this.UserPrincipalName = entity.UserPrincipalName;
            this.LocalLogonEnabled = entity.LocalLogonEnabled;
            this.Enabled = entity.Enabled;
            this.AlternativeUserPrincipalNames = entity.AlternativeUserPrincipalNames;
        }

        public Guid Id { get; set; }
        public string UserPrincipalName { get; set; }
        public List<string> AlternativeUserPrincipalNames { get; set; }
        public bool Enabled { get; set; }
        public bool LocalLogonEnabled { get; set; }
    }
}
