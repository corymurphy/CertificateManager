﻿using CertificateManager.Logic;
using CertificateManager.Logic.UXLogic;
using CertificateManager.Repository;
using CertificateServices;
using CertificateServices.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace CertificateManager
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            ConfigureAutoMapper();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<IISOptions>(options => {
                options.AutomaticAuthentication = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/view/auth/forbidden";
                    options.LoginPath = "/view/auth/login";
                });
                //.AddOpenIdConnect("OidcPrimary",
                //    options =>
                //    {
                //        options.MetadataAddress = @"https://idp.msdev.com:9445/oauth2/oidcdiscovery/.well-known/openid-configuration";
                //        //options.SaveTokens = true;
                //        options.ClientId = "_MA4J_KLfC_sH8cYOcyO5Sy7UZ0a";
                //        options.ClientSecret = "A1bpXBYrTBEZxkRxMq49pPuLE5wa";
                //        options.RemoteAuthenticationTimeout = TimeSpan.FromHours(1);
                //        options.ResponseType = "id_token token";
                //        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //        options.Authority = @"https://idp.dev.muellersystems.net:9445/oauth2/token";
                //        //options.
                //    }
                //);


            //options =>

            //{
            //    options.MetadataAddress = @"https://idp.msdev.com:9445/oauth2/oidcdiscovery/.well-known/openid-configuration";
            //    options.SaveTokens = true;
            //    options.ClientId = "_MA4J_KLfC_sH8cYOcyO5Sy7UZ0a";

            //}


            // Add framework services.
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

            LiteDbConfigurationRepository configurationRepository = new LiteDbConfigurationRepository(@"D:\db\config.db");
            RuntimeCacheRepository runtimeCacheRepository = new RuntimeCacheRepository(@"d:\db\runtimecache.db");
          
            services.AddSingleton<IConfigurationRepository>(configurationRepository);
            services.AddSingleton<ICertificateProvider>(new Win32CertificateProvider());
            services.AddSingleton<ICertificateRepository>(new LiteDbCertificateRepository(@"d:\db\certs.db"));
            services.AddSingleton<IRuntimeConfigurationState>(new RuntimeConfigurationState(configurationRepository, runtimeCacheRepository));
            services.AddSingleton<JavascriptConfigurationHelper>(new JavascriptConfigurationHelper(configurationRepository));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                WorkstationDevelopment devEnv = new WorkstationDevelopment(@"D:\db\config.db");
                //devEnv.Setup();

                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            IRuntimeConfigurationState runtimeConfigurationState = app.ApplicationServices.GetService<IRuntimeConfigurationState>();


            app.UseAuthentication();

           


            //runtimeConfigurationState.AlertApplicationStarted();

            //runtimeConfigurationState.Validate();


            app.UseStaticFiles();



            //app.UseCookieAuthentication(new CookieAuthenticationOptions()
            //{
            //    AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme,
            //    AutomaticAuthenticate = true,
                
            //});



            app.UseMvc();



        }

        public void ConfigureAutoMapper()
        {
            //AutoMapper.Mapper.
            //    CreateMap<, CertificateSubject>();
        }
    }
}
