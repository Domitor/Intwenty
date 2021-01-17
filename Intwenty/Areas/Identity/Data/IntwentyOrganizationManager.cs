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
using Intwenty.Areas.Identity.Models;

namespace Intwenty.Areas.Identity.Data
{
    public interface IIntwentyOrganizationManager
    {
        Task<IdentityResult> CreateAsync(IntwentyOrganization organization);
        Task<IdentityResult> UpdateAsync(IntwentyOrganization organization);
        Task<IdentityResult> DeleteAsync(IntwentyOrganization organization);
        Task<IntwentyOrganization> FindByIdAsync(int id);
        Task<List<IntwentyOrganization>> GetAllAsync();
        Task<List<IntwentyOrganizationMember>> GetMembersAsync(int organizationid);
        Task<IdentityResult> AddMemberAsync(IntwentyOrganizationMember member);
        Task<IdentityResult> RemoveMemberAsync(IntwentyOrganizationMember member);
        Task<List<IntwentyOrganizationProduct>> GetProductsAsync(int organizationid);
        Task<IdentityResult> AddProductAsync(IntwentyOrganizationProduct product);
        Task<IdentityResult> RemoveProductAsync(IntwentyOrganizationProduct product);
    }

    public class IntwentyOrganizationManager : IIntwentyOrganizationManager
    {

        private IntwentySettings Settings { get; }


        public IntwentyOrganizationManager(IOptions<IntwentySettings> settings) 
        {
            Settings = settings.Value;
        }

        public async Task<IdentityResult> CreateAsync(IntwentyOrganization organization)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var t = await client.InsertEntityAsync(organization);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(IntwentyOrganization organization)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var t = await client.UpdateEntityAsync(organization);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IntwentyOrganization organization)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            await client.DeleteEntityAsync(organization);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

       

        public async Task<IntwentyOrganization> FindByIdAsync(int id)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var org = await client.GetEntityAsync<IntwentyOrganization>(id);
            await client.CloseAsync();
            return org;
        }

        public async Task<List<IntwentyOrganization>> GetAllAsync()
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var orgs = await client.GetEntitiesAsync<IntwentyOrganization>();
            await client.CloseAsync();
            return orgs;
        }

        public async Task<List<IntwentyOrganizationMember>> GetMembersAsync(int organizationid)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var members = await client.GetEntitiesAsync<IntwentyOrganizationMember>();
            await client.CloseAsync();
            return members.Where(p => p.OrganizationId == organizationid).ToList(); 
        }

        public async Task<IdentityResult> AddMemberAsync(IntwentyOrganizationMember member)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var allexisting = await client.GetEntitiesAsync<IntwentyOrganizationMember>();
            await client.CloseAsync();
            if (allexisting.Exists(p=> p.OrganizationId == member.OrganizationId && p.UserId == member.UserId))
                return IdentityResult.Success;

            await client.OpenAsync();
            var t = await client.InsertEntityAsync(member);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveMemberAsync(IntwentyOrganizationMember member)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var allexisting = await client.GetEntitiesAsync<IntwentyOrganizationMember>();
            await client.CloseAsync();
            var current = allexisting.Find(p => p.OrganizationId == member.OrganizationId && p.UserId == member.UserId);
            if (current==null)
                return IdentityResult.Success;

            await client.OpenAsync();
            var t = await client.DeleteEntityAsync(current);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

        public async Task<List<IntwentyOrganizationProduct>> GetProductsAsync(int organizationid)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var members = await client.GetEntitiesAsync<IntwentyOrganizationProduct>();
            await client.CloseAsync();
            return members.Where(p => p.OrganizationId == organizationid).ToList();
        }

        public async Task<IdentityResult> AddProductAsync(IntwentyOrganizationProduct product)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var allexisting = await client.GetEntitiesAsync<IntwentyOrganizationProduct>();
            await client.CloseAsync();
            if (allexisting.Exists(p => p.OrganizationId == product.OrganizationId && p.ProductId == product.ProductId))
                return IdentityResult.Success;

            await client.OpenAsync();
            var t = await client.InsertEntityAsync(product);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveProductAsync(IntwentyOrganizationProduct product)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var allexisting = await client.GetEntitiesAsync<IntwentyOrganizationProduct>();
            await client.CloseAsync();
            var current = allexisting.Find(p => p.OrganizationId == product.OrganizationId && p.ProductId == product.ProductId);
            if (current == null)
                return IdentityResult.Success;

            await client.OpenAsync();
            var t = await client.DeleteEntityAsync(current);
            await client.CloseAsync();
            return IdentityResult.Success;
        }


    }
}
