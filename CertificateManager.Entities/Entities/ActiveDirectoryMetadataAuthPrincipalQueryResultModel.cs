using System;

namespace CertificateManager.Entities
{
    public class ActiveDirectoryMetadataAuthPrincipalQueryResultModel
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string SamAccountName { get; set; }
        public string UserPrincipalName { get; set; }
        public string Domain { get; set; }
        public Guid DomainId { get; set; }
        public string Name { get; set; }
    }
}
