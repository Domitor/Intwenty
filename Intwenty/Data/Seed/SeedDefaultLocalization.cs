using System;
using Intwenty.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using Intwenty.Model;
using Intwenty.Data.Entity;
using Intwenty.DataClient;

namespace Intwenty.Data.Seed
{
    public static class SeedDefaultLocalization
    {


        public static void Seed(IServiceProvider services)
        {

            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();

           

            var temp = new List<TranslationItem>();

            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "COPYRIGHT", Text = "2020 Intwenty - All rights reserved" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "COPYRIGHT", Text = "2020 Intwenty - Alla rättighter reserverade" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "METAMODELDOC", Text = "Meta model documentation" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "METAMODELDOC", Text = "Dokumentation över metamodellen" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Create new", Text = "Create new" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Create new", Text = "Skapa ny" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "New", Text = "New {0}" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "New", Text = "Ny {0}" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Save", Text = "Save" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Save", Text = "Spara" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Edit", Text = "Edit" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Edit", Text = "Ändra" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "APIKEYINFO", Text = "Create your API Key in order to integrate with our service" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "APIKEYINFO", Text = "Create your API Key in order to integrate with our service" });


            var client = new Connection(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);
            client.Open();
            var existing = client.GetEntities<TranslationItem>();
            client.Close();
            foreach (var t in temp)
            {
                if (!existing.Exists(p => p.Culture == t.Culture && p.TransKey == t.TransKey))
                    client.InsertEntity(t);
            }


        }


    }
}
