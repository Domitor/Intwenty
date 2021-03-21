using Intwenty.Areas.Identity.Data;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Localization;
using Intwenty.Interface;
using Intwenty.Model;
using Intwenty.Services;
using Intwenty.SystemEvents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Intwenty.DataClient;
using Intwenty.Entity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Intwenty.Seed;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Intwenty.Middleware
{
    public static class mwIntwenty
    {
        public static void AddIntwenty<TIntwentyDataService, TIntwentyEventService>(this IServiceCollection services, IConfiguration configuration)
                         where TIntwentyDataService : class, IIntwentyDataService where TIntwentyEventService : class, IIntwentyEventService
        {
            services.AddIntwenty<TIntwentyDataService, TIntwentyEventService, IntwentySeeder>(configuration);
        }

        public static void AddIntwenty<TIntwentyDataService,TIntwentyEventService,TInwentySeeder>(this IServiceCollection services, IConfiguration configuration) 
                           where TIntwentyDataService : class, IIntwentyDataService where TIntwentyEventService : class, IIntwentyEventService where TInwentySeeder : class, IIntwentySeeder
        {
            services.AddSignalR();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var settings = configuration.GetSection("IntwentySettings").Get<IntwentySettings>();

            if (string.IsNullOrEmpty(settings.DefaultConnection))
                throw new InvalidOperationException("Could not find default database connection in setting file");
            if (string.IsNullOrEmpty(settings.IAMConnection))
                throw new InvalidOperationException("Could not find IAM database connection in setting file");
            if (string.IsNullOrEmpty(settings.ProductId))
                throw new InvalidOperationException("Could not find a valid productid in setting file");

            //Create Intwenty database objects
            CreateIntwentyFrameworkTables(settings);
            CreateIntwentyIAMTables(settings);

            //Required for Intwenty: Settings
            services.Configure<IntwentySettings>(configuration.GetSection("IntwentySettings"));

            //Required for Intwenty: Services
            //Override default intwenty dataservice
            services.TryAddTransient<IIntwentyDataService, TIntwentyDataService>();
            services.TryAddTransient<IIntwentyModelService, IntwentyModelService>();
            services.TryAddTransient<IIntwentyEventService, TIntwentyEventService>();
            services.TryAddTransient<IIntwentySeeder, TInwentySeeder>();
            services.TryAddTransient<IIntwentyProductManager, IntwentyProductManager>();
            services.TryAddTransient<IIntwentyOrganizationManager, IntwentyOrganizationManager>();



            //Required for Intwenty services to work correctly
            services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = false;
                options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

            //Required for Intwenty if Identity is used
            services.AddIdentity<IntwentyUser, IntwentyProductAuthorizationItem>(options =>
            {
                //options.SignIn.RequireConfirmedAccount = true;
                //options.SignIn.RequireConfirmedPhoneNumber = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.User.RequireUniqueEmail = true;

            })
             .AddRoles<IntwentyProductAuthorizationItem>()
             .AddUserStore<IntwentyUserStore>()
             .AddRoleStore<IntwentyProductAuthorizationStore>()
             .AddUserManager<IntwentyUserManager>()
             .AddSignInManager<IntwentySignInManager>()
             .AddClaimsPrincipalFactory<IntwentyClaimsPricipalFactory>()
             .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.Cookie.Name = "IntwentyAuthCookie";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Identity/Account/Login";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });


            if (settings.UseExternalLogins && settings.UseFacebookLogin)
            {
                services.AddAuthentication().AddFacebook(options =>
                {
                    options.AppId = settings.FacebookAppId;
                    options.AppSecret = settings.FacebookAppSecret;

                });
            }

            if (settings.UseExternalLogins && settings.UseGoogleLogin)
            {
                services.AddAuthentication().AddGoogle(options =>
                {
                    options.ClientId = settings.GoogleClientId;
                    options.ClientSecret = settings.GoogleClientSecret;

                });
            }

          


            if (string.IsNullOrEmpty(settings.DefaultCulture))
                throw new InvalidOperationException("Could not find DefaultCulture in setting file");
            if (settings.SupportedLanguages == null)
                throw new InvalidOperationException("Could not find SupportedLanguages in setting file");
            if (settings.SupportedLanguages.Count == 0)
                throw new InvalidOperationException("Could not find SupportedLanguages in setting file");

            var supportedCultures = settings.SupportedLanguages.Select(p => new CultureInfo(p.Culture)).ToList();
            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    options.AddInitialRequestCultureProvider(new UserCultureProvider());
                    options.DefaultRequestCulture = new RequestCulture(culture: settings.DefaultCulture, uiCulture: settings.DefaultCulture);
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                    options.RequestCultureProviders.Insert(0, new UserCultureProvider());

                });

            services.AddLocalization();

            services.AddSingleton<IStringLocalizerFactory, IntwentyStringLocalizerFactory>();

            services.AddMvc(options =>
            {
                // This pushes users to login if not authenticated
                options.Filters.Add(new AuthorizeFilter());
                
            }).AddViewLocalization();

            if (settings.UseIntwentyAPI)
            {
                services.AddSwaggerGen(options =>
                {
                    options.DocumentFilter<mwSwaggerDocumentFilter>();
                    options.AddSecurityDefinition("API-Key", new OpenApiSecurityScheme
                    {
                        Description = "API-Key",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "API-Key"

                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "API-Key"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });

                });
            }


        }

        public static IApplicationBuilder UseIntwenty(this IApplicationBuilder builder)
        {
            var configuration = builder.ApplicationServices.GetRequiredService<IConfiguration>();
            var settings = configuration.GetSection("IntwentySettings").Get<IntwentySettings>();

            builder.UseHttpsRedirection();
            builder.UseCookiePolicy();
            builder.UseRouting();


            builder.UseAuthentication();
            builder.UseAuthorization();
            builder.UseRequestLocalization();
            builder.ConfigureIntwentyTwoFactorAuth(settings);
            builder.ConfigureEndpoints(settings);
            builder.ConfigureIntwentyAPI(settings);
            builder.SeedIntwenty(settings);

            return builder;

        }

        private static void ConfigureEndpoints(this IApplicationBuilder builder, IntwentySettings settings)
        {

            builder.UseEndpoints(endpoints =>
            {

                endpoints.MapDefaultControllerRoute();

                //INTWENTY ROUTING
                endpoints.MapControllerRoute("apiroute_1", "Application/API/{action=All}/{id?}", defaults: new { controller = "ApplicationAPI" });
                endpoints.MapControllerRoute("apiroute_2", "Application/API/{action=All}/{applicationid?}/{viewid?}/{id?}", defaults: new { controller = "ApplicationAPI" });

                var modelservice = builder.ApplicationServices.GetRequiredService<IIntwentyModelService>();

                //REGISTER ENDPOINTS
                if (settings.UseIntwentyAPI)
                {
                    var epmodels = modelservice.GetEndpointModels();

                    foreach (var ep in epmodels)
                    {
                        if (ep.IsMetaTypeCustomPost)
                            continue;
                        if (ep.IsMetaTypeCustomGet)
                            continue;

                        endpoints.MapControllerRoute(ep.MetaCode, ep.Path + "{action=" + ep.Action + "}/{id?}", defaults: new { controller = "DynamicEndpoint" });
                    }
                }
                //endpoints.MapDynamicControllerRoute<IntwentyEndpointTransformer>("/cp/{**slug}");

                var appmodels = modelservice.GetApplicationModels();
                foreach (var a in appmodels)
                {

                    foreach (var view in a.Views)
                    {
                        if (string.IsNullOrEmpty(view.Path))
                            continue;

                        var path = view.Path;
                        path.Trim();
                        if (path[0] != '/')
                            path = "/" + path;
                        if (path[path.Length - 1] != '/')
                            path = path + "/";

                        endpoints.MapControllerRoute("app_route_" + a.Application.MetaCode + "_" + view.MetaCode, path, defaults: new { controller = "Application", action = "View" });
                    }
                }


                endpoints.MapRazorPages();
                endpoints.MapHub<Intwenty.PushData.ServerToClientPush>("/serverhub");


                /* Handle all responses
                endpoints.MapGet("/{**slug}", async context =>
                {
                   await context.Response.WriteAsync("Hello World!");
                });
                */

            });

        }

        private static void ConfigureIntwentyTwoFactorAuth(this IApplicationBuilder builder, IntwentySettings settings)
        {

            if (settings.ForceMfaAuthentication)
            {
                builder.UseMiddleware<mwMFA>();
            }

        }

        private static void ConfigureIntwentyAPI(this IApplicationBuilder builder, IntwentySettings settings)
        {
            if (settings.UseIntwentyAPI)
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                builder.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                builder.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Intwenty API V1");
                });
            }

        }


        private static void SeedIntwenty(this IApplicationBuilder builder, IntwentySettings settings)
        {

            //Below can be activated/deactivated in the appsetting.json file
            //-SeedProductAndOrganizationOnStartUp
            //-UseDemoSettings
            //-SeedModelOnStartUp
            //-SeedLocalizationsOnStartUp
            //-ConfigureDatabaseOnStartUp
            //-SeedDataOnStartUp

            var seederservice = builder.ApplicationServices.GetRequiredService<IIntwentySeeder>();

            //The order is important

            if (settings.SeedLocalizationsOnStartUp)
                seederservice.SeedLocalization(settings, builder.ApplicationServices);

            if (settings.SeedProductAndOrganizationOnStartUp)
                seederservice.SeedProductAndOrganization(settings, builder.ApplicationServices);

            if (settings.SeedDemoUserAccountsOnStartUp)
                seederservice.SeedUsersAndRoles(settings, builder.ApplicationServices);

            if (settings.SeedModelOnStartUp)
                seederservice.SeedModel(settings, builder.ApplicationServices);

            if (settings.SeedDataOnStartUp)
                seederservice.SeedData(settings, builder.ApplicationServices);

            if (settings.ConfigureDatabaseOnStartUp)
                seederservice.ConfigureDataBase(settings, builder.ApplicationServices);

            if (settings.SeedProductAndOrganizationOnStartUp)
                seederservice.SeedProductAuthorizationItems(settings, builder.ApplicationServices);


        }

        private static void CreateIntwentyFrameworkTables(IntwentySettings settings)
        {

            if (!settings.CreateIntwentyDbObjectsOnStartUp)
                return;

            var client = new Connection(settings.DefaultConnectionDBMS, settings.DefaultConnection);

            try
            {
                client.Open();
                client.CreateTable<SystemItem>();
                client.CreateTable<ApplicationItem>();
                client.CreateTable<DatabaseItem>();
                client.CreateTable<EventLog>();
                client.CreateTable<InformationStatus>();
                client.CreateTable<InstanceId>();
                client.CreateTable<ViewItem>();
                client.CreateTable<UserInterfaceStructureItem>();
                client.CreateTable<UserInterfaceItem>();
                client.CreateTable<FunctionItem>();
                client.CreateTable<ValueDomainItem>();
                client.CreateTable<DefaultValue>();
                client.CreateTable<TranslationItem>();
                client.CreateTable<EndpointItem>();
                client.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                client.Close();
            }



        }

        private static void CreateIntwentyIAMTables(IntwentySettings settings)
        {

            if (!settings.CreateIntwentyDbObjectsOnStartUp)
                return;

            var client = new Connection(settings.IAMConnectionDBMS, settings.IAMConnection);

            try
            {


                client.Open();
                client.CreateTable<IntwentyAuthorization>(); //security_Authorization
                client.CreateTable<IntwentyUser>(); //security_User
                client.CreateTable<IntwentyOrganization>(); //security_Organization
                client.CreateTable<IntwentyOrganizationMember>(); //security_OrganizationMembers
                client.CreateTable<IntwentyOrganizationProduct>(); //security_OrganizationProducts
                client.CreateTable<IntwentyProduct>(); //security_Product
                client.CreateTable<IntwentyProductAuthorizationItem>(); //security_ProductAuthorizationItem
                client.CreateTable<IntwentyProductGroup>(); //security_ProductGroup
                client.CreateTable<IntwentyUserProductGroup>(); //security_UserProductGroup
                client.CreateTable<IntwentyUserProductClaim>(); //security_UserProductClaim
                client.CreateTable<IntwentyUserProductLogin>(); //security_UserProductLogin
                client.CreateTable<IntwentyUserSetting>(); //security_UserSetting
                client.CreateTable<EventLog>();
                //client.CreateTable<IntwentyProductRoleClaim>(true, true); //security_RoleClaims
                //client.CreateTable<IntwentyUserProductToken>(true, true); //security_UserTokens
                client.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                client.Close();
            }

        }
    }
}
