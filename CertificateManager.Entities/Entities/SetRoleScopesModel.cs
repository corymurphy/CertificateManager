using CertificateManager.Entities.Extensions;
using CertificateManager.Entities.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    public class SetRoleScopesModel : ILoggableEntity
    {
        public Guid RoleId { get; set; }
        public Guid Id { get; set; }
        public List<Guid> Scopes { get; set; }

        public string GetDescription()
        {
            string scopes = JsonConvert.SerializeObject(Scopes, Formatting.Indented);
            return string.Format("Attempting to add the following scopes to role id {0} {1} {2}", RoleId, Environment.NewLine, scopes);
        }

        public string GetId()
        {
            return RoleId.GetId();
        }
    }
}
