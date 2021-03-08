using System;
using Intwenty.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using Intwenty.Model;
using Intwenty.Entity;
using Intwenty.DataClient;
using System.Threading.Tasks;

namespace Intwenty.Seed
{
    public static class DefaultLocalization
    {


        public static async Task Seed(IServiceProvider services)
        {

            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();

            var client = new Connection(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);

            try
            {
                var temp = new List<TranslationItem>();

                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Code", Text = "Code" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Code", Text = "Kod" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Address", Text = "Address" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Address", Text = "Adress" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Message", Text = "Message" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Message", Text = "Meddelande" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Email", Text = "Email" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Email", Text = "Epost" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Phone", Text = "Phone" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Phone", Text = "Telefon" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Previous", Text = "Previous" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Previous", Text = "Föregående" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Next", Text = "Next" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Next", Text = "Nästa" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Changed By", Text = "Changed By" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Changed By", Text = "Ändrad Av" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Changed", Text = "Changed" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Changed", Text = "Ändrad" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "List", Text = "List" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "List", Text = "Lista" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Add", Text = "Add" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Add", Text = "Lägg till" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Open", Text = "Open" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Open", Text = "Öppna" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Close", Text = "Close" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Close", Text = "Stäng" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Title", Text = "Title" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Title", Text = "Titel" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Delete",  Text = "Delete ?" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Delete",  Text = "Ta bort ?" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Cancel",  Text = "Cancel" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Cancel",  Text = "Avbryt" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Date",  Text = "Date" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Date",  Text = "Datum" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Event",  Text = "Event" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Event",  Text = "Händelse" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Name",  Text = "Name" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Name",  Text = "Namn" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Create new", Text = "Create new" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Create new", Text = "Skapa ny" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "New", Text = "New" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "New", Text = "Ny" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Save", Text = "Save" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Save", Text = "Spara" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Changes Saved", Text = "Changes saved" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Changes Saved", Text = "Ändringar sparade" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Edit", Text = "Edit" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Edit", Text = "Ändra" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Menu", Text = "Menu" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Menu", Text = "Meny" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "BACKTOLIST", Text = "Back to list" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "BACKTOLIST", Text = "Tillbaka" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "ADDEDIT", Text = "Add / Edit" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "ADDEDIT", Text = "Lägg till / Ändra" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "COPYRIGHT", Text = "2020 Intwenty - All rights reserved" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "COPYRIGHT", Text = "2020 Intwenty - Alla rättighter reserverade" });


                await client.OpenAsync();

                var existing = await client.GetEntitiesAsync<TranslationItem>();

                foreach (var t in temp)
                {
                    if (!existing.Exists(p => p.Culture == t.Culture && p.TransKey == t.TransKey))
                        await client.InsertEntityAsync(t);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                await client.CloseAsync();
            }

          


        }


    }
}
