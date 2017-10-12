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
            this.LocalLogonEnabled = true;
            this.EmergencyAccessEnabled = true;
        }
        public bool EmergencyAccessEnabled { get; set; }
        public bool LocalLogonEnabled { get; set; }
        public int JwtValidityPeriod { get; set; }
        public string LocalIdpIdentifier { get; set; }
        public Guid Id { get; set; }
    }
}
