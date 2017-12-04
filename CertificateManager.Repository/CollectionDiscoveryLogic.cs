using CertificateManager.Entities.Attributes;
using CertificateManager.Entities.Exceptions;
using System.Linq;

namespace CertificateManager.Repository
{
    public class CollectionDiscoveryLogic
    {
        public string GetName<T>()
        {
            RepositoryAttribute attribute = typeof(T).GetCustomAttributes(typeof(RepositoryAttribute), false).FirstOrDefault() as RepositoryAttribute;

            if (attribute == null)
            {
                return typeof(T).Name;
            }
            else
            {
                if(string.IsNullOrWhiteSpace(attribute.Name) || attribute.Name.Length > 20)
                {
                    throw new RepositoryAttributeIsInvalidException();
                }
                else
                {
                    return attribute.Name;
                }
            }
        }
    }
}
