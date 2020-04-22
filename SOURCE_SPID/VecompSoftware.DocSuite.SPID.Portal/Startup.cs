using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using VecompSoftware.DocSuite.SPID.AuthEngine.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using VecompSoftware.DocSuite.SPID.Logging.Configurations;
using VecompSoftware.DocSuite.SPID.JWT;
using VecompSoftware.DocSuite.SPID.Portal.Code;
using VecompSoftware.DocSuite.SPID.Portal.Models;
using VecompSoftware.DocSuite.SPID.Model.IDP;
using VecompSoftware.DocSuite.SPID.AuthEngine.Models;
using VecompSoftware.DocSuite.SPID.DataProtection;
using VecompSoftware.DocSuite.SPID.AuthEngine;

namespace VecompSoftware.DocSuite.SPID.Portal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IConfigurationSection jwtAppSettings = Configuration.GetSection(nameof(JwtConfiguration));
            IConfigurationSection authSettings = Configuration.GetSection(nameof(AuthConfiguration));

            services.AddProtectionData(this.Configuration);            

            services.AddSAMLAuth(this.Configuration);            
            services.AddSPIDLogging();

            services.AddTransient<CustomJwtBearerEvents>();

            ServiceProvider sp = services.BuildServiceProvider();
            IDataProtectionService dataProtectionService = sp.GetService<IDataProtectionService>();
            SymmetricSecurityKey signingKey = dataProtectionService.GetSigningKey();
            services.AddJwtSPID(this.Configuration, signingKey);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookie()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, cfg =>
                {                                                           
                    cfg.RequireHttpsMetadata = true;
                    cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtAppSettings[nameof(JwtConfiguration.Issuer)],

                        ValidateAudience = true,
                        ValidAudience = jwtAppSettings[nameof(JwtConfiguration.Issuer)],

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    cfg.EventsType = typeof(CustomJwtBearerEvents);
                });

            services.AddOptions();            
            services.Configure<ClientConfiguration>(options =>
            {
                options.IdpType = authSettings.GetValue<IdpType>(nameof(AuthConfiguration.IdpType));
                options.ApplicationName = this.Configuration.GetValue<string>(nameof(ClientConfiguration.ApplicationName));
            });

            Assembly externalAssembly = typeof(SamlController).GetTypeInfo().Assembly;
            EmbeddedFileProvider embeddedFileProvider = new EmbeddedFileProvider(externalAssembly, "VecompSoftware.DocSuite.SPID.AuthEngine");

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Add(embeddedFileProvider);
            });

            IConfigurationSection corsOrigins = Configuration.GetSection("AllowedOrigins");
            string[] allowedOrigins = corsOrigins.Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {            
            if (env.IsDevelopment())
            {
                loggerFactory.AddDebug();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseSPIDLogging();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCors("AllowSpecificOrigin");
            app.UseApplicationRoutes();
        }
    }
}
