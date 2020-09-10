using System;
using Intwenty.Data.DBAccess;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using Intwenty.Model;
using Intwenty.Data.Entity;

namespace Intwenty.Data.Seed
{
    public static class SeedDefaultLocalization
    {


        public static void Seed(IServiceProvider services)
        {

            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();

            IIntwentyDbORM DataRepository = null;
            if (Settings.Value.IsNoSQL)
                DataRepository = new IntwentyNoSqlDbClient(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);
            else
                DataRepository = new IntwentySqlDbClient(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);


            var temp = new List<TranslationItem>();

            temp.Add(new TranslationItem() { Culture = "en-US", Key = "COPYRIGHT", Text = "2020 Intwenty - All rights reserved" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "COPYRIGHT", Text = "2020 Intwenty - Alla rättighter reserverade" });
            temp.Add(new TranslationItem() { Culture = "en-US", Key = "METAMODELDOC", Text = "Meta model documentation" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "METAMODELDOC", Text = "Dokumentation över metamodellen" });
            temp.Add(new TranslationItem() { Culture = "en-US", Key = "Create new", Text = "Create new" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "Create new", Text = "Skapa ny" });
            temp.Add(new TranslationItem() { Culture = "en-US", Key = "New", Text = "New {0}" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "New", Text = "Ny {0}" });
            temp.Add(new TranslationItem() { Culture = "en-US", Key = "Save", Text = "Save" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "Save", Text = "Spara" });
            temp.Add(new TranslationItem() { Culture = "en-US", Key = "Edit", Text = "Edit" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "Edit", Text = "Ändra" });
            temp.Add(new TranslationItem() { Culture = "en-US", Key = "APIKEYINFO", Text = "Create your API Key in order to integrate with our service" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", Key = "APIKEYINFO", Text = "Create your API Key in order to integrate with our service" });



            var existing = DataRepository.GetAll<TranslationItem>();
            foreach (var t in temp)
            {
                if (!existing.Exists(p => p.Culture == t.Culture && p.Key == t.Key))
                    DataRepository.Insert(t);
            }


        }


    }
}
