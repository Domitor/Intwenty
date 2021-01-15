using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intwenty.Areas.Identity.Entity;
using Intwenty.DataClient;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Intwenty.Areas.Identity.Data
{



    public class IntwentyProductRoleStore : IRoleStore<IntwentyProductRole>
    {

        private static readonly string RolesCacheKey = "SYSROLES";

        private IntwentySettings Settings { get; }

        private IMemoryCache RoleCache { get; }

        public IntwentyProductRoleStore(IOptions<IntwentySettings> settings, IMemoryCache cache)
        {
            Settings = settings.Value;
            RoleCache = cache;
        }

        public async Task<IdentityResult> CreateAsync(IntwentyProductRole role, CancellationToken cancellationToken)
        {
            RoleCache.Remove(RolesCacheKey);

            role.ProductId = Settings.ProductId;

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            await client.InsertEntityAsync(role);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

       

        public async Task<IdentityResult> DeleteAsync(IntwentyProductRole role, CancellationToken cancellationToken)
        {
            RoleCache.Remove(RolesCacheKey);

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            await client.DeleteEntityAsync(role);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public async Task<IntwentyProductRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var roles = await client.GetEntitiesAsync<IntwentyProductRole>();
            var role = roles.Find(p => p.Id == roleId && p.ProductId == Settings.ProductId);
            await client.CloseAsync();
            return role;
        }

        public async Task<IntwentyProductRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var roles = await client.GetEntitiesAsync<IntwentyProductRole>();
            var role = roles.Find(p => p.NormalizedName == normalizedRoleName && p.ProductId == Settings.ProductId);
            await client.CloseAsync();
            return role;
        }

        public async Task<string> GetNormalizedRoleNameAsync(IntwentyProductRole role, CancellationToken cancellationToken)
        {
            return await Task.FromResult<string>(role.NormalizedName);
        }

        public async Task<string> GetRoleIdAsync(IntwentyProductRole role, CancellationToken cancellationToken)
        {
            return await Task.FromResult<string>(role.Id);
        }

        public Task<string> GetRoleNameAsync(IntwentyProductRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(IntwentyProductRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(IntwentyProductRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(IntwentyProductRole role, CancellationToken cancellationToken)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            await client.UpdateEntityAsync(role);
            await client.CloseAsync();
            return await Task.FromResult(IdentityResult.Success);
        }
    }
}
