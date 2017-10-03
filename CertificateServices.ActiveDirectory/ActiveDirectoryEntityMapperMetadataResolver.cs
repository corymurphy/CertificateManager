using CertificateServices.ActiveDirectory.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CertificateServices.ActiveDirectory
{
    public class ActiveDirectoryEntityMapperMetadataResolver
    {
        public string GetSchemaClass<T>()
        {
            EntitySchemaClassAttribute attribute = typeof(T).GetCustomAttribute<EntitySchemaClassAttribute>();

            if (attribute == null || string.IsNullOrWhiteSpace(attribute.Name))
                throw new TypeDoesNotHaveEntitySchemaClassAttributeException("EntitySchemaClassAttribute must have a name specified to map this type");
            else
                return attribute.Name;
        }

        public string[] GetPropertiesToLoad<T>()
        {
            List<string> propertiesToLoad = new List<string>();

            PropertyInfo[] propertiesWithDirectoryAttributes =
               typeof(T).GetProperties()
                   .Where(prop => prop.GetCustomAttributes<DirectoryAttributeMapping>(false)
                   .Any()).ToArray();

            if (propertiesWithDirectoryAttributes == null || propertiesWithDirectoryAttributes.Length < 1)
                throw new TypeDoesNotHaveDirectoryAttributeException("Automapped active directory entities must have the DirectoryAttributeMapping attribute on at least one property field");

            foreach (PropertyInfo property in propertiesWithDirectoryAttributes)
            {
                DirectoryAttributeMapping attribute = property.GetCustomAttribute<DirectoryAttributeMapping>();

                if (attribute == null || string.IsNullOrWhiteSpace(attribute.Name))
                    throw new TypeHasInvalidDirectoryAttributeException("Types with DirectoryAttributeMapping must have a valid active directory attribute provided via the name");

                propertiesToLoad.Add(attribute.Name);
            }

            return propertiesToLoad.ToArray();
        }
    }
}
