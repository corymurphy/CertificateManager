using System;
using System.Collections.Generic;

namespace CertificateManager.Entities
{
    public class ImportUsersExternalIdentitySourceModel
    {
        public List<ExternalIdentitySourceAuthPrincipalQueryResultModel> Users { get; set; }
        public Guid MergeWith { get; set; }
        public bool Merge { get; set; }
    }
}
