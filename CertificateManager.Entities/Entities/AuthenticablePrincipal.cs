using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    public class AuthenticablePrincipal
    {
        public Guid Id { get; set; }
        public string UserPrincipalName { get; set; }
        public List<string> AlternativeUserPrincipalNames { get; set; }
        public DateTime LastLogonDate { get; set; }
        public bool Enabled { get; set; }
        public string LastLogonRealm { get; set; }
        public bool LocalLogonEnabled { get; set; }
        public string PasswordHash { get; set; }
        //public byte[] PasswordSalt { get; set; }
        //public byte[] PasswordHash { get; set; }
    }
}
