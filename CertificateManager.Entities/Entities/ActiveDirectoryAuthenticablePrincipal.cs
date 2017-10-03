using CertificateServices.ActiveDirectory;
using System;

namespace CertificateManager.Entities
{
    [EntitySchemaClassAttribute("user")]
    public class ActiveDirectoryAuthenticablePrincipal
    {
        [DirectoryAttributeMapping("displayName")]
        public string DisplayName { get; set; }

        [DirectoryAttributeMapping("userPrincipalName")]
        public string UserPrincipalName { get; set; }

        [DirectoryAttributeMapping("samAccountName")]
        public string SamAccountName { get; set; }

        [DirectoryAttributeMapping("objectGuid")]
        public Guid Id { get; set; }

        [DirectoryAttributeMapping("name")]
        public string Name { get; set; }
    }
}
