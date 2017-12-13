using System;

namespace CertificateManager.Entities.Extensions
{
    public static class GuidExtensions
    {
        public static string GetId(this Guid id)
        {
            if(id.ValidId())
            {
                return id.ToString();
            }
            else
            {
                return "invalid id";
            }
        }

        public static bool ValidId(this Guid id)
        {
            if(id == null || id.ToString() == (new Guid().ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
