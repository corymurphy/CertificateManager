using CertificateManager.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Reflection;

namespace CertificateManager.Logic.ActiveDirectory
{
    public static class ActiveDirectoryEntityMapper
    {

        private const long fileTimeMaxValue = 2650467743999999999;


        public static List<T> MapSearchResult<T>(SearchResultCollection results)
        {

            List<T> resultsList = new List<T>();
            Dictionary<string, string> attributeMap = GetDirectoryAttributeMap<T>();

            foreach (SearchResult result in results)
            {
                var entity = (T)Activator.CreateInstance(typeof(T));

                foreach (KeyValuePair<string, string> map in attributeMap)
                {
                    if (entity.GetType().GetProperty(map.Value).PropertyType == typeof(String))
                    {
                        typeof(T).GetProperty(map.Value).SetValue(entity, ResolveStandardProperty(result.Properties[map.Key]));
                        continue;
                    }

                    if (entity.GetType().GetProperty(map.Value).PropertyType == typeof(Guid))
                    {
                        typeof(T).GetProperty(map.Value).SetValue(entity, new Guid(result.Properties[map.Key][0] as byte[]));
                        continue;
                    }


                    if (entity.GetType().GetProperty(map.Value).PropertyType == typeof(DateTime))
                    {
                        typeof(T).GetProperty(map.Value).SetValue(entity, ResolveDateTimeAttribute(result.Properties[map.Key]));
                        continue;
                    }

                    if (entity.GetType().GetProperty(map.Value).PropertyType == typeof(byte[][]))
                    {
                        typeof(T).GetProperty(map.Value).SetValue(entity, ResolveNestedByteArrayAttribute(result.Properties[map.Key]));
                        continue;
                    }

                    if (entity.GetType().GetProperty(map.Value).PropertyType == typeof(string[]))
                    {
                        typeof(T).GetProperty(map.Value).SetValue(entity, ResolveStringArrayAttribute(result.Properties[map.Key]));
                        continue;
                    }

                    if (entity.GetType().GetProperty(map.Value).PropertyType == typeof(Int32))
                    {
                        typeof(T).GetProperty(map.Value).SetValue(entity, ResolveInt32Attribute(result.Properties[map.Key]));
                        continue;
                    }

                }

                resultsList.Add(entity);
            }
            return resultsList;
        }

        public static T MapSearchResult<T>(SearchResult result)
        {
            Dictionary<string, string> attributeMap = GetDirectoryAttributeMap<T>();

            T entity = (T)Activator.CreateInstance(typeof(T));

            foreach (KeyValuePair<string, string> map in attributeMap)
            {
                if (entity.GetType().GetProperty(map.Value).PropertyType == typeof(String))
                {
                    typeof(T).GetProperty(map.Value).SetValue(entity, ResolveStandardProperty(result.Properties[map.Key]));
                    continue;
                }

                if (entity.GetType().GetProperty(map.Value).PropertyType == typeof(Guid))
                {
                    typeof(T).GetProperty(map.Value).SetValue(entity, new Guid(result.Properties[map.Key][0] as byte[]));
                    continue;
                }


                if (entity.GetType().GetProperty(map.Value).PropertyType == typeof(DateTime))
                {
                    typeof(T).GetProperty(map.Value).SetValue(entity, ResolveDateTimeAttribute(result.Properties[map.Key]));
                    continue;
                }

                if (entity.GetType().GetProperty(map.Value).PropertyType == typeof(Boolean))
                {
                    typeof(T).GetProperty(map.Value).SetValue(entity, ResolveBooleanAttribute(result.Properties[map.Key]));
                    continue;
                }

                if (entity.GetType().GetProperty(map.Value).PropertyType == typeof(byte[][]))
                {
                    typeof(T).GetProperty(map.Value).SetValue(entity, ResolveNestedByteArrayAttribute(result.Properties[map.Key]));
                    continue;
                }

                if (entity.GetType().GetProperty(map.Value).PropertyType == typeof(string[]))
                {
                    typeof(T).GetProperty(map.Value).SetValue(entity, ResolveStringArrayAttribute(result.Properties[map.Key]));
                    continue;
                }

                if (entity.GetType().GetProperty(map.Value).PropertyType == typeof(Int32))
                {
                    typeof(T).GetProperty(map.Value).SetValue(entity, ResolveInt32Attribute(result.Properties[map.Key]));
                    continue;
                }

            }
            return entity;
        }



        public static Dictionary<string, string> GetDirectoryAttributeMap<T>()
        {
            Dictionary<string, string> attributeMap = new Dictionary<string, string>();

            PropertyInfo[] fieldsWithPropertiesToLoad =
                typeof(T).GetProperties()
                    .Where(prop => prop.GetCustomAttributes(false)
                    .Any()).ToArray();

            foreach (PropertyInfo property in fieldsWithPropertiesToLoad)
            {
                attributeMap.Add(
                    ((DirectoryAttributeMapping)Attribute.GetCustomAttribute(property, typeof(DirectoryAttributeMapping))).Name,
                    property.Name);
            }

            return attributeMap;
        }

