using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CertificateManager.Entities
{
    public static class AuthorizationScopes
    {
        private const string manageRolesScope = "52fd4b83-38e6-4905-98b0-eba4977437e7";
        private const string manageUsersScope = "b70db756-6d0a-4d69-8784-733fbb243594";
        private const string manageIdentityProvidersScope = "9c7c5e85-264e-49d2-9ddf-07065a0f90c2";
        private const string manageCertificateAuthoritiesScope = "dcc20573-4969-4d59-966a-d945657a5888";
        private const string manageAdcsTemplatesScope = "a8ce22d2-98c6-44bc-9379-ada9910ac0ca";
        private const string certificateFullControlScope = "266cea37-be5d-482e-9723-7cbf54777feb";
        private const string issuePendingCertificatesScope = "b517d658-e884-4bcb-a599-73a8f7f986c4";
        private const string deleteCertificatesScope = "02803f7c-5cf3-4bb4-8f57-1dc940035f5b";

        public static Guid ManageUsers { get { return new Guid(manageUsersScope); } }

        public static Guid ManageRoles { get { return new Guid(manageRolesScope); } }

        public static Guid ManageIdentityProviders { get { return new Guid(manageIdentityProvidersScope); } }

        public static Guid ManageCertificateAuthorities { get { return new Guid(manageCertificateAuthoritiesScope); } }

        public static Guid ManageAdcsTemplates { get { return new Guid(manageAdcsTemplatesScope); } }

        public static Guid CertificateFullControl { get { return new Guid(certificateFullControlScope); } }

        public static Guid IssuePendingCertificates { get { return new Guid(issuePendingCertificatesScope); } }

        public static Guid DeleteCertificates { get { return new Guid(deleteCertificatesScope); } }

        public static List<Scope> InitialScopes { get { return GetInitialScopes(); } }

        private static List<Scope> GetInitialScopes()
        {
            List<Guid> scopeIds = new List<Guid>();
            List<Scope> scopes = new List<Scope>();

            foreach(PropertyInfo prop in GetScopesFromProperties())
            {
                scopes.Add(new Scope()
                {
                    Name = prop.Name,
                    Id = (Guid)(prop.GetValue(null, null))
                });

            }
            
            return scopes;
        }

        public static Scope GetScope(Guid id)
        {
            return GetInitialScopes().Where(scope => scope.Id == id).First();
        }

        private static PropertyInfo[] GetScopesFromProperties()
        {
            return (typeof(AuthorizationScopes)).GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(prop => prop.PropertyType == typeof(Guid))
                .ToArray();
        }
    }
}
