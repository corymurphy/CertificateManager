using Microsoft.AspNetCore.Hosting;

namespace CertificateManager.Entities
{
    public class EnvironmentInitializationProvider
    {
        IHostingEnvironment env;
        public EnvironmentInitializationProvider(IHostingEnvironment env)
        {
            this.env = env;
        }

        public string GetAppSettingsFileName()
        {
            if (env.IsProduction())
                return "appsettings.json";
            else
                return $"appsettings.{env.EnvironmentName}.json";
        }
    }
}
