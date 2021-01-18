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

    /// <summary>
    /// OBSOLETE 20210118
    /// </summary>
    public interface IIntwentyPermissionManager
    {
        Task<IdentityResult> CreateAsync(IntwentyProductAuthorizationItem permission);
        Task<IdentityResult> DeleteAsync(IntwentyProductAuthorizationItem permission);
        Task<IntwentyProductAuthorizationItem> FindByIdAsync(string permissionid);
        Task<List<IntwentyProductAuthorizationItem>> GetPermissions(string productid);
    }

    public class IntwentyPermissionManager : IIntwentyPermissionManager
    {

        private IntwentySettings Settings { get; }


        public IntwentyPermissionManager(IOptions<IntwentySettings> settings) 
        {
            Settings = settings.Value;
        }

        public async Task<IdentityResult> CreateAsync(IntwentyProductAuthorizationItem permission)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var t = await client.InsertEntityAsync(permission);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IntwentyProductAuthorizationItem permission)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            await client.DeleteEntityAsync(permission);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

       

        public async Task<IntwentyProductAuthorizationItem> FindByIdAsync(string permissionid)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var permissions = await client.GetEntitiesAsync<IntwentyProductAuthorizationItem>();
            var permission = permissions.Find(p => p.Id == permissionid);
            await client.CloseAsync();
            return permission;
        }

        public async Task<List<IntwentyProductAuthorizationItem>> GetPermissions(string productid)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var permissions = await client.GetEntitiesAsync<IntwentyProductAuthorizationItem>();
            await client.CloseAsync();
            return permissions;
        }

     



    }
}
