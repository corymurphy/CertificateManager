using CertificateManager.Entities.Extensions;
using CertificateManager.Entities.Interfaces;
using System;

namespace CertificateManager.Entities.Models
{
    public class DeleteSecurityRoleMemberModel : ILoggableEntity
    {
        public Guid RoleId { get; set; }
        public Guid MemberId { get; set; }

        public string GetDescription()
        {
            if(RoleId.ValidId() && MemberId.ValidId())
            {
                return string.Format("Attempting to delete member {0} from role {1}", MemberId, RoleId);
            }
            else
            {
                return "Attempting to delete a role member with an invalid member id or invalid role id";
            }
        }

        public string GetId()
        {
            return RoleId.GetId();
        }
    }
}
