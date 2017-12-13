using CertificateManager.Entities.Interfaces;
using System;
using System.Collections.Generic;
using CertificateManager.Entities.Enumerations;
using LiteDB;
using CertificateManager.Entities.Attributes;
using System.Linq;
using Newtonsoft.Json;

namespace CertificateManager.Entities
{
    [Repository("secroles")]
    public class SecurityRole : ISecurityPrincipal, ILoggableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<Guid> Member { get; set; }
        public List<Guid> Scopes { get; set; }

        [BsonIgnore]
        public IdentityType IdentityType { get { return IdentityType.Role; } }

        public string GetDescription()
        {
            string msg = string.Empty;

            if(Member == null || !Member.Any())
            {
                msg = "Attempting to import role with no members";
            }
            else
            {
                string member = JsonConvert.SerializeObject(Member);
                msg = string.Format("Importing role '{0}' {1} with {2} member(s) {3} {4}", GetId(), Id, Member.Count(), Environment.NewLine, member);
            }
            return msg;
        }

        public string GetId()
        {
            if(string.IsNullOrEmpty(Name))
            {

                return "Role name invalid";
            }
            else
            {
                return Name;
            }
        }
    }
}
