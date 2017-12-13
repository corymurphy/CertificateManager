using CertificateManager.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CertificateManager.Entities
{
    public class UpdateUserModel : ILoggableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> AlternativeNames { get; set; }
        public bool Enabled { get; set; }
        public bool LocalLogonEnabled { get; set; }

        public string GetDescription()
        {
            return string.Format("Set user with values - Name:{0}; Enabled: {1}; LocalLogonEnabled: {2}; HasAlternativeNames: {3}", Name, Enabled, LocalLogonEnabled, AlternativeNames.Any());
        }

        public string GetId()
        {
            return this.Id.ToString();
        }
    }
}
