﻿using Intwenty.Areas.Identity.Entity;
using Intwenty.Areas.Identity.Pages.Account;
using Intwenty.DataClient;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Intwenty.Areas.Identity.Data
{
    public class IntwentyUserStore : IUserStore<IntwentyUser>, IUserPasswordStore<IntwentyUser>, IUserRoleStore<IntwentyUser>, IUserPhoneNumberStore<IntwentyUser>, 
                                     IUserEmailStore<IntwentyUser>, IUserAuthenticatorKeyStore<IntwentyUser>, IUserTwoFactorStore<IntwentyUser>, IUserLockoutStore<IntwentyUser>,
                                     IUserLoginStore<IntwentyUser>, IUserClaimStore<IntwentyUser>
    {
        private IntwentySettings Settings { get; }

        private IMemoryCache UserCache { get; }

        private static readonly string UsersCacheKey = "ALLUSERS";

        private static readonly string UserAuthCacheKey = "USERAUTH";


        public IntwentyUserStore(IOptions<IntwentySettings> settings, IMemoryCache cache)
        {
            Settings = settings.Value;
            UserCache = cache;
        }

        public async Task<IdentityResult> CreateAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            var allusers = await GetUsersAsync();
            if (allusers.Exists(p => p.UserName == user.UserName))
                return IdentityResult.Failed();

            UserCache.Remove(UsersCacheKey);


            await client.OpenAsync();
            await client.InsertEntityAsync(user);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            UserCache.Remove(UsersCacheKey);

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            await client.DeleteEntityAsync(user);
            await client.CloseAsync();
            return IdentityResult.Success;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<IntwentyUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            client.Open();
            var user = client.GetEntity<IntwentyUser>(userId);
            client.Close();
            return Task.FromResult(user);
        }

        public Task<IntwentyUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            client.Open();
            var user = client.GetEntities<IntwentyUser>().Find(p => p.NormalizedUserName == normalizedUserName);
            client.Close();
            return Task.FromResult(user);

        }

        public Task<string> GetNormalizedUserNameAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult<string>(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(IntwentyUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(IntwentyUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            UserCache.Remove(UsersCacheKey);

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            client.Open();
            client.UpdateEntity(user);
            client.Close();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetPasswordHashAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash != string.Empty);
        }

        public Task SetPasswordHashAsync(IntwentyUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task AddToRoleAsync(IntwentyUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

       

        public async Task<IList<string>> GetRolesAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<string> result = null;

            if (UserCache.TryGetValue(UserAuthCacheKey + "_" + user.Id, out result))
            {
                return result;
            }

            result = new List<string>();
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var userroles = await client.GetResultSetAsync(string.Format("SELECT AuthorizationItemNormalizedName FROM security_Authorization WHERE AuthorizationItemType='ROLE' AND UserId='{0}' AND ProductId='{1}'", user.Id, Settings.ProductId), false);
            await client.CloseAsync();

            //TODO: CHECK IF USER ORG HAS ROLE (USER IS IMPLICIT IN ROLE)

            foreach (var ur in userroles.Rows)
            {
                result.Add(ur.GetAsString("AuthorizationItemNormalizedName"));
            }

            UserCache.Set(UserAuthCacheKey + "_" + user.Id, result);

            return result;
        }

        public Task<IList<IntwentyUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            /*
            cancellationToken.ThrowIfCancellationRequested();
           

            IList<IntwentyUser> result = new List<IntwentyUser>();
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            client.Open();
            var userroles = client.GetEntities<IntwentyUserProductRole>().Where(p=> p.ProductId == Settings.ProductId).ToList();
            var users = client.GetEntities<IntwentyUser>();
            client.Close();

            var roles = GetRoles();

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
            */
        }

        public async Task<bool> IsInRoleAsync(IntwentyUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userroles = await GetRolesAsync(user, cancellationToken);
            var is_on_user = userroles.Contains(roleName);
            if (is_on_user)
                return is_on_user;


            //TODO: CHECK IF USER ORG HAS ROLE (USER IS IMPLICIT IN ROLE)


            return false;


        }

        public Task RemoveFromRoleAsync(IntwentyUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(IntwentyUser user, IntwentyProductAuthorizationItem role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

      

        public async Task<List<IntwentyUser>> GetUsersAsync()
        {
            List<IntwentyUser> res = null;

            if (UserCache.TryGetValue(UsersCacheKey, out res))
            {
                return res;
            }

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            await client.OpenAsync();
            var users = await client.GetEntitiesAsync<IntwentyUser>();
            await client.CloseAsync();
            UserCache.Set(UsersCacheKey, users);
            return users;

        }

        public Task SetPhoneNumberAsync(IntwentyUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(IntwentyUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetEmailAsync(IntwentyUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(IntwentyUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<IntwentyUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            client.Open();
            var user = client.GetEntities<IntwentyUser>().Find(p => p.NormalizedEmail == normalizedEmail);
            client.Close();

            return Task.FromResult(user);
        }

        public Task<string> GetNormalizedEmailAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(IntwentyUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        

        public Task SetAuthenticatorKeyAsync(IntwentyUser user, string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AuthenticatorKey = key;
            return Task.CompletedTask;
        }

        public Task<string> GetAuthenticatorKeyAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.AuthenticatorKey);
        }

        public Task SetTwoFactorEnabledAsync(IntwentyUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.TwoFactorEnabled = enabled;
            if (!enabled)
                user.AuthenticatorKey = "";

            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(IntwentyUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount +=1;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(IntwentyUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task AddLoginAsync(IntwentyUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {

            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            client.Open();
            client.InsertEntity(new IntwentyUserProductLogin() { Id = Guid.NewGuid().ToString(), LoginProvider = login.LoginProvider, ProviderDisplayName = login.ProviderDisplayName, ProviderKey = login.ProviderKey, UserId = user.Id   });
            client.Close();

            return Task.FromResult(true);
        }

        public Task RemoveLoginAsync(IntwentyUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            client.Open();
            var model = client.GetEntities<IntwentyUserProductLogin>().Find(p => p.UserId == user.Id && p.LoginProvider == loginProvider && p.ProviderKey == providerKey && p.ProductId == Settings.ProductId);
            client.Close();

            if (model != null)
            {
                client.Open();
                client.DeleteEntity<IntwentyUserProductLogin>(model);
                client.Close();
            }
            return Task.CompletedTask;
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<UserLoginInfo> result = null;
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            client.Open();
            result = client.GetEntities<IntwentyUserProductLogin>().Where(p => p.UserId == user.Id && p.ProductId == Settings.ProductId).Select(p => new UserLoginInfo(p.LoginProvider, p.ProviderKey, p.ProviderDisplayName)).ToList();
            client.Close();

            return Task.FromResult(result);
        }

        public async Task<IntwentyUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            client.Open();
            var login = client.GetEntities<IntwentyUserProductLogin>().Find(p => p.LoginProvider == loginProvider && p.ProviderKey == providerKey && p.ProductId == Settings.ProductId);
            client.Close();
            if (login != null)
            {
                client.Open();
                var user = client.GetEntity<IntwentyUser>(login.UserId);
                client.Close();
                return await Task.FromResult(user);
            }

            return null;
        }

        public Task<IList<Claim>> GetClaimsAsync(IntwentyUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<Claim> result = null;
            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            client.Open();
            result = client.GetEntities<IntwentyUserProductClaim>().Where(p => p.UserId == user.Id && p.ProductId == Settings.ProductId).Select(p=> p.ToClaim()).ToList();
            client.Close();
            return Task.FromResult(result);
            
        }

        public Task AddClaimsAsync(IntwentyUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            client.Open();
            foreach (var claim in claims)
            {
                client.InsertEntity(new IntwentyUserProductClaim() { ClaimType = claim.Type, ClaimValue = claim.Value,  UserId = user.Id, ProductId = Settings.ProductId   });
            }
            client.Close();
            return Task.FromResult(false);
        }

        public Task ReplaceClaimAsync(IntwentyUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            client.Open();
            var mclaims = client.GetEntities<IntwentyUserProductClaim>().Where(p => p.UserId == user.Id && p.ClaimValue == claim.Value && p.ClaimType == claim.Type && p.ProductId == Settings.ProductId).ToList();
            client.Close();
            foreach (var matchedClaim in mclaims)
            {
                matchedClaim.ClaimValue = newClaim.Value;
                matchedClaim.ClaimType = newClaim.Type;
            }

            return Task.CompletedTask;
        }

        public Task RemoveClaimsAsync(IntwentyUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);

            client.Open();
            foreach (var claim in claims)
            {
                
                var mclaims = client.GetEntities<IntwentyUserProductClaim>().Where(p => p.UserId == user.Id && p.ClaimValue == claim.Value && p.ClaimType == claim.Type && p.ProductId == Settings.ProductId).ToList();
                foreach (var c in mclaims)
                {
                    client.DeleteEntity(c);
                }
            }
            client.Close();

            return Task.CompletedTask;
        }

        public Task<IList<IntwentyUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var client = new Connection(Settings.IAMConnectionDBMS, Settings.IAMConnection);
            client.Open();
            var Users = client.GetEntities<IntwentyUser>();
            var UserClaims = client.GetEntities<IntwentyUserProductClaim>();
            client.Close();
            IList<IntwentyUser> query = (from userclaims in UserClaims
                        join user in Users on userclaims.UserId equals user.Id
                        where userclaims.ClaimValue == claim.Value
                        && userclaims.ClaimType == claim.Type && userclaims.ProductId == Settings.ProductId
                        select user).ToList();


            return Task.FromResult(query);
        }
    }
}
