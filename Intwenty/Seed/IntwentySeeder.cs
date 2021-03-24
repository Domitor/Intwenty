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

    
             var temp = new List<TranslationItem>();

            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Do you want to rename", Text = "Vill du byta namn" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "You must type a new groupname", Text = "Du måste ange ett nytt gruppnamn" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Invite member", Text = "Bjud in medlem" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "You must type a valid email", Text = "Du måste ange en korrekt epostadress" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "New Groupname", Text = "Nytt gruppnamn" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Type a new groupname for", Text = "Skriv ett nytt gruppnamn för" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "GROUPINVITESENT", Text = "An inbjudan att delta i gruppen har skickats till" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "GROUPINVITESENT", Text = "An invite to join the group has been sent to" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "GROUPINVITEINSTRUCT", Text = "Skriv epostadressen till en person att bjuda in till" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "GROUPINVITEINSTRUCT", Text = "Type the email of a person to invite to" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "GROUPREQTOOWNERSENT", Text = "En förfrågan om att bli medlem har skickats till gruppens ägare." });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "GROUPREQTOOWNERSENT", Text = "A request to join the group has been sent to the owner." });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Become a member of the group", Text = "Bli medlem i gruppen" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Do you want to create the group", Text = "Vill du skapa gruppen" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Group", Text = "Grupp" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Create or Join", Text = "Skapa eller gå med" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Groupname", Text = "Gruppnamn" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Create Group", Text = "Skapa grupp" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Join Group", Text = "Gå med i grupp" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "My Groups", Text = "Mina grupper" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "My role", Text = "Min roll" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Invite Member", Text = "Bjud in medlem" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Accept", Text = "Acceptera" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Members", Text = "Medlemmar" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Members of my groups", Text = "Medlemmar i mina grupper" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Role", Text = "Roll" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Members in", Text = "Medlemmar i" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Change Password", Text = "Ändra lösenord" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Current Password", Text = "Nuvarande lösenord" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "New Password", Text = "Nytt lösenord" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Confirm Password", Text = "Bekräfta lösenord" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Account settings", Text = "kontoinställningar" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Manage Account", Text = "Hantera ditt konto" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Create Key", Text = "Skapa nyckel" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Renew Key", Text = "Förnya nyckel" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "First Name", Text = "Förnamn" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Last Name", Text = "Efternamn" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Profile", Text = "Mina uppgifter" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Language", Text = "Språk" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Two-factor authentication", Text = "2-faktor autentisering" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "API Key", Text = "API nyckel" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Groups", Text = "Grupper" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Personal data", Text = "Personlig data" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Use external accounts", Text = "Använd ett externt konto" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Register as a new user", Text = "Skapa konto" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Forgot your password", Text = "Glömt lösenordet" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Remember Me", Text = "Kom i håg mig" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Password", Text = "Lösenord" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "User Name", Text = "Användarnamn" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Log In", Text = "Logga In" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Code", Text = "Kod" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Address", Text = "Adress" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Message", Text = "Meddelande" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Email", Text = "Epost" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Phone", Text = "Telefon" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Previous", Text = "Föregående" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Next", Text = "Nästa" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Changed By", Text = "Ändrad Av" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Changed", Text = "Ändrad" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "List", Text = "Lista" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Add", Text = "Lägg till" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Open", Text = "Öppna" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Close", Text = "Stäng" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Title", Text = "Titel" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "Delete", Text = "Delete ?" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Remove", Text = "Ta bort" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Delete", Text = "Ta bort ?" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Cancel", Text = "Avbryt" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Date", Text = "Datum" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Event", Text = "Händelse" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Leave", Text = "Lämna" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Name", Text = "Namn" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Rename", Text = "Ändra namn" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Create new", Text = "Skapa ny" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "New", Text = "Ny" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Save", Text = "Spara" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Changes Saved", Text = "Ändringar sparade" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Edit", Text = "Ändra" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Menu", Text = "Meny" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "to", Text = "till" });
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

                await client.CloseAsync();

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


     

        public virtual Task SeedModel(IntwentySettings settings, IServiceProvider services)
        {
            return Task.CompletedTask;
        }

        public virtual Task SeedData(IntwentySettings settings, IServiceProvider services)
        {
            return Task.CompletedTask;
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
