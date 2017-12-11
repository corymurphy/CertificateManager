using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    public class ImportUsersActiveDirectoryMetadataModel
    {
        public List<ActiveDirectoryMetadataAuthPrincipalQueryResultModel> Users { get; set; }
        public Guid? MergeWith { get; set; }
        public bool Merge { get; set; }
    }
}
