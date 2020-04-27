using Intwenty.Data.DBAccess;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Intwenty.Data.Identity
{
    public class IntwentyUserStore : IUserStore<IntwentyUser>, IUserPasswordStore<IntwentyUser>, IUserRoleStore<IntwentyUser>, IUserPhoneNumberStore<IntwentyUser>, IUserEmailStore<IntwentyUser>
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
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Insert<IntwentyUser>(user);

            return Task.FromResult<IdentityResult>(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
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
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var user = client.GetOne<IntwentyUser>(userId);

            return Task.FromResult(user);
        }

        public Task<IntwentyUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
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
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
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
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var existingrole = client.GetAll<IntwentyRole>().Find(p => p.NormalizedName == roleName);
            if (existingrole == null)
                return Task.FromResult(IdentityResult.Failed(new IdentityError[] { new IdentityError() { Code = "NOROLE", Description = string.Format("There is nor role named {0}", roleName) } }));

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
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
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
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
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
                if (user != null)
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
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
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

        public Task SetPhoneNumberAsync(IntwentyUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(IntwentyUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetEmailAsync(IntwentyUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(IntwentyUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<IntwentyUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var user = client.GetAll<IntwentyUser>().Find(p => p.NormalizedEmail == normalizedEmail);

            return Task.FromResult(user);
        }

        public Task<string> GetNormalizedEmailAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(IntwentyUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }
    }
}
