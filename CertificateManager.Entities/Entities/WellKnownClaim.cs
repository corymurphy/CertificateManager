namespace CertificateManager.Entities
{
    public static class WellKnownClaim
    {
        public static string Uid { get { return "http://certificatemanager/uid"; } }
        public static string AlternativeName { get { return "http://certificatemanager/alternative-upn";  } }
        public static string Role { get { return "http://certificatemanager/role"; } }
        public static string Name { get { return "http://certificatemanager/upn"; } }
    }
}
