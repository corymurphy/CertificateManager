using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    public class UpdateUserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> AlternativeNames { get; set; }
        public bool Enabled { get; set; }
        public bool LocalLogonEnabled { get; set; }
    }
}
