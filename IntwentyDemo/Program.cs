using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Intwenty;
using Microsoft.Extensions.Configuration;
using Intwenty.Interface;
using IntwentyDemo.Seed;
using System.Threading.Tasks;

namespace IntwentyDemo
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    //Below can be activated/deactivated in the appsetting.json file
                    //-SeedProductAndOrganizationOnStartUp
                    //-UseDemoSettings
                    //-SeedModelOnStartUp
                    //-SeedLocalizationsOnStartUp
                    //-ConfigureDatabaseOnStartUp
                    //-SeedDataOnStartUp


                    //Use intwenty to create the configured product and the organization
                    await Intwenty.Seed.Product.SeedProductAndOrganization(services);
                    //Use intwenty to create an admin user and more
                    await Intwenty.Seed.Demo.SeedDemoUsersAndRoles(services);
                    //Use intwenty to seed some common localization
                    await Intwenty.Seed.DefaultLocalization.Seed(services);


                    DemoModel.SeedModel(services);
                    DemoModel.SeedLocalizations(services);
                    DemoModel.ConfigureDatabase(services);
                    DemoModel.SeedData(services);

                    //At last create product athorization items (based on systems and applications in the model)
                    await Intwenty.Seed.Product.SeedProductAuthorizationItems(services);


                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            await host.RunAsync();


        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((hostingContext, config) =>
              {
                   config.AddUserSecrets("b77e8d87-d3be-4daf-9074-ec3ccd53ed21");
              })
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 //To allow local folders, needed by sqlite, local documents and so on....
                 webBuilder.UseStaticWebAssets();

                 webBuilder.UseStartup<Startup>();
                
             });


    }
}
