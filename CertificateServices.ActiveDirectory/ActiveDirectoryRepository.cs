using System.Collections.Generic;
using System.DirectoryServices;

namespace CertificateServices.ActiveDirectory
{
    public class ActiveDirectoryRepository
    {
        private bool basicAuth;
        private string username;
        private string password;
        private string domain;
        private string server;
        private string baseDn;

        private ActiveDirectoryEntityMapperMetadataResolver metadataResolver;
        public ActiveDirectoryRepository(string domain, string server, string username, string password)
        {
            basicAuth = true;
            this.username = username;
            this.password = password;
            this.domain = domain;
            this.server = server;
            this.metadataResolver = new ActiveDirectoryEntityMapperMetadataResolver();
            this.baseDn = GetBaseDistinguishedName(domain);
        }

        public string GetBaseDistinguishedName(string domain)
        {
            string[] domainArray = domain.Split('.');

            string baseDn = string.Empty;

            foreach(string domainPart in domainArray)
            {
                if (string.IsNullOrEmpty(baseDn))
                    baseDn = string.Format("DC={0}", domainPart);
                else
                    baseDn = baseDn + string.Format(",DC={0}", domainPart);
            }

            return baseDn;
        }

        public string GetLdapConnectionString(NamingContext namingContext)
        {
            switch(namingContext)
            {
                case NamingContext.Configuration:
                    return string.Format("LDAP://{0}/CN=Configuration,{1}", server, baseDn);
                default:
                    return string.Format("LDAP://{0}/{1}", server, baseDn);
            }
        }

        public List<T> Search<T>(NamingContext namingContext)
        {
            List<T> list = new List<T>();

            string searchFilter = string.Format("(objectClass={0})", metadataResolver.GetSchemaClass<T>());

            using (DirectoryEntry entry = new DirectoryEntry(GetLdapConnectionString(namingContext), username, password))
            {
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = searchFilter;
                    var propertiesToLoad = metadataResolver.GetPropertiesToLoad<T>();
                    searcher.PropertiesToLoad.AddRange(propertiesToLoad);
                    foreach (SearchResult result in searcher.FindAll())
                    {
                        list.Add(ActiveDirectoryEntityMapper.MapSearchResult<T>(result));
                    }
                }
            }
            return list;
        }

        public List<T> Search<T>(string searchKey, string searchValue, NamingContext namingContext)
        {
            List<T> list = new List<T>();

            string searchFilter = string.Format("(&(objectClass={0})({1}={2}))", metadataResolver.GetSchemaClass<T>(), searchKey, searchValue);

            string ldapConnectionString = GetLdapConnectionString(namingContext);

            using (DirectoryEntry entry = new DirectoryEntry(ldapConnectionString, username, password))
            {
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = searchFilter;
                    searcher.PropertiesToLoad.AddRange(metadataResolver.GetPropertiesToLoad<T>());
                    foreach(SearchResult result in searcher.FindAll())
                    {
                        list.Add(ActiveDirectoryEntityMapper.MapSearchResult<T>(result));
                    }

                }
            }
            return list;
        }

        public T GetObject<T>(string schemaClass, string searchValue, bool retry = false)
        {
            return default(T);
        }

        private T GetObject<T>(string ldapBind)
        {
            using (DirectoryEntry entry = new DirectoryEntry(ldapBind))
            {
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.PropertiesToLoad.AddRange(metadataResolver.GetPropertiesToLoad<T>());
                    return ActiveDirectoryEntityMapper.MapSearchResult<T>(searcher.FindOne());
                }
            }
        }
    }
}
