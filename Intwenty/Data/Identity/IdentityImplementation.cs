using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intwenty.Data.DBAccess;
using Intwenty.Data.DBAccess.Annotations;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Engine;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Intwenty.Data.Identity
{

    [DbTableName("security_User")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyUser : IdentityUser
    {
       
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string APIKey { get; set; }

    }

    [DbTableName("security_Role")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyRole : IdentityRole
    {

    }

    [DbTableName("security_UserRoles")]
    [DbTablePrimaryKey("UserId,RoleId")]
    public class IntwentyUserRole : IdentityUserRole<string>
    {

    }



    public class IntwentyUserStore : IUserStore<IntwentyUser>, IUserPasswordStore<IntwentyUser>, IUserRoleStore<IntwentyUser>
    {
        private IntwentySettings Settings { get; }

        public IntwentyUserStore(IOptions<IntwentySettings> settings)
        {
            Settings = settings.Value;
        }

        public Task<IdentityResult> CreateAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Insert<IntwentyUser>(user);

            return Task.FromResult<IdentityResult>(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Delete(user);

            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<IntwentyUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var user = client.GetOne<IntwentyUser>(userId);

            return Task.FromResult(user);
        }

        public Task<IntwentyUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var user = client.GetAll<IntwentyUser>().Find(p => p.NormalizedUserName == normalizedUserName);

            return Task.FromResult(user);

        }

        public Task<string> GetNormalizedUserNameAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(IntwentyUser user, CancellationToken cancellationToken)
        {

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
  
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(IntwentyUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(IntwentyUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Update(user);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetPasswordHashAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != string.Empty);
        }

        public Task SetPasswordHashAsync(IntwentyUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task AddToRoleAsync(IntwentyUser user, string roleName, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var existingrole = client.GetAll<IntwentyRole>().Find(p => p.NormalizedName == roleName);
            if (existingrole == null)
                return Task.FromResult(IdentityResult.Failed(new IdentityError[] { new IdentityError() { Code = "NOROLE", Description=string.Format("There is nor role named {0}", roleName) } }));

            var existing_userrole = client.GetAll<IntwentyUserRole>().Find(p => p.UserId == user.Id && p.RoleId == existingrole.Id);
            if (existing_userrole != null)
                return Task.FromResult(IdentityResult.Success);

            var urole = new IntwentyUserRole() { RoleId = existingrole.Id, UserId = user.Id };
            client.Insert(urole);

            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            IList<string> result = new List<string>();
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var userroles = client.GetAll<IntwentyUserRole>();
            var roles = client.GetAll<IntwentyRole>();
            foreach (var ur in userroles)
            {
                if (ur.UserId == user.Id)
                {
                    var role = roles.Find(p => p.Id == ur.RoleId);
                    if (role == null)
                        continue;

                    if (result.Contains(role.Name))
                        continue;

                    result.Add(role.Name);
                }

            }

            return Task.FromResult(result);
        }

        public Task<IList<IntwentyUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            IList<IntwentyUser> result = new List<IntwentyUser>();
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var userroles = client.GetAll<IntwentyUserRole>();
            var users = client.GetAll<IntwentyUser>();
            var roles = client.GetAll<IntwentyRole>();
            foreach (var ur in userroles)
            {
                var role = roles.Find(p => p.Id == ur.RoleId);
                if (role == null)
                    continue;
                if (role.NormalizedName != roleName)
                    continue;

               var user = users.Find(p => p.Id == ur.UserId);
               if (user!= null)
                   result.Add(user);

            }

            return Task.FromResult(result);
        }

        public Task<bool> IsInRoleAsync(IntwentyUser user, string roleName, CancellationToken cancellationToken)
        {
            var userroles = GetRolesAsync(user, cancellationToken);
            return Task.FromResult(userroles.Result.Contains(roleName));
        }

        public Task RemoveFromRoleAsync(IntwentyUser user, string roleName, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var existingrole = client.GetAll<IntwentyRole>().Find(p => p.NormalizedName == roleName);
            if (existingrole == null)
                return Task.FromResult(IdentityResult.Failed(new IdentityError[] { new IdentityError() { Code = "NOROLE", Description = string.Format("There is nor role named {0}", roleName) } }));

            var existing_userrole = client.GetAll<IntwentyUserRole>().Find(p => p.UserId == user.Id && p.RoleId == existingrole.Id);
            if (existing_userrole == null)
                return Task.FromResult(IdentityResult.Success);

            client.Delete(existing_userrole);

            return Task.CompletedTask;
        }
    }

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
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Insert(role);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(IntwentyRole role, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
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
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var role = client.GetOne<IntwentyRole>(roleId);

            return Task.FromResult(role);
        }

        public Task<IntwentyRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
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
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Update(role);


            return Task.FromResult(IdentityResult.Success);
        }
    }
}
