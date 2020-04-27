using System.Threading;
using System.Threading.Tasks;
using Intwenty.Data.DBAccess;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Intwenty.Data.Identity
{



    public class IntwentyRoleStore : IRoleStore<IntwentyRole>
    {

        private IntwentySettings Settings { get; }


        public IntwentyRoleStore(IOptions<IntwentySettings> settings)
        {
            Settings = settings.Value;
        }

        public Task<IdentityResult> CreateAsync(IntwentyRole role, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Insert(role);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(IntwentyRole role, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Delete(role);

            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<IntwentyRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var role = client.GetOne<IntwentyRole>(roleId);

            return Task.FromResult(role);
        }

        public Task<IntwentyRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var role = client.GetAll<IntwentyRole>().Find(p => p.NormalizedName == normalizedRoleName);

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
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Update(role);


            return Task.FromResult(IdentityResult.Success);
        }
    }
}
