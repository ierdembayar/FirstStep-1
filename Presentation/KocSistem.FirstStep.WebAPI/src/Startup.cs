// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="KocSistem">
// Copyright (c) KocSistem. All rights reserved.
// Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using KocSistem.FirstStep.Application;
using KocSistem.FirstStep.Application.Abstractions;
using KocSistem.FirstStep.Domain;
using KocSistem.FirstStep.Infrastructure;
using KocSistem.FirstStep.Persistence;
using KocSistem.OneFrame.ErrorHandling;
using KocSistem.OneFrame.ErrorHandling.Web;
using KocSistem.OneFrame.I18N;
using KocSistem.OneFrame.Logging;
using KocSistem.OneFrame.Mapper;
using KocSistem.OneFrame.Mapper.KsMapster;
using KocSistem.OneFrame.Notification.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KocSistem.FirstStep.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddKsLogging();

            this.AddKsI18N(serviceCollection);

            serviceCollection.AddCommonInfrastructure(this.Configuration);

            serviceCollection.AddPersistenceInfrastructure(this.Configuration);

            MapperConfigurationFactory.GetConfiguration<KsMapsterConfiguration>().Flexible().IgnoreCase();

            serviceCollection.AddTransient<IMapper, KsMapster>();

            this.AddEmailNotifications(serviceCollection);

            serviceCollection.AddTransient<IInitialDataSeedService, InitialDataSeedService>();

            this.AddSwaggerDocumentation(serviceCollection);

            this.AddIdentity(serviceCollection);

            this.AddAuthentication(serviceCollection);

            this.AddAuthorization(serviceCollection);

            serviceCollection.AddErrorHandling();

            this.AddCors(serviceCollection);

            this.AddMvcCore(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            loggerFactory.AddDebug();

            using (var scope = serviceCollection.BuildServiceProvider().CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<MainDbContext>();
                var pendingMigrations = dbContext.Database.GetPendingMigrations();
                if (pendingMigrations.Count() > 0)
                {
                    dbContext.Database.Migrate();
                }
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseKsExceptionHandler();
            app.UseHttpMethodOverride();
            app.UseStatusCodePages();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseRequestLocalization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                /*http://localhost:5000/swagger/v1/swagger.json*/
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
                c.InjectStylesheet("/swagger-ui/custom.css");
                c.DocExpansion(DocExpansion.None);
            });
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}",
                    defaults: new { format = "json" });
            });
        }

        private void AddSwaggerDocumentation(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info() { Title = "Default API", Version = "v1" });

                c.ExampleFilters();

                c.OperationFilter<AddResponseHeadersFilter>();

                c.DescribeAllEnumsAsStrings();

                // Configure Swagger to use the xml documentation file
                var xmlFile = Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml");
                c.IncludeXmlComments(xmlFile);

                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", Array.Empty<string>() },
                };
                c.AddSecurityDefinition("Basic", new ApiKeyScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = "header",
                    Name = "Authorization",
                    Type = "apiKey",
                });
                c.AddSecurityRequirement(security);

                c.OperationFilter<SecurityRequirementsOperationFilter>();
            }).AddSwaggerExamplesFromAssemblyOf<ApplicationRoleResponseExample>();
        }

        private void AddKsI18N(IServiceCollection serviceCollection)
        {
            serviceCollection.AddKsI18N((options) =>
            {
                options.DefaultCulture = "en-US";
                options.DefaultUICulture = "en-US";
                options.PlaceholderFormat = "[[{0}]]";
                options.ResourcesPath = "Resources/";
                options.SupportedUICultures = new string[] { "tr-TR", "en-US" };
            });
        }

        private void AddEmailNotifications(IServiceCollection serviceCollection)
        {
            serviceCollection.AddEmailNotifications(new EmailNotificationSettings()
            {
                Host = this.Configuration["EmailNotificationSettings:Host"],
                From = this.Configuration["EmailNotificationSettings:From"],
                Port = Convert.ToInt32(this.Configuration["EmailNotificationSettings:Port"]),
                ClientDomain = this.Configuration["EmailNotificationSettings:ClientDomain"],
                DefaultCredentials = true,
                UseSSL = false,
            });
        }

        private void AddCors(IServiceCollection serviceCollection)
        {
            serviceCollection.AddCors(x =>
            {
                x.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowCredentials()
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                });
            });
        }

        private void AddMvcCore(IServiceCollection serviceCollection)
        {
            // https://github.com/aspnet/Mvc/blob/master/src/Microsoft.AspNetCore.Mvc/MvcServiceCollectionExtensions.cs
            serviceCollection.AddMvc((options) =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;
                options.InputFormatters.Add(new XmlSerializerInputFormatter());
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                options.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/json");
                options.FormatterMappings.SetMediaTypeMappingForFormat("xml", "application/xml");
                options.Filters.Add(typeof(ValidateModelAttribute));
            })
            .AddDataAnnotationsLocalization();
        }

        private void AddIdentity(IServiceCollection serviceCollection)
        {
            serviceCollection.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequiredUniqueChars = 3;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;

                opt.SignIn.RequireConfirmedEmail = false;
                opt.SignIn.RequireConfirmedPhoneNumber = false;
            })
                    .AddEntityFrameworkStores<MainDbContext>()
                    .AddDefaultTokenProviders();

            serviceCollection.AddTransient<ITokenHelper, TokenHelper>();
        }

        private void AddAuthentication(IServiceCollection serviceCollection)
        {
            int expireInMinutes;
            if (!int.TryParse(this.Configuration["Identity:Jwt:ExpireInMinutes"], out expireInMinutes))
            {
                throw new OneFrameWebException("Invalid ExpireInMinutes in web.config");
            }

            serviceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                            .AddJwtBearer(options =>
                            {
                                options.TokenValidationParameters = new TokenValidationParameters()
                                {
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    ClockSkew = TimeSpan.FromMinutes(expireInMinutes),
                                    ValidIssuer = this.Configuration["Identity:Jwt:Issuer"],
                                    ValidAudience = this.Configuration["Identity:Jwt:Issuer"],
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["Identity:Jwt:Key"])),
                                };
                            });
        }

        private void AddAuthorization(IServiceCollection serviceCollection)
        {
            serviceCollection.AddAuthorization();
        }
    }
}