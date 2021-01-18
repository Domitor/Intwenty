using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Intwenty.Seed;
using Microsoft.Extensions.Hosting;
using Intwenty;
using Microsoft.Extensions.Configuration;
using Intwenty.Interface;
using IntwentyDemo.Seed;

namespace IntwentyDemo
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var modelservice = services.GetRequiredService<IIntwentyModelService>();

                    //Create intwenty db objects and identity db objects
                    //modelservice.CreateIntwentyDatabase();


                    //Create intwenty db objects and identity db objects
                    Identity.SeedDemoUsersAndRoles(services);
                    DefaultLocalization.Seed(services);
                    DemoModel.SeedModel(services);
                    DemoModel.SeedLocalizations(services);
                    DemoModel.ConfigureDatabase(services);
                    DemoModel.SeedData(services);


                }
                catch (Exception ex)
                {
                    throw ex;
                    //var logger = services.GetRequiredService<ILogger<Program>>();
                    //logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            host.Run();


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
                 webBuilder.UseStartup<Startup>();
                
             });


    }
}
