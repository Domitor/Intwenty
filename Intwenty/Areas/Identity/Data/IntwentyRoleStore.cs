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



    public class IntwentyRoleStore : IRoleStore<IntwentyRole>
    {

        private static readonly string RolesCacheKey = "SYSROLES";

        private IntwentySettings Settings { get; }

        private IMemoryCache RoleCache { get; }

        public IntwentyRoleStore(IOptions<IntwentySettings> settings, IMemoryCache cache)
        {
            Settings = settings.Value;
            RoleCache = cache;
        }

        public Task<IdentityResult> CreateAsync(IntwentyRole role, CancellationToken cancellationToken)
        {
            RoleCache.Remove(RolesCacheKey);

            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            client.Open();
            client.InsertEntity(role);
            client.Close();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(IntwentyRole role, CancellationToken cancellationToken)
        {
            RoleCache.Remove(RolesCacheKey);

            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            client.Open();
            client.DeleteEntity(role);
            client.Close();
            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<IntwentyRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            client.Open();
            var role = client.GetEntity<IntwentyRole>(roleId);
            client.Close();
            return Task.FromResult(role);
        }

        public Task<IntwentyRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            client.Open();
            var role = client.GetEntities<IntwentyRole>().Find(p => p.NormalizedName == normalizedRoleName);
            client.Close();
            return Task.FromResult(role);
        }

        public Task<string> GetNormalizedRoleNameAsync(IntwentyRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(IntwentyRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(role.Id);
        }

        public Task<string> GetRoleNameAsync(IntwentyRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(IntwentyRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(IntwentyRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IntwentyRole role, CancellationToken cancellationToken)
        {
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            client.Open();
            client.UpdateEntity(role);
            client.Close();
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
