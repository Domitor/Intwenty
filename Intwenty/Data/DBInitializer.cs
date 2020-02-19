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
            
            context.Set<ApplicationDescription>().Add(new ApplicationDescription() { Description = "An app for managing tickets", MetaCode = "TICKET", Title = "Ticket", DbName = "Ticket", IsHierarchicalApplication = false, UseVersioning=false, TestDataAmount = 100  });
            


            context.SaveChanges();
        }


        private static void SeedSystemMenus(ApplicationDbContext context)
        {

            context.Set<MetaMenuItem>().Add(new MetaMenuItem() { AppMetaCode = "", MetaType = "MAINMENU", MetaCode = "TICKET_MAINMENU", ParentMetaCode = "ROOT", Title = "Menu", Order = 1, Action = "", Controller = ""  });
            context.Set<MetaMenuItem>().Add(new MetaMenuItem() { AppMetaCode = "TICKET", MetaType = "MENUITEM", MetaCode = "M_ITEM_TICKET", ParentMetaCode = "TICKET_MAINMENU", Title = "Ticket", Order = 10, Action = "", Controller = "" });
           

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
            //NoSerie
            context.Set<NoSerie>().Add(new NoSerie() { AppMetaCode = "ITEM", MetaCode = "TICKETID_SERIE", DataMetaCode="TICKETID", Description= "No serie definition for Ticket Id", Prefix = "TIC", StartValue = 10000, Counter = 0  });


            context.SaveChanges();
        }





    }
 }