        private static string ResolveStandardProperty(PropertyValueCollection property)
        {
            if (property == null)
                return string.Empty;

            try
            {
                if (string.IsNullOrEmpty(property[0] as string))
                    return string.Empty;

                if (property.Count > 0)
                    return property[0] as string;
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;

        }

        private static Int32 ResolveInt32Attribute(PropertyValueCollection result)
        {
            if (result == null)
                return 0;
            if (result.Count > 0)
                return Int32.Parse(result[0].ToString());

            return 0;
        }

        private static DateTime ResolveDateTimeAttribute(PropertyValueCollection result)
        {
            if (result == null)
                return DateTime.MaxValue;

            long dateLong = 0;

            if (result[0] is DateTime)
                return (DateTime)result[0];
            else
                if (long.TryParse(((Int64)result[0]).ToString(), out dateLong))
            {
                if (dateLong > fileTimeMaxValue)
                    DateTime.FromFileTime(fileTimeMaxValue);
                else
                    return DateTime.FromFileTime(dateLong);
            }

            return DateTime.FromFileTime(0);
        }

        private static bool ResolveBooleanAttribute(PropertyValueCollection result)
        {
            if (result == null)
                return false;

            if (result.Count > 0)
                return Boolean.Parse(result[0].ToString());

            return false;
        }

        private static byte[][] ResolveNestedByteArrayAttribute(PropertyValueCollection result)
        {
            List<byte[]> certByteList = new List<byte[]>();


            if (result == null)
                return certByteList.ToArray();

            if (result.Count > 0)
            {
                foreach (byte[] certByte in result)
                {
                    certByteList.Add(certByte);
                }

                return certByteList.ToArray();
            }

            return certByteList.ToArray();
        }

        private static byte[] ResolveByteArrayAttribute(PropertyValueCollection result)
        {
            //result.Properties["userCertificate"]
            if (result == null)
                return new byte[0];

            List<byte> certByteList = new List<byte>();
            if (result.Count > 0)
            {
                foreach (byte certByte in result)
                {
                    certByteList.Add(certByte);
                }

                return certByteList.ToArray();
            }


            return new byte[0];
        }

        private static string[] ResolveStringArrayAttribute(PropertyValueCollection result)
        {
            if (result == null)
                return new string[0];

            List<string> resultList = new List<string>();
            if (result.Count > 0)
            {
                foreach (string item in result)
                {
                    resultList.Add(item);
                }

                return resultList.ToArray();
            }


            return new string[0];
        }






        private static string ResolveStandardProperty(ResultPropertyValueCollection property)
        {
            if (property == null)
                return string.Empty;

            try
            {
                if (property.Count > 0)
                    return property[0].ToString();
            }
            catch
            {
                return string.Empty;
            }
            return string.Empty;
        }

        private static Int32 ResolveInt32Attribute(ResultPropertyValueCollection property)
        {
            if (property == null)
                return 0;
            if (property.Count > 0)
                return Int32.Parse(property[0].ToString());

            return 0;
        }

        private static DateTime ResolveDateTimeAttribute(ResultPropertyValueCollection result)
        {
            if (result == null)
                return DateTime.MaxValue;

            long dateLong = 0;

            if (result[0] is DateTime)
                return (DateTime)result[0];
            else
                if (long.TryParse(((Int64)result[0]).ToString(), out dateLong))
            {
                if (dateLong > fileTimeMaxValue)
                    DateTime.FromFileTime(fileTimeMaxValue);
                else
                    return DateTime.FromFileTime(dateLong);
            }

            return DateTime.FromFileTime(0);
        }

        private static bool ResolveBooleanAttribute(ResultPropertyValueCollection result)
        {
            if (result == null)
                return false;

            if (result.Count > 0)
                return Boolean.Parse(result[0].ToString());

            return false;
        }

        private static byte[] ResolveByteArrayAttribute(ResultPropertyValueCollection result)
        {
            //result.Properties["userCertificate"]
            if (result == null)
                return new byte[0];

            List<byte> certByteList = new List<byte>();
            if (result.Count > 0)
            {
                foreach (byte certByte in result)
                {
                    certByteList.Add(certByte);
                }

                return certByteList.ToArray();
            }


            return new byte[0];
        }

        private static byte[][] ResolveNestedByteArrayAttribute(ResultPropertyValueCollection result)
        {
            List<byte[]> certByteList = new List<byte[]>();


            if (result == null)
                return certByteList.ToArray();

            if (result.Count > 0)
            {
                foreach (byte[] certByte in result)
                {
                    certByteList.Add(certByte);
                }

                return certByteList.ToArray();
            }

            return certByteList.ToArray();
        }

        private static string[] ResolveStringArrayAttribute(ResultPropertyValueCollection result)
        {
            if (result == null)
                return new string[0];

            List<string> resultList = new List<string>();
            if (result.Count > 0)
            {
                foreach (string item in result)
                {
                    resultList.Add(item);
                }

                return resultList.ToArray();
            }


            return new string[0];
        }
    }
}
