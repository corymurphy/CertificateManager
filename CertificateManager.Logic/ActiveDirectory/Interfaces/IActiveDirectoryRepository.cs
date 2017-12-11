using CertificateManager.Entities;
using System.Collections.Generic;

namespace CertificateManager.Logic.ActiveDirectory.Interfaces
{
    public interface IActiveDirectoryRepository
    {
        string GetBaseDistinguishedName(string domain);

        string GetLdapConnectionString(NamingContext namingContext, ActiveDirectoryMetadata metadata);

        List<T> Search<T>(NamingContext namingContext, ActiveDirectoryMetadata metadata);

        List<T> Search<T>(string searchKey, string searchValue, NamingContext namingContext, ActiveDirectoryMetadata metadata);

        T GetObject<T>(string schemaClass, string searchValue, ActiveDirectoryMetadata metadata, bool retry = false);

        //void SetContext()
    }
}
