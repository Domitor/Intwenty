using Intwenty.Areas.Identity.Entity;
using Intwenty.DataClient;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Intwenty.Areas.Identity.Data
{
    public interface IIntwentyProductManager
    {
        Task<IdentityResult> CreateAsync(IntwentyProduct product);
        Task<IdentityResult> DeleteAsync(IntwentyProduct product);
        Task<IntwentyProduct> FindByIdAsync(string productid);
        Task<List<IntwentyProduct>> GetProducts();
    }

    public class IntwentyProductManager : IIntwentyProductManager
    {

        private IntwentySettings Settings { get; }


        public IntwentyProductManager(IOptions<IntwentySettings> settings) 
        {
            Settings = settings.Value;
        }

        public async Task<IdentityResult> CreateAsync(IntwentyProduct product)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var t = await client.InsertEntityAsync(product);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IntwentyProduct product)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            await client.DeleteEntityAsync(product);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

       

        public async Task<IntwentyProduct> FindByIdAsync(string productid)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var products = await client.GetEntitiesAsync<IntwentyProduct>();
            var product = products.Find(p => p.Id == productid);
            await client.CloseAsync();
            return product;
        }

        public async Task<List<IntwentyProduct>> GetProducts()
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var products = await client.GetEntitiesAsync<IntwentyProduct>();
            await client.CloseAsync();
            return products;
        }

     



    }
}
