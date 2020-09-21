using Intwenty.Areas.Identity.Models;
using Intwenty.Data.DBAccess;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Areas.Identity.Data
{
    public class IntwentyUserManager : UserManager<IntwentyUser>
    {

        private IntwentySettings Settings { get; }

        public IntwentyUserManager(IUserStore<IntwentyUser> store, 
                                   IOptions<IdentityOptions> optionsAccessor, 
                                   IPasswordHasher<IntwentyUser> passwordHasher, 
                                   IEnumerable<IUserValidator<IntwentyUser>> userValidators, 
                                   IEnumerable<IPasswordValidator<IntwentyUser>> passwordValidators, 
                                   ILookupNormalizer keyNormalizer, 
                                   IdentityErrorDescriber errors, 
                                   IServiceProvider services, 
                                   ILogger<UserManager<IntwentyUser>> logger,
                                   IOptions<IntwentySettings> settings)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            Settings = settings.Value;
        }
      

        public Task<IntwentyGroup> AddGroupAsync(string groupname)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var t = new IntwentyGroup();
            t.Id = Guid.NewGuid().ToString();
            t.Name = groupname;
            var user = client.Insert(t);

         

            return Task.FromResult(t);
        }

        public Task<IntwentyGroup> GetGroupByNameAsync(string groupname)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var t = client.GetAll<IntwentyGroup>().Find(p => p.Name.ToUpper() == groupname.ToUpper());

            return Task.FromResult(t);
        }

        public Task<IntwentyGroup> GetGroupByIdAsync(string groupid)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var t = client.GetAll<IntwentyGroup>().Find(p => p.Id== groupid);

            return Task.FromResult(t);
        }

        public Task<IdentityResult> AddGroupMemberAsync(IntwentyUser user, IntwentyGroup group, string membershiptype, string membershipstatus)
        {
            if (user == null || group == null)
                throw new InvalidOperationException("Error when adding member to group.");

            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var check = client.GetAll<IntwentyUserGroup>().Exists(p => p.UserName.ToUpper() == user.UserName.ToUpper() && p.GroupName.ToUpper() == group.Name.ToUpper());
            if (check)
                return Task.FromResult(IdentityResult.Success);

            var t = new IntwentyUserGroup();
            t.Id = Guid.NewGuid().ToString();
            t.UserId = user.Id;
            t.UserName = user.UserName;
            t.GroupId = group.Id;
            t.GroupName = group.Name;
            t.MembershipType = membershiptype;
            t.MembershipStatus = membershipstatus;
            client.Insert(t);


            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateGroupMembershipAsync(IntwentyUser user, string groupname, string membershipstatus)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var result = IdentityResult.Success;
            var t = client.GetAll<IntwentyUserGroup>().Find(p => p.GroupName.ToUpper() == groupname.ToUpper() && p.UserId == user.Id);
            if (t != null)
            {
                t.MembershipStatus = membershipstatus;
                client.Update(t);
            }
            else
            {
                result = IdentityResult.Failed();
            }
            return Task.FromResult(result);
        }

        public Task<IdentityResult> UpdateGroupMembershipAsync(IntwentyUser user, IntwentyGroup group, string membershipstatus)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var result = IdentityResult.Success;
            var t = client.GetAll<IntwentyUserGroup>().Find(p => p.GroupId == group.Id && p.UserId == user.Id);
            if (t != null)
            {
                t.MembershipStatus = membershipstatus;
                client.Update(t);
            }
            else
            {
                result = IdentityResult.Failed();
            }
            return Task.FromResult(result);
        }

        public Task<IdentityResult> ChangeGroupNameAsync(string groupid, string newgroupname)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var result = IdentityResult.Success;
            var t = client.GetOne<IntwentyGroup>(groupid);
            if (t != null)
            {
                t.Name = newgroupname;
                client.Update(t);

                var l = client.GetAll<IntwentyUserGroup>();
                foreach (var g in l)
                {
                    if (g.GroupId == groupid)
                    {
                        g.GroupName = newgroupname;
                        client.Update(g);
                    }
                }
            }
            else
            {
                result = IdentityResult.Failed();
            }

            return Task.FromResult(result);
        }

        public Task<bool> GroupExists(string groupname)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var t = client.GetAll<IntwentyGroup>().Exists(p => p.Name.ToUpper() == groupname.ToUpper());
            return Task.FromResult(t);
        }

        public Task<List<IntwentyUserGroup>> GetUserGroups(IntwentyUser user)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var t = client.GetAll<IntwentyUserGroup>().Where(p => p.UserId == user.Id).ToList();
            return Task.FromResult(t);

        }


        public Task<List<IntwentyUserGroup>> GetGroupMembers(IntwentyGroup group)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var t = client.GetAll<IntwentyUserGroup>().Where(p => p.GroupId == group.Id);
            return Task.FromResult(t.ToList());

        }

        public Task<bool> IsWaitingToJoinGroup(string username)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var t = client.GetAll<IntwentyUserGroup>().Exists(p => p.UserName.ToUpper() == username.ToUpper() && p.MembershipStatus == "WAITING");
            if (client.GetAll<IntwentyUserGroup>().Exists(p => p.UserName.ToUpper() == username.ToUpper() && p.MembershipStatus == "ACCEPTED"))
                t = false;

            return Task.FromResult(t);

        }

        public Task<IdentityResult> RemoveFromGroupAsync(string userid, string groupid)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var t = client.GetAll<IntwentyUserGroup>().Find(p => p.UserId == userid && p.GroupId == groupid);

            client.Delete(t);

            return Task.FromResult(IdentityResult.Success);

        }


        public override Task<IdentityResult> DeleteAsync(IntwentyUser user)
        {
           
            var t = base.DeleteAsync(user);
            if (t.Result.Succeeded)
            {
                if (t != null && t.Result != null)
                {
                    IIntwentyDbORM client;
                    if (Settings.IsNoSQL)
                        client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
                    else
                        client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

                    var logins = client.GetAll<IntwentyUserLogin>().Where(p => p.UserId == user.Id);
                    foreach (var l in logins)
                    {
                        client.Delete(l);
                    }
                    /*
                    var usergroup = GetUserGroup(user);
                    if (usergroup!= null && usergroup.Result != null)
                        client.Delete(usergroup.Result);
                    */
                }

                return t;
            }
            else
            {
                return t;
            }
        }

       



    }
}
