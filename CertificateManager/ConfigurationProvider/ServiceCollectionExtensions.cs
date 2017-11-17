using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Logic.UXLogic;
using CertificateManager.Repository;
using CertificateServices;
using CertificateServices.ActiveDirectory;
using CertificateServices.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace CertificateManager.Logic.ConfigurationProvider
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureWritable<T>( this IServiceCollection services, IConfigurationSection section, string file = "appsettings.json") where T : class, new()
        {
            services.Configure<T>(section);

            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                var environment = provider.GetService<IHostingEnvironment>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                return new WritableOptions<T>(environment, options, section.Key, file);
            });
        }
    }
}
