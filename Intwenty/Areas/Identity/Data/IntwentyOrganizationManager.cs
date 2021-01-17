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
    public interface IIntwentyOrganizationManager
    {
        Task<IdentityResult> CreateAsync(IntwentyOrganization product);
        Task<IdentityResult> UpdateAsync(IntwentyOrganization product);
        Task<IdentityResult> DeleteAsync(IntwentyOrganization product);
        Task<IntwentyOrganization> FindByIdAsync(int id);
        Task<List<IntwentyOrganization>> GetAll();
    }

    public class IntwentyOrganizationManager : IIntwentyOrganizationManager
    {

        private IntwentySettings Settings { get; }


        public IntwentyOrganizationManager(IOptions<IntwentySettings> settings) 
        {
            Settings = settings.Value;
        }

        public async Task<IdentityResult> CreateAsync(IntwentyOrganization product)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var t = await client.InsertEntityAsync(product);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(IntwentyOrganization product)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var t = await client.UpdateEntityAsync(product);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IntwentyOrganization product)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            await client.DeleteEntityAsync(product);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

       

        public async Task<IntwentyOrganization> FindByIdAsync(int id)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var orgs = await client.GetEntitiesAsync<IntwentyOrganization>();
            var org = orgs.Find(p => p.Id == id);
            await client.CloseAsync();
            return org;
        }

        public async Task<List<IntwentyOrganization>> GetAll()
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var orgs = await client.GetEntitiesAsync<IntwentyOrganization>();
            await client.CloseAsync();
            return orgs;
        }

     



    }
}
