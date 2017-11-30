using System;

namespace CertificateManager.Entities
{
    public static class AuthorizationScopes
    {
        private const string manageRolesScopeId = "52fd4b83-38e6-4905-98b0-eba4977437e7";

        public static Guid ManageRolesScope { get { return new Guid(manageRolesScopeId); } }
    }
}
