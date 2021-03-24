using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Intwenty;
using Microsoft.Extensions.Configuration;
using Intwenty.Interface;
using IntwentyDemo.Seed;
using System.Threading.Tasks;
using Intwenty.Model;
using IntwentyDemo.Services;
using Intwenty.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting.Internal;
using Intwenty.Services;

namespace IntwentyDemo
{
    public class Program
    {

        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception ex)
            {

            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>

            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddUserSecrets("b77e8d87-d3be-4daf-9074-ec3ccd53ed21");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStaticWebAssets();

                    webBuilder.ConfigureServices((buildercontext, services) =>
                    {

                        var configuration = buildercontext.Configuration;
                        var settings = configuration.GetSection("IntwentySettings").Get<IntwentySettings>();
                        var adminroles = new string[] { "SUPERADMIN", "USERADMIN", "SYSTEMADMIN" };
                        var userroles = new string[] { "USER", "SUPERADMIN", "USERADMIN", "SYSTEMADMIN" };
                        var iamroles = new string[] { };
                        if (settings.UseSeparateIAMDatabase)
                            iamroles = new string[] { "SUPERADMIN" };
                        else
                            iamroles = new string[] { "SUPERADMIN", "USERADMIN" };


                        //Add Email and SMS
                        services.TryAddTransient<IIntwentyEmailService, EmailService>();
                        services.TryAddTransient<IIntwentySmsService, AspSmsService>();

                        //Add intwenty 
                        //services.AddIntwenty<IntwentyDataService,IntwentyEventService>(Configuration); //with default implementation
                        services.AddIntwenty<CustomDataService, CustomEventService, DemoSeeder>(configuration); //customized services

                      

                        //Default is anonymus athorization, comment out this line and use policy.RequireRole to apply role base authorization
                        //services.AddScoped<IAuthorizationHandler, IntwentyAllowAnonymousAuthorization>();
                        services.AddAuthorization(options =>
                        {
                            options.AddPolicy("IntwentyAppAuthorizationPolicy", policy =>
                            {
                                //policy.AddRequirements(new IntwentyAllowAnonymousAuthorization());
                                policy.RequireRole(userroles);
                            });

                            options.AddPolicy("IntwentyModelAuthorizationPolicy", policy =>
                            {
                                //policy.AddRequirements(new IntwentyAllowAnonymousAuthorization());
                                policy.RequireRole(adminroles);

                            });

                            options.AddPolicy("IntwentyUserAdminAuthorizationPolicy", policy =>
                            {
                                //policy.AddRequirements(new IntwentyAllowAnonymousAuthorization());
                                policy.RequireRole(iamroles);


                            });

                            options.AddPolicy("IntwentyProtectedLinkPolicy", policy =>
                            {
                                //policy.AddRequirements(new IntwentyAllowAnonymousAuthorization());
                                policy.RequireRole(userroles);
                                policy.RequireAuthenticatedUser();
                                policy.RequireClaim("linktoken");


                            });
                        });

                        services.AddRazorPages().AddViewLocalization().AddRazorRuntimeCompilation();
                    })
                    .Configure((buildercontext, app) =>
                    {
                        //var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
                        //var logger = loggerFactory.CreateLogger<Program>();
                        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                        var config = app.ApplicationServices.GetRequiredService<IConfiguration>();


                        //applicationLifetime.ApplicationStopped.Register(OnShutdown);
                        //applicationlifetime.ApplicationStarted.Register(OnStarted);

                        app.UseStaticFiles();
                        app.UseExceptionHandler("/Home/Error");
                        app.UseHsts();


                        //Set up everything related to intwenty
                        app.UseIntwenty();

                      


                    });
                });
        /*
        private void OnShutdown()
        {
            try
            {
                var settings = Configuration.GetSection("IntwentySettings").Get<IntwentySettings>();
                var client = new Connection(settings.DefaultConnectionDBMS, settings.DefaultConnection);
                client.InsertEntity<EventLog>(new EventLog() { ApplicationId = 0, AppMetaCode = "", EventDate = DateTime.Now, Id = 0, Message = "Instance stopped", UserName = "", Verbosity = "INFO" });
            }
            catch { }
        }

        private void OnStarted()
        {
            try
            {
                var settings = Configuration.GetSection("IntwentySettings").Get<IntwentySettings>();
                var client = new Connection(settings.DefaultConnectionDBMS, settings.DefaultConnection);
                client.InsertEntity<EventLog>(new EventLog() { ApplicationId = 0, AppMetaCode = "", EventDate = DateTime.Now, Id = 0, Message = "Instance started", UserName = "", Verbosity = "INFO" });
            }
            catch { }
        }
        */

      
    }
}
