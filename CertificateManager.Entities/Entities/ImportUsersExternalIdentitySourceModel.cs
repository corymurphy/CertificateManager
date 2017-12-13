using CertificateManager.Entities.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CertificateManager.Entities
{
    public class ImportUsersActiveDirectoryMetadataModel : ILoggableEntity
    {
        public List<ActiveDirectoryMetadataAuthPrincipalQueryResultModel> Users { get; set; }
        public Guid? MergeWith { get; set; }
        public bool Merge { get; set; }

        public string GetDescription()
        {
            string msg = string.Empty;

            if(Users == null || !Users.Any())
            {
                msg = "No users specified for import";
            }
            else
            {
                msg = JsonConvert.SerializeObject(this.Users);
            }

            return msg;
        }

        public string GetId()
        {
            if(MergeWith != null)
            {
                return MergeWith.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
