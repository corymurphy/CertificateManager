using CertificateManager.Entities.Interfaces;
using System;
using System.Collections.Generic;

namespace CertificateManager.Logic
{
    public class SecurityPrincipalLogic
    {
        UserManagementLogic userManagementLogic;
        RoleManagementLogic roleManagementLogic;

        public SecurityPrincipalLogic(RoleManagementLogic roleManagementLogic, UserManagementLogic userManagementLogic)
        {
            this.roleManagementLogic = roleManagementLogic;
            this.userManagementLogic = userManagementLogic;
        }

        public List<ISecurityPrincipal> GetSecurityPrincipals()
        {
            List<ISecurityPrincipal> principals = new List<ISecurityPrincipal>();

            principals.AddRange(roleManagementLogic.GetRoles());
            principals.AddRange(userManagementLogic.GetUsers());

            return principals;
        }

        public string ResolveSecurityPrincipalDisplayName(string identity)
        {
            if(string.IsNullOrWhiteSpace(identity))
                return "Unresolved name";

            Guid id = new Guid();

            if (!Guid.TryParse(identity, out id))
                return identity;

            ISecurityPrincipal securityPrincipal = null;

            try
            {
                securityPrincipal = roleManagementLogic.GetRole(id);

                if (securityPrincipal != null && !string.IsNullOrEmpty(securityPrincipal.Name))
                    return securityPrincipal.Name;
            }
            catch
            {

            }

            try
            {
                securityPrincipal = userManagementLogic.GetUser(id);

                if (securityPrincipal != null && !string.IsNullOrEmpty(securityPrincipal.Name))
                    return securityPrincipal.Name;
            }
            catch
            {

            }

            return "Unresolved name";
        }
    }
}
