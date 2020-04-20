using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intwenty.Data.DBAccess;
using Intwenty.Data.DBAccess.Annotations;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Engine;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shared;

namespace Intwenty.Data.Identity
{

    [DbTableName("security_User")]
    [DbTablePrimaryKey("Id")]
    public class SystemUser : IdentityUser
    {
       
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string APIKey { get; set; }

    }

    [DbTableName("security_Role")]
    [DbTablePrimaryKey("Id")]
    public class SystemRole : IdentityRole
    {

    }

    [DbTableName("security_UserRoles")]
    [DbTablePrimaryKey("UserId,RoleId")]
    public class SystemUserRole : IdentityUserRole<string>
    {

    }



    public class IntwentyUserStore : IUserStore<SystemUser>, IUserPasswordStore<SystemUser>, IUserRoleStore<SystemUser>
    {
        private SystemSettings Settings { get; }

        private ConnectionStrings DbConnections { get; }

        public IntwentyUserStore(IOptions<SystemSettings> settings, IOptions<ConnectionStrings> connections)
        {
            Settings = settings.Value;
            DbConnections = connections.Value;
        }

        public Task<IdentityResult> CreateAsync(SystemUser user, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            client.Insert<SystemUser>(user);

            return Task.FromResult<IdentityResult>(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(SystemUser user, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            client.Delete(user);

            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<SystemUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            var user = client.GetOne<SystemUser>(userId);

            return Task.FromResult(user);
        }

        public Task<SystemUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            var user = client.GetAll<SystemUser>().Find(p => p.NormalizedUserName == normalizedUserName);

            return Task.FromResult(user);

        }

        public Task<string> GetNormalizedUserNameAsync(SystemUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(SystemUser user, CancellationToken cancellationToken)
        {

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(SystemUser user, CancellationToken cancellationToken)
        {
  
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(SystemUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(SystemUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(SystemUser user, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            client.Update(user);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetPasswordHashAsync(SystemUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(SystemUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != string.Empty);
        }

        public Task SetPasswordHashAsync(SystemUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task AddToRoleAsync(SystemUser user, string roleName, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            var existingrole = client.GetAll<SystemRole>().Find(p => p.NormalizedName == roleName);
            if (existingrole == null)
                return Task.FromResult(IdentityResult.Failed(new IdentityError[] { new IdentityError() { Code = "NOROLE", Description=string.Format("There is nor role named {0}", roleName) } }));

            var existing_userrole = client.GetAll<SystemUserRole>().Find(p => p.UserId == user.Id && p.RoleId == existingrole.Id);
            if (existing_userrole != null)
                return Task.FromResult(IdentityResult.Success);

            var urole = new SystemUserRole() { RoleId = existingrole.Id, UserId = user.Id };
            client.Insert(urole);

            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(SystemUser user, CancellationToken cancellationToken)
        {
            IList<string> result = new List<string>();
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            var userroles = client.GetAll<SystemUserRole>();
            var roles = client.GetAll<SystemRole>();
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

        public Task<IList<SystemUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            IList<SystemUser> result = new List<SystemUser>();
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            var userroles = client.GetAll<SystemUserRole>();
            var users = client.GetAll<SystemUser>();
            var roles = client.GetAll<SystemRole>();
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

        public Task<bool> IsInRoleAsync(SystemUser user, string roleName, CancellationToken cancellationToken)
        {
            var userroles = GetRolesAsync(user, cancellationToken);
            return Task.FromResult(userroles.Result.Contains(roleName));
        }

        public Task RemoveFromRoleAsync(SystemUser user, string roleName, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            var existingrole = client.GetAll<SystemRole>().Find(p => p.NormalizedName == roleName);
            if (existingrole == null)
                return Task.FromResult(IdentityResult.Failed(new IdentityError[] { new IdentityError() { Code = "NOROLE", Description = string.Format("There is nor role named {0}", roleName) } }));

            var existing_userrole = client.GetAll<SystemUserRole>().Find(p => p.UserId == user.Id && p.RoleId == existingrole.Id);
            if (existing_userrole == null)
                return Task.FromResult(IdentityResult.Success);

            client.Delete(existing_userrole);

            return Task.CompletedTask;
        }
    }

    public class IntwentyRoleStore : IRoleStore<SystemRole>
    {

        private SystemSettings Settings { get; }

        private ConnectionStrings DbConnections { get; }

        public IntwentyRoleStore(IOptions<SystemSettings> settings, IOptions<ConnectionStrings> connections)
        {
            Settings = settings.Value;
            DbConnections = connections.Value;
        }

        public Task<IdentityResult> CreateAsync(SystemRole role, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            client.Insert(role);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(SystemRole role, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            client.Delete(role);

            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<SystemRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            var role = client.GetOne<SystemRole>(roleId);

            return Task.FromResult(role);
        }

        public Task<SystemRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            var role = client.GetAll<SystemRole>().Find(p => p.NormalizedName == normalizedRoleName);

            return Task.FromResult(role);
        }

        public Task<string> GetNormalizedRoleNameAsync(SystemRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(SystemRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(role.Id);
        }

        public Task<string> GetRoleNameAsync(SystemRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(SystemRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(SystemRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(SystemRole role, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection, "IntwentyDb");
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, DbConnections.DefaultConnection);

            client.Update(role);


            return Task.FromResult(IdentityResult.Success);
        }
    }
}
