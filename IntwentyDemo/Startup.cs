using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Intwenty;
using Intwenty.Data.Identity;
using Intwenty.Model;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Intwenty.Data.Localization;

namespace IntwentyDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

       
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSignalR();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //Required for Intwenty: Settings
            services.Configure<IntwentySettings>(Configuration.GetSection("IntwentySettings"));

            //Required for Intwenty: Services
            services.AddTransient<IIntwentyModelService, IntwentyModelService>();
            services.AddTransient<IIntwentyDataService, IntwentyDataService>();

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
            })
             .AddRoles<IntwentyRole>()
             .AddUserStore<IntwentyUserStore>()
             .AddRoleStore<IntwentyRoleStore>();

            services.AddScoped<IAuthorizationHandler, IntwentyAllowAnonymousAuthorization>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IntwentyAppAuthorizationPolicy", policy =>
                {
                    policy.AddRequirements(new IntwentyAllowAnonymousAuthorization());
                });

                options.AddPolicy("IntwentyModelAuthorizationPolicy", policy =>
                {
                    policy.AddRequirements(new IntwentyAllowAnonymousAuthorization());
                });
            });

            //Culture
            services.Configure<RequestLocalizationOptions>(
              options =>
              {
                  var supportedCultures = new List<CultureInfo>
                      {
                            new CultureInfo("en-GB"),
                            new CultureInfo("sv-SE"),
                      };

                  options.DefaultRequestCulture = new RequestCulture(culture: "sv-SE", uiCulture: "sv-SE");
                  options.SupportedCultures = supportedCultures;
                  options.SupportedUICultures = supportedCultures;

              });

           


            //Remove this in production
            services.AddRazorPages().AddViewLocalization().AddRazorRuntimeCompilation();
            services.AddLocalization();

            services.AddSingleton<IStringLocalizerFactory, IntwentyStringLocalizerFactory>();
            //services.AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseRouting();

            //Needed only if Identity is used
            app.UseAuthentication();
            app.UseAuthorization();

          

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<Intwenty.PushData.ServerToClientPush>("/serverhub");
            });
        }
    }
}
