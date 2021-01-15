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
    public interface IIntwentyPermissionManager
    {
        Task<IdentityResult> CreateAsync(IntwentyProductPermission permission);
        Task<IdentityResult> DeleteAsync(IntwentyProductPermission permission);
        Task<IntwentyProductPermission> FindByIdAsync(string permissionid);
        Task<List<IntwentyProductPermission>> GetPermissions(string productid);
    }

    public class IntwentyPermissionManager : IIntwentyPermissionManager
    {

        private IntwentySettings Settings { get; }


        public IntwentyPermissionManager(IOptions<IntwentySettings> settings) 
        {
            Settings = settings.Value;
        }

        public async Task<IdentityResult> CreateAsync(IntwentyProductPermission permission)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var t = await client.InsertEntityAsync(permission);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IntwentyProductPermission permission)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            await client.DeleteEntityAsync(permission);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

       

        public async Task<IntwentyProductPermission> FindByIdAsync(string permissionid)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var permissions = await client.GetEntitiesAsync<IntwentyProductPermission>();
            var permission = permissions.Find(p => p.Id == permissionid);
            await client.CloseAsync();
            return permission;
        }

        public async Task<List<IntwentyProductPermission>> GetPermissions(string productid)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var permissions = await client.GetEntitiesAsync<IntwentyProductPermission>();
            await client.CloseAsync();
            return permissions;
        }

     



    }
}
