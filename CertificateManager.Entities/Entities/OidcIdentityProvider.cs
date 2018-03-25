using CertificateManager.Entities.Interfaces;
using System;

namespace CertificateManager.Entities
{
    public class OidcIdentityProvider : ILoggableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Authority { get; set; }
        
        public string GetDescription()
        {
            return "OpenId Connect Identity Provider";
        }

        public string GetId()
        {
            return this.Id.ToString();
        }
    }
}
