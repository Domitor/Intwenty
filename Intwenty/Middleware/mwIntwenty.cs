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
using Microsoft.AspNetCore.Identity.UI.Services;
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

namespace Intwenty.Middleware
{
    public static class mwIntwenty
    {

        public static void AddIntwenty<TIntwentyDataService,TIntwentyEventService>(this IServiceCollection services, IConfiguration configuration) 
                           where TIntwentyDataService : class, IIntwentyDataService where TIntwentyEventService : class, IIntwentyEventService
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
            services.TryAddTransient<IEmailSender, EmailService>();
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
            services.AddDefaultIdentity<IntwentyUser>(options =>
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
             .AddSignInManager<IntwentySignInManager>();
            


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

         


            //CHANGE 20210120
            //services.AddRazorPages().AddViewLocalization();

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


            //Needed only if Identity is used
            builder.UseAuthentication();
            builder.UseAuthorization();
            builder.UseRequestLocalization();


            builder.UseEndpoints(endpoints =>
            {

                endpoints.MapDefaultControllerRoute();

                //INTWENTY ROUTING
                endpoints.MapControllerRoute("approute_1", "{controller=Application}/{action=All}/{id}");
                endpoints.MapControllerRoute("approute_2", "{controller=Application}/{action=Edit}/{applicationid}/{id}");
                endpoints.MapControllerRoute("apiroute_1", "Application/API/{action=All}/{id?}", defaults: new { controller = "ApplicationAPI" });
                endpoints.MapControllerRoute("apiroute_2", "Application/API/{action=All}/{applicationid?}/{id?}", defaults: new { controller = "ApplicationAPI" });

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

                        endpoints.MapControllerRoute(ep.MetaCode, ep.Path + "{action=" + ep.Action + "}/{id?}", defaults: new { controller = "CustomerAPI" });
                    }
                }
                //endpoints.MapDynamicControllerRoute<IntwentyEndpointTransformer>("/cp/{**slug}");

                var appmodels = modelservice.GetApplicationModels();
                foreach (var a in appmodels)
                {
                    if (a.Application == null)
                        continue;

                    if (string.IsNullOrEmpty(a.Application.ApplicationPath))
                        continue;

                    if (a.Application.ApplicationPath.Length < 2)
                        continue;

                    var path = a.Application.ApplicationPath;
                    path.Trim();
                    if (path[0] != '/')
                        path = "/" + path;
                    if (path[path.Length - 1] != '/')
                        path = path + "/";

                    endpoints.MapControllerRoute("app_route_" + a.Application.MetaCode + "_create", path + "{action=Create}", defaults: new { controller = "Application" });
                    endpoints.MapControllerRoute("app_route_" + a.Application.MetaCode + "_edit", path + "{action=Edit}/{id}", defaults: new { controller = "Application" });
                    endpoints.MapControllerRoute("app_route_" + a.Application.MetaCode + "_editlist", path + "{action=EditList}", defaults: new { controller = "Application" });
                    endpoints.MapControllerRoute("app_route_" + a.Application.MetaCode + "_detail", path + "{action=Detail}/{id}", defaults: new { controller = "Application" });
                    endpoints.MapControllerRoute("app_route_" + a.Application.MetaCode + "_create", path + "{action=List}", defaults: new { controller = "Application" });
                }


                endpoints.MapRazorPages();
                endpoints.MapHub<Intwenty.PushData.ServerToClientPush>("/serverhub");
            }); 

           

            if (settings.ForceTwoFactorAuthentication)
            {
                builder.UseMiddleware<mwMFA>();
            }

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


            return builder;

        }

        private static void CreateIntwentyFrameworkTables(IntwentySettings settings)
        {

            if (!settings.SeedModelOnStartUp)
                return;

            var client = new Connection(settings.DefaultConnectionDBMS, settings.DefaultConnection);

            try
            {
                client.Open();
                client.CreateTable<SystemItem>();
                client.CreateTable<ApplicationItem>();
                client.CreateTable<DatabaseItem>();
                client.CreateTable<DataViewItem>();
                client.CreateTable<EventLog>();
                client.CreateTable<InformationStatus>();
                client.CreateTable<InstanceId>();
                client.CreateTable<UserInterfaceItem>();
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

            if (!settings.SeedModelOnStartUp)
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
