using Intwenty.Areas.Identity.Data;
using Intwenty.Areas.Identity.Entity;
using Intwenty.DataClient;
using Intwenty.Entity;
using Intwenty.Interface;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Seed
{
    public class IntwentySeeder : IIntwentySeeder
    {
        public virtual async Task SeedLocalization(IntwentySettings settings, IServiceProvider services)
        {
           
            var client = new Connection(settings.DefaultConnectionDBMS, settings.DefaultConnection);

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
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Delete", Text = "Delete ?" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Delete", Text = "Ta bort ?" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Cancel", Text = "Cancel" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Cancel", Text = "Avbryt" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Date", Text = "Date" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Date", Text = "Datum" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Event", Text = "Event" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Event", Text = "Händelse" });
                temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Name", Text = "Name" });
                temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Name", Text = "Namn" });
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

        public virtual async Task SeedProductAndOrganization(IntwentySettings settings, IServiceProvider services)
        {

            var productManager = services.GetRequiredService<IIntwentyProductManager>();
            var organizationManager = services.GetRequiredService<IIntwentyOrganizationManager>();

            

            IntwentyProduct product = await productManager.FindByIdAsync(settings.ProductId);
            if (product == null)
            {
                product = new IntwentyProduct();
                product.Id = settings.ProductId;
                product.ProductName = settings.SiteTitle;
                await productManager.CreateAsync(product);
            }

            IntwentyOrganization org = await organizationManager.FindByNameAsync(settings.DefaultProductOrganization);
            if (org == null)
            {
                org = new IntwentyOrganization();
                org.Name = settings.DefaultProductOrganization;
                await organizationManager.CreateAsync(org);
            }

            var all_products = await organizationManager.GetProductsAsync(org.Id);
            var thisproduct = all_products.Find(p => p.ProductId == product.Id);
            if (thisproduct == null)
            {
                await organizationManager.AddProductAsync(new IntwentyOrganizationProduct() { OrganizationId = org.Id, ProductId = product.Id, ProductName = product.ProductName });
            }
        }
        public virtual async Task SeedProductAuthorizationItems(IntwentySettings settings, IServiceProvider services)
        {
            
            var client = new Connection(settings.DefaultConnectionDBMS, settings.DefaultConnection);

            await client.OpenAsync();
            var current_apps = await client.GetEntitiesAsync<ApplicationItem>();
            var current_systems = await client.GetEntitiesAsync<SystemItem>();
            var current_views = await client.GetEntitiesAsync<ViewItem>();
            await client.CloseAsync();

            var iamclient = new Connection(settings.IAMConnectionDBMS, settings.IAMConnection);
            await iamclient.OpenAsync();
            var current_permissions = await iamclient.GetEntitiesAsync<IntwentyProductAuthorizationItem>();

            foreach (var t in current_systems)
            {
                if (!current_permissions.Exists(p => p.MetaCode == t.MetaCode && p.ProductId == settings.ProductId && p.AuthorizationType == SystemModelItem.MetaTypeSystem))
                {
                    var sysauth = new IntwentyProductAuthorizationItem() { Id = Guid.NewGuid().ToString(), Name = t.Title, NormalizedName = t.MetaCode.ToUpper(), AuthorizationType = SystemModelItem.MetaTypeSystem, ProductId = settings.ProductId };
                    await iamclient.InsertEntityAsync(sysauth);
                }
            }


            foreach (var t in current_apps)
            {
                if (!current_permissions.Exists(p => p.MetaCode == t.MetaCode && p.ProductId == settings.ProductId && p.AuthorizationType == ApplicationModelItem.MetaTypeApplication))
                {
                    var appauth = new IntwentyProductAuthorizationItem() { Id = Guid.NewGuid().ToString(), Name = t.Title, NormalizedName = t.MetaCode.ToUpper(), AuthorizationType = ApplicationModelItem.MetaTypeApplication, ProductId = settings.ProductId };
                    await iamclient.InsertEntityAsync(appauth);
                }

                foreach (var v in current_views)
                {
                    if (v.AppMetaCode == t.MetaCode)
                    {
                        if (!current_permissions.Exists(p => p.MetaCode == v.MetaCode && p.ProductId == settings.ProductId && p.AuthorizationType == ViewModel.MetaTypeUIView))
                        {
                            var appauth = new IntwentyProductAuthorizationItem() { Id = Guid.NewGuid().ToString(), Name = t.Title + " - " + v.Title, NormalizedName = v.MetaCode.ToUpper(), AuthorizationType = ViewModel.MetaTypeUIView, ProductId = settings.ProductId };
                            await iamclient.InsertEntityAsync(appauth);
                        }
                    }
                }
            }

            await iamclient.CloseAsync();
        }

        public virtual async Task SeedUsersAndRoles(IntwentySettings settings, IServiceProvider services)
        {

            var userManager = services.GetRequiredService<IntwentyUserManager>();
            var roleManager = services.GetRequiredService<RoleManager<IntwentyProductAuthorizationItem>>();
            var productManager = services.GetRequiredService<IIntwentyProductManager>();
            var organizationManager = services.GetRequiredService<IIntwentyOrganizationManager>();

            //ENSURE WE HAVE A PRODUCT AND ORG
            IntwentyProduct product = await productManager.FindByIdAsync(settings.ProductId);
            if (product == null)
            {
                product = new IntwentyProduct();
                product.Id = settings.ProductId;
                product.ProductName = settings.SiteTitle;
                await productManager.CreateAsync(product);
            }

            IntwentyOrganization org = await organizationManager.FindByNameAsync(settings.DefaultProductOrganization);
            if (org == null)
            {
                org = new IntwentyOrganization();
                org.Name = settings.DefaultProductOrganization;
                await organizationManager.CreateAsync(org);
            }

            var all_products = await organizationManager.GetProductsAsync(org.Id);
            var thisproduct = all_products.Find(p => p.ProductId == product.Id);
            if (thisproduct == null)
            {
                await organizationManager.AddProductAsync(new IntwentyOrganizationProduct() { OrganizationId = org.Id, ProductId = product.Id, ProductName = product.ProductName });
            }



            var admrole = await roleManager.FindByNameAsync("SUPERADMIN");
            if (admrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "SUPERADMIN";
                role.AuthorizationType = "ROLE";
                await roleManager.CreateAsync(role);
            }

            admrole = await roleManager.FindByNameAsync("USERADMIN");
            if (admrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "USERADMIN";
                role.AuthorizationType = "ROLE";
                await roleManager.CreateAsync(role);
            }

            admrole = await roleManager.FindByNameAsync("SYSTEMADMIN");
            if (admrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "SYSTEMADMIN";
                role.AuthorizationType = "ROLE";
                await roleManager.CreateAsync(role);
            }

            var userrole = await roleManager.FindByNameAsync("USER");
            if (userrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "USER";
                role.AuthorizationType = "ROLE";
                await roleManager.CreateAsync(role);
            }

            userrole = await roleManager.FindByNameAsync("APIUSER");
            if (userrole == null)
            {
                var role = new IntwentyProductAuthorizationItem();
                role.ProductId = product.Id;
                role.Name = "APIUSER";
                role.AuthorizationType = "ROLE";
                await roleManager.CreateAsync(role);
            }

            if (!string.IsNullOrEmpty(settings.DemoAdminUser) && !string.IsNullOrEmpty(settings.DemoAdminPassword))
            {
                IntwentyUser admin_user = await userManager.FindByNameAsync(settings.DemoAdminUser);
                if (admin_user == null)
                {
                    admin_user = new IntwentyUser();
                    admin_user.UserName = settings.DemoAdminUser;
                    admin_user.Email = settings.DemoAdminUser;
                    admin_user.FirstName = "Admin";
                    admin_user.LastName = "Adminsson";
                    admin_user.EmailConfirmed = true;
                    admin_user.Culture = settings.DefaultCulture;
                    await userManager.CreateAsync(admin_user, settings.DemoAdminPassword);
                }

                var all_members = await organizationManager.GetMembersAsync(org.Id);
                var admin_member = all_members.Find(p => p.UserId == admin_user.Id);
                if (admin_member == null)
                {
                    await organizationManager.AddMemberAsync(new IntwentyOrganizationMember() { OrganizationId = org.Id, UserId = admin_user.Id, UserName = admin_user.UserName });
                    await userManager.AddUpdateUserRoleAuthorizationAsync("SUPERADMIN", admin_user.Id, org.Id, settings.ProductId);
                }
            }

            if (!string.IsNullOrEmpty(settings.DemoUser) && !string.IsNullOrEmpty(settings.DemoUserPassword))
            {
                IntwentyUser default_user = await userManager.FindByNameAsync(settings.DemoUser);
                if (default_user == null)
                {
                    default_user = new IntwentyUser();
                    default_user.UserName = settings.DemoUser;
                    default_user.Email = settings.DemoUser;
                    default_user.FirstName = "User";
                    default_user.LastName = "Usersson";
                    default_user.EmailConfirmed = true;
                    default_user.Culture = settings.DefaultCulture;
                    await userManager.CreateAsync(default_user, settings.DemoUserPassword);
                }

                var all_members = await organizationManager.GetMembersAsync(org.Id);
                var user_member = all_members.Find(p => p.UserId == default_user.Id);
                if (user_member == null)
                {
                    await organizationManager.AddMemberAsync(new IntwentyOrganizationMember() { OrganizationId = org.Id, UserId = default_user.Id, UserName = default_user.UserName });
                    await userManager.AddUpdateUserRoleAuthorizationAsync("USER", default_user.Id, org.Id, settings.ProductId);
                }
            }
        }


     

        public virtual async Task SeedModel(IntwentySettings settings, IServiceProvider services)
        {
          
        }

        public virtual async Task SeedData(IntwentySettings settings, IServiceProvider services)
        {
          
        }

        public virtual async Task ConfigureDataBase(IntwentySettings settings, IServiceProvider services)
        {

            if (!settings.ConfigureDatabaseOnStartUp)
                return;

            var modelservice = services.GetRequiredService<IIntwentyModelService>();

            await modelservice.ConfigureDatabase();
        }

      

       
       

       

      
    }
}
