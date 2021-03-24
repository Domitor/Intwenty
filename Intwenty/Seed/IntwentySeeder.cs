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

            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFATOTPUNVERIFIED", Text = "Koden kunde inte verifiera testa nästa kod i din app." });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFATOTPUNVERIFIED", Text = "The code could not be verified, try the next code from your authenticator app." });

            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFATOTPSTEPS_1", Text = "För att använda en autentiserings app, går du igenom följande steg:" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFATOTPSTEPS_1", Text = "To use an authenticator app go through the following steps:" });

            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFATOTPSTEPS_2", Text = "Ladda den en autentiserings app (Microsoft Authenticator or Google Authenticator) till din mobiltelefon" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFATOTPSTEPS_2", Text = "Download an authentication app (Microsoft Authenticator or Google Authenticator) to your phone" });

            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFATOTPSTEPS_3", Text = "Skanna QR koden nedan eller ange denna kod" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFATOTPSTEPS_3", Text = "Scan the QR Code or enter this key" });

            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFATOTPSTEPS_4", Text = "i din autentiserings app i telefonen" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFATOTPSTEPS_4", Text = "into your two factor authenticator app" });

            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFATOTPSTEPS_5", Text = "När du skannat qr-koden eller matat in koden kommer din autentiserings app att ge dig en unik kod, ange den koden i rutan nedan och klicka på skicka" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFATOTPSTEPS_5", Text = "Once you have scanned the QR code or input the key above, your two factor authentication app will provide you with a unique code.Enter the code in the confirmation box below." });


            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFASMSCODESENT", Text = "Vi har skickat en kod till ditt telefonnummer, vänligen ange det nedan." });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFASMSCODESENT", Text = "We've sent a code to your phone, input the code below" });

            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFASMSPHONEREQ", Text = "Vänligen ange det telefonnummer du vill använda ör att ta emot koder." });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFASMSPHONEREQ", Text = "Please input the phonenumber to use with sms 2FA." });

            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Verify", Text = "Verifiera" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Code", Text = "Kod" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Invalid format", Text = "Ogiltigt format" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "You must update your email first, do it by", Text = "Du måste ange en epostadress först, det gär du genom att" });

            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Thank you. Two-factor authentication is now set up", Text = "Tack 2-faktor autentisering är nu aktiverad." });

            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFANOTVERIFIED", Text = "Koden kunde inte verifieras, du kan försöka igen genom att" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFANOTVERIFIED", Text = "The code could not be verified, try again by" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "clicking here", Text = "Klicka här" });

            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFAEMALCODESENT", Text = "Vi har skickat en kod till din epostadress, vänligen mata in koden nedan." });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFAEMALCODESENT", Text = "We've sent a code to your email, please input that code and push verify." });

            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Reset 2FA", Text = "Återställ 2FA" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Disable 2FA", Text = "Avaktivera 2FA" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFARESETINFO", Text = "Denna åtgärd tar bort dina 2FA inställningar. För att bibehålla en hög säkerhet bör du aktivera 2FA igen senare" });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFARESETINFO", Text = "This action disables your 2FA settings. To achive the highest security you should set it up again later." });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFAFORCEDRESETINFO", Text = "Denna åtgärd tar bort dina 2FA inställningar. Eftersom vår tjänst använder tvingande 2FA måste du göra om konfigurationen för att kunna logga in. Om du använder en app måste du även ta bort kontot i appen och skapa ett nytt." });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFAFORCEDRESETINFO", Text = "This action resets your 2FA settings. Since our service is using enforced 2FA you must configure it again in order to login." });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "You're using Bank ID 2FA", Text = "Du använder 2FA via Bank ID" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "You're using Email 2FA", Text = "Du använder 'Kod via Epost' 2FA" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "You're using FIDO2 2FA", Text = "Du använder FIDO2 2FA" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "You're using SMS Code 2FA", Text = "Du använder 'Kod via SMS' 2FA" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "You're using an authentication app for 2FA", Text = "Du använder 2FA via en autentiserings app" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFASTATUS", Text = "Du är klar. När du loggar in kommer du efterfrågas en extra säkerhetskod som levereras till dig på det sätt du valt." });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFASTATUS", Text = "You're done. Next time you login you will be promted for a security code." });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Enable Email Code 2FA", Text = "Aktivera 'Kod via Epost' 2FA" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Enable FIDO2 2FA", Text = "Aktivera 'FIDO2' 2FA" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Enable SMS Code 2FA", Text = "Aktivera 'Kod via SMS' 2FA" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Enable Bank ID 2FA", Text = "Aktivera 2FA via Bank ID" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Enable Microsoft Authenticator", Text = "Aktivera Microsoft Authenticator 2FA" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Enable Google Authenticator", Text = "Aktivera Google Authenticator 2FA" });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFAOPTIONAL", Text = "Genom att aktivera två-faktor autentisering så höjer du säkerheten på ditt konto, vänligen välj ett av alternativen nedan." });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFAOPTIONAL", Text = "By adding two-factor authentication you can add extra security to your account. Please choose one option below." });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "MFAFORCED", Text = "För att nå en hög säkerhet använder vi tvingande 2-faktor autentisering. För att använda tjänsten måste du välja ett alternativ nedan." });
            temp.Add(new TranslationItem() { Culture = "en-US", TransKey = "MFAFORCED", Text = "To add extra security, our service is using enforced 2FA. To use the service you must enable one of the options below." });
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Do you want to rename", Text = "Vill du byta namn på" });
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
            temp.Add(new TranslationItem() { Culture = "sv-SE", TransKey = "Send", Text = "Skicka" });
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
