using CertificateManager.Entities;
using CertificateManager.Logic;
using CertificateManager.Logic.ActiveDirectory;
using CertificateManager.Logic.ActiveDirectory.Interfaces;
using CertificateManager.Logic.ConfigurationProvider;
using CertificateManager.Logic.InitialSetupDependencies;
using CertificateManager.Logic.Interfaces;
using CertificateManager.Logic.MvcMiddleware;
using CertificateManager.Logic.UXLogic;
using CertificateManager.Repository;
using CertificateServices;
using CertificateServices.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager
{

    public class Startup
    {
        DatabaseLocator databaseLocator;
        private bool initialSetupComplete = false;
        private IHostingEnvironment env;
        private CertificateManagementLogic certificateManagementLogic;
        private AppConfig appConfig;
        private EnvironmentInitializationProvider environmentInitializationProvider;
        private IOpenIdConnectIdentityProviderLogic oidcLogic;
        //private static CancellationTokenSource cancelTokenSource = new System.Threading.CancellationTokenSource();

        public Startup(IHostingEnvironment env)
        {

            this.env = env;
            environmentInitializationProvider = new EnvironmentInitializationProvider(env);

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            ConfigureAutoMapper();

            
            
        }

        public IConfigurationRoot Configuration { get; }


        public void InitializeOidc(IServiceCollection services, IEnumerable<OidcIdentityProvider> idps)
        {
            foreach(OidcIdentityProvider idp in idps)
            {
                services.AddAuthentication().AddOpenIdConnect
                (
                    options =>
                    {
                        options.Authority = idp.Authority;
                        options.ClientId = idp.ClientId;
                        options.Scope.Add("email");
                    }
                );
            }
        }

        public void ConfigureAuthentication(IServiceCollection services)
        {


            DownloadPfxCertificateEntity certEntity = certificateManagementLogic.GetPfxCertificateContent(appConfig.JwtCertificateId);

            string password = certificateManagementLogic.GetCertificatePassword(appConfig.JwtCertificateId, LocalIdentityProviderLogic.GetSystemIdentity()).DecryptedPassword;

            X509Certificate2 cert = new X509Certificate2(certEntity.Content, password);


            services.AddAuthentication(IISDefaults.AuthenticationScheme);


            services
                .AddAuthentication(authOptions =>
                {
                    authOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    authOptions.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    authOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    authOptions.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    authOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/view/auth/forbidden";
                    options.LoginPath = "/view/auth/login";
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = appConfig.LocalIdpIdentifier,
                        ValidAudience = appConfig.LocalIdpIdentifier,
                        IssuerSigningKey = new SymmetricSecurityKey(cert.Export(X509ContentType.Cert))
                    };

                });



        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureWritable<AppSettings>(Configuration.GetSection("AppSettings"), environmentInitializationProvider.GetAppSettingsFileName());

            services.Configure<IISOptions>(options => {
                options.AutomaticAuthentication = true;
            });

            AppSettings appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();

            databaseLocator = new DatabaseLocator(appSettings);

            if(databaseLocator.ConfigurationRepositoryExists())
            {
                initialSetupComplete = true;
                InitializeApp(services, appSettings);
            }
            else
            {
                initialSetupComplete = false;
                InitializeSetup(services);
            }

            ConfigureAuthentication(services);

            // Add framework services.
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                //WorkstationDevelopment devEnv = new WorkstationDevelopment(@"D:\db\config.db");
                //devEnv.Setup();

                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //IRuntimeConfigurationState runtimeConfigurationState = app.ApplicationServices.GetService<IRuntimeConfigurationState>();

            //if (env.IsDevelopment())
            //    runtimeConfigurationState.IsDevelopment = true;


            app.UseAuthentication();
            
            app.UseStaticFiles();

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseMvc();
        }

        private void RequireInitialSetup(IApplicationBuilder app)
        {
            //app.UseMvc(routes => routes.MapRoute().DefaultHandler)


            //app.Run(context =>
            //{
            //    if(context.Request.Path.Value != "/initial-setup") context.Response.Redirect("/initial-setup");
            //    return Task.FromResult<object>(null);
            //});
        }


        private void InitializeSetup(IServiceCollection services)
        {
            services.AddSingleton<IRuntimeConfigurationState>(
                new RuntimeConfigurationState(null, null)
                {
                    InitialSetupComplete = false
                }
            );

            services.AddSingleton<IClientsideConfigurationProvider>(new ClientsideConfigurationProviderInitialSetup());

            services.AddSingleton<IAuditLogic>(new AuditLogicInitialSetup());
            //IdentityAuthenticationLogic(configurationRepository, activeDirectoryAuthenticator)

            services.AddSingleton<IdentityAuthenticationLogic>(new IdentityAuthenticationLogic(null, null));
        }

        private void InitializeApp(IServiceCollection services, AppSettings appSettings)
        {
            LiteDbConfigurationRepository configurationRepository = new LiteDbConfigurationRepository(databaseLocator.GetConfigurationRepositoryConnectionString());

            appConfig = configurationRepository.GetAppConfig();

            ActiveDirectoryRepository activeDirectory = new ActiveDirectoryRepository();

            EncryptionProvider cipher = new EncryptionProvider(appConfig.EncryptionKey);

            services.AddSingleton<EncryptionProvider>(cipher);

            services.AddSingleton<IActiveDirectoryAuthenticator>(activeDirectory);
            services.AddSingleton<IActiveDirectoryRepository>(activeDirectory);

            IdentityAuthenticationLogic identityAuthenticationLogic = new IdentityAuthenticationLogic(configurationRepository, activeDirectory);

            services.AddSingleton<IdentityAuthenticationLogic>();

            ICertificateRepository certificateRepository = new LiteDbCertificateRepository(databaseLocator.GetCertificateRepositoryConnectionString());

            RuntimeCacheRepository runtimeCacheRepository = null;

            LiteDbAuditRepository auditRepository = new LiteDbAuditRepository(databaseLocator.GetAuditRepositoryConnectionString());

            IAuditLogic auditLogic = new AuditLogic(auditRepository, configurationRepository);

            services.AddSingleton<IAuditLogic>(auditLogic);

            IAuthorizationLogic authorizationLogic = new AuthorizationLogic(configurationRepository, auditLogic);

            IScriptManagementLogic scriptManagement = new ScriptManagementLogic(configurationRepository, authorizationLogic);

            services.AddSingleton<IScriptManagementLogic>(scriptManagement);

            IPowershellEngine powershellEngine = new PowershellEngine(auditLogic, scriptManagement);

            services.AddSingleton<IPowershellEngine>(powershellEngine);

            RoleManagementLogic roleManagementLogic = new RoleManagementLogic(configurationRepository, authorizationLogic);

            services.AddSingleton<RoleManagementLogic>(roleManagementLogic);

            UserManagementLogic userManagementLogic = new UserManagementLogic(configurationRepository, authorizationLogic);

            services.AddSingleton<UserManagementLogic>(userManagementLogic);

            SecurityPrincipalLogic securityPrincipalLogic = new SecurityPrincipalLogic(roleManagementLogic, userManagementLogic);

            services.AddSingleton<SecurityPrincipalLogic>();

            AdcsTemplateLogic adcsTemplateLogic = new AdcsTemplateLogic(configurationRepository, activeDirectory);

            services.AddSingleton<AdcsTemplateLogic>(adcsTemplateLogic);

            services.AddSingleton<IAuthorizationLogic>(authorizationLogic);

            services.AddSingleton<IConfigurationRepository>(configurationRepository);

            ICertificateProvider certificateProvider = new Win32CertificateProvider();

            services.AddSingleton<ICertificateProvider>(certificateProvider);
          
            services.AddSingleton<ICertificateRepository>(certificateRepository);

            ActiveDirectoryIdentityProviderLogic activeDirectoryIdentityProviderLogic = new ActiveDirectoryIdentityProviderLogic(configurationRepository);

            services.AddSingleton<ActiveDirectoryIdentityProviderLogic>(activeDirectoryIdentityProviderLogic);

            certificateManagementLogic = new CertificateManagementLogic(
                configurationRepository,
                certificateRepository,
                authorizationLogic,
                auditLogic,
                securityPrincipalLogic,
                cipher);

            services.AddSingleton<CertificateManagementLogic>(certificateManagementLogic);

            PrivateCertificateProcessing privateCertificateProcessing  = new PrivateCertificateProcessing(certificateRepository, configurationRepository, certificateProvider, authorizationLogic, adcsTemplateLogic, auditLogic);

            services.AddSingleton<IPrivateCertificateProcessing>(privateCertificateProcessing);

            services.AddSingleton<NodeLogic>(new NodeLogic(configurationRepository, authorizationLogic, activeDirectoryIdentityProviderLogic, powershellEngine, auditLogic, certificateManagementLogic, privateCertificateProcessing));
        
            services.AddSingleton<IRuntimeConfigurationState>(
                new RuntimeConfigurationState(configurationRepository, runtimeCacheRepository)
                {
                    InitialSetupComplete = initialSetupComplete
                });

            services.AddSingleton<IClientsideConfigurationProvider>(new ClientsideConfigurationProvider(configurationRepository));



            services.AddSingleton<AnalyticsLogic>(new AnalyticsLogic(configurationRepository, certificateRepository, auditRepository));

            services.AddSingleton<DataRenderingProvider>(new DataRenderingProvider());

            oidcLogic = new OpenIdConnectIdentityProviderLogic(configurationRepository, authorizationLogic);
            services.AddSingleton<IOpenIdConnectIdentityProviderLogic>(oidcLogic);
        }

        public void ConfigureAutoMapper()
        {

        }


    }
}
