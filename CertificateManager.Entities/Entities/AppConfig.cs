using System;

namespace CertificateManager.Entities
{
    public class AppConfig
    {
        public AppConfig()
        {
            this.JwtValidityPeriod = 5;
            this.LocalIdpIdentifier = "https://certificatemanager/idp";
            this.Id = new Guid();
        }
        public int JwtValidityPeriod { get; set; }
        public string LocalIdpIdentifier { get; set; }
        public Guid Id { get; set; }
    }
}
