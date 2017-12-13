using System;
using System.Security.Claims;

namespace CertificateManager.Entities.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            Claim claim = claimsPrincipal.FindFirst(WellKnownClaim.Uid);

            if (claim == null || string.IsNullOrEmpty(claim.Value))
            {
                throw new Exception("Current user context does not have a uid claim");
            }

            
            Guid uid = new Guid();
            bool result = Guid.TryParse(claim.Value, out uid);

            if(result)
            {
                return uid;
            }
            else
            {
                throw new Exception(string.Format("Current user context does not have a valid UID claim - {0}", claim.Value));
            }
        }

        public static string GetName(this ClaimsPrincipal claimsPrincipal)
        {
            Claim claim = claimsPrincipal.FindFirst(WellKnownClaim.Name);

            if(claim == null || string.IsNullOrEmpty(claim.Value))
            {
                throw new Exception("Current user context does not have a name claim");
            }

            return claim.Value;
        }
    }
}
