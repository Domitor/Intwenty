using System;
using Microsoft.Extensions.DependencyInjection;
using Moley.Data.Entity;

namespace Moley.Data
{
    public class DBInitializer
    {
        public static void Initialize(IServiceProvider provider)
        {
            var context = provider.GetRequiredService<ApplicationDbContext>();

            
            if (context.Database.EnsureCreated())
            {
                SeedApplicationDescriptions(context);
                SeedSystemMenus(context);
                SeedApplicationContent(context);
                SeedValueDomains(context);
                SeedNoSeries(context);
            }

           
        }


        private static void SeedApplicationDescriptions(ApplicationDbContext context)
        {
            
           

            context.SaveChanges();
        }


        private static void SeedSystemMenus(ApplicationDbContext context)
        {

           
            context.SaveChanges();
        }


        private static void SeedApplicationContent(ApplicationDbContext context)
        {
           
          

            context.SaveChanges();
        }

        private static void SeedValueDomains(ApplicationDbContext context)
        {
          
            context.SaveChanges();
        }

        private static void SeedNoSeries(ApplicationDbContext context)
        {
           
            context.SaveChanges();
        }





    }
 }
