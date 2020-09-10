using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Intwenty.Data.Seed;
using Microsoft.Extensions.Hosting;
using Intwenty;

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
                    modelservice.CreateIntwentyDatabase();


                    //Create intwenty db objects and identity db objects
                    SeedIdentity.Seed(services);
                    SeedDefaultLocalization.Seed(services);
                    SeedSalesOrderDemoModel.Seed(services);
                    SeedSalesOrderDemoModel.ConfigureDataBase(services);


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
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.UseStaticWebAssets();
                 webBuilder.UseStartup<Startup>();
             });


    }
}
