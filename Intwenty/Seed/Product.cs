using Intwenty.Areas.Identity.Data;
using Intwenty.Areas.Identity.Entity;
using Intwenty.DataClient;
using Intwenty.Entity;
using Intwenty.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Seed
{
    public static class Product
    {
        public static async Task SeedProductAndOrganization(IServiceProvider services)
        {
            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();
            var productManager = services.GetRequiredService<IIntwentyProductManager>();
            var organizationManager = services.GetRequiredService<IIntwentyOrganizationManager>();

            if (!Settings.Value.SeedProductAndOrganizationOnStartUp)
                return;

            IntwentyProduct product = await productManager.FindByIdAsync(Settings.Value.ProductId);
            if (product == null)
            {
                product = new IntwentyProduct();
                product.Id = Settings.Value.ProductId;
                product.ProductName = Settings.Value.SiteTitle;
                await productManager.CreateAsync(product);
            }

            IntwentyOrganization org = await organizationManager.FindByNameAsync(Settings.Value.DefaultProductOrganization);
            if (org == null)
            {
                org = new IntwentyOrganization();
                org.Name = Settings.Value.DefaultProductOrganization;
                await organizationManager.CreateAsync(org);
            }

            var all_products = await organizationManager.GetProductsAsync(org.Id);
            var thisproduct = all_products.Find(p => p.ProductId == product.Id);
            if (thisproduct == null)
            {
                await organizationManager.AddProductAsync(new IntwentyOrganizationProduct() { OrganizationId = org.Id, ProductId = product.Id, ProductName = product.ProductName });
            }

        }

        public static async Task SeedProductAuthorizationItems(IServiceProvider services)
        {
            var Settings = services.GetRequiredService<IOptions<IntwentySettings>>();
            if (!Settings.Value.SeedDataOnStartUp)
                return;

            var client = new Connection(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);
            await client.OpenAsync();
            var current_apps = await client.GetEntitiesAsync<ApplicationItem>();
            var current_systems = await client.GetEntitiesAsync<SystemItem>();
            await client.CloseAsync();

            var iamclient = new Connection(Settings.Value.IAMConnectionDBMS, Settings.Value.IAMConnection);
            await iamclient.OpenAsync();
            var current_permissions = await iamclient.GetEntitiesAsync<IntwentyProductAuthorizationItem>();

            foreach (var t in current_systems)
            {
                if (!current_permissions.Exists(p => p.MetaCode == t.MetaCode && p.ProductId == Settings.Value.ProductId && p.AuthorizationType == SystemModelItem.MetaTypeSystem))
                {
                    var sysauth = new IntwentyProductAuthorizationItem() { Id = Guid.NewGuid().ToString(), Name = t.Title, NormalizedName = t.MetaCode.ToUpper(), AuthorizationType = SystemModelItem.MetaTypeSystem, ProductId = Settings.Value.ProductId };
                    await iamclient.InsertEntityAsync(sysauth);
                }
            }


            foreach (var t in current_apps)
            {
                if (!current_permissions.Exists(p => p.MetaCode == t.MetaCode && p.ProductId == Settings.Value.ProductId && p.AuthorizationType == ApplicationModelItem.MetaTypeApplication))
                {
                    var appauth = new IntwentyProductAuthorizationItem() { Id = Guid.NewGuid().ToString(), Name = t.Title, NormalizedName = t.MetaCode.ToUpper(), AuthorizationType = ApplicationModelItem.MetaTypeApplication, ProductId = Settings.Value.ProductId };
                    await iamclient.InsertEntityAsync(appauth);
                }
            }

            await iamclient.CloseAsync();

        }
    }
}
