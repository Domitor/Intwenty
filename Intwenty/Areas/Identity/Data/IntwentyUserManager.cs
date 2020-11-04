using Intwenty.Areas.Identity.Models;
using Intwenty.DataClient;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            var t = new IntwentyGroup();
            t.Id = Guid.NewGuid().ToString();
            t.Name = groupname;
            client.Open();
            var user = client.InsertEntity(t);
            client.Close();
            return Task.FromResult(t);
        }

        public Task<IntwentyGroup> GetGroupByNameAsync(string groupname)
        {
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            client.Open();
            var t = client.GetEntities<IntwentyGroup>().Find(p => p.Name.ToUpper() == groupname.ToUpper());
            client.Close();
            return Task.FromResult(t);
        }

        public Task<IntwentyGroup> GetGroupByIdAsync(string groupid)
        {
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            client.Open();
            var t = client.GetEntities<IntwentyGroup>().Find(p => p.Id== groupid);
            client.Close();
            return Task.FromResult(t);
        }

        public Task<IdentityResult> AddGroupMemberAsync(IntwentyUser user, IntwentyGroup group, string membershiptype, string membershipstatus)
        {
            if (user == null || group == null)
                throw new InvalidOperationException("Error when adding member to group.");

            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            client.Open();
            var check = client.GetEntities<IntwentyUserGroup>().Exists(p => p.UserName.ToUpper() == user.UserName.ToUpper() && p.GroupName.ToUpper() == group.Name.ToUpper());
            client.Close();
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
            client.Open();
            client.InsertEntity(t);
            client.Close();


            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateGroupMembershipAsync(IntwentyUser user, string groupname, string membershipstatus)
        {
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            var result = IdentityResult.Success;
            
            client.Open();
            var t = client.GetEntities<IntwentyUserGroup>().Find(p => p.GroupName.ToUpper() == groupname.ToUpper() && p.UserId == user.Id);
            if (t != null)
            {
                t.MembershipStatus = membershipstatus;
                client.UpdateEntity(t);
            }
            else
            {
                result = IdentityResult.Failed();
            }
            client.Close();

            return Task.FromResult(result);
        }

        public Task<IdentityResult> UpdateGroupMembershipAsync(IntwentyUser user, IntwentyGroup group, string membershipstatus)
        {
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            var result = IdentityResult.Success;

            client.Open();
            var t = client.GetEntities<IntwentyUserGroup>().Find(p => p.GroupId == group.Id && p.UserId == user.Id);
            if (t != null)
            {
                t.MembershipStatus = membershipstatus;
                client.UpdateEntity(t);
            }
            else
            {
                result = IdentityResult.Failed();
            }
            client.Close();

            return Task.FromResult(result);
        }

        public Task<IdentityResult> ChangeGroupNameAsync(string groupid, string newgroupname)
        {
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            var result = IdentityResult.Success;

            client.Open();
            var t = client.GetEntity<IntwentyGroup>(groupid);
            if (t != null)
            {
                t.Name = newgroupname;
                client.UpdateEntity(t);

                var l = client.GetEntities<IntwentyUserGroup>();
                foreach (var g in l)
                {
                    if (g.GroupId == groupid)
                    {
                        g.GroupName = newgroupname;
                        client.UpdateEntity(g);
                    }
                }
            }
            else
            {
                result = IdentityResult.Failed();
            }
            client.Close();

            return Task.FromResult(result);
        }

        public Task<bool> GroupExists(string groupname)
        {
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Open();
            var t = client.GetEntities<IntwentyGroup>().Exists(p => p.Name.ToUpper() == groupname.ToUpper());
            client.Close();

            return Task.FromResult(t);
        }

        public Task<List<IntwentyUserGroup>> GetUserGroups(IntwentyUser user)
        {
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Open();
            var t = client.GetEntities<IntwentyUserGroup>().Where(p => p.UserId == user.Id).ToList();
            client.Close();

            return Task.FromResult(t);

        }


        public Task<List<IntwentyUserGroup>> GetGroupMembers(IntwentyGroup group)
        {
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Open();
            var t = client.GetEntities<IntwentyUserGroup>().Where(p => p.GroupId == group.Id);
            client.Close();

            return Task.FromResult(t.ToList());

        }

        public Task<bool> IsWaitingToJoinGroup(string username)
        {
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Open();
            var list = client.GetEntities<IntwentyUserGroup>();
            client.Close();

            var t = list.Exists(p => p.UserName.ToUpper() == username.ToUpper() && p.MembershipStatus == "WAITING");
            if (list.Exists(p => p.UserName.ToUpper() == username.ToUpper() && p.MembershipStatus == "ACCEPTED"))
                t = false;

          
            return Task.FromResult(t);

        }

        public Task<IdentityResult> RemoveFromGroupAsync(string userid, string groupid)
        {
            IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Open();
            var t = client.GetEntities<IntwentyUserGroup>().Find(p => p.UserId == userid && p.GroupId == groupid);
            if (t!=null)
                client.DeleteEntity(t);

            client.Close();

            return Task.FromResult(IdentityResult.Success);

        }


        public override Task<IdentityResult> DeleteAsync(IntwentyUser user)
        {
           
            var t = base.DeleteAsync(user);
            if (t.Result.Succeeded)
            {
                if (t != null && t.Result != null)
                {
                    IDataClient client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
                    client.Open();
                    var logins = client.GetEntities<IntwentyUserLogin>().Where(p => p.UserId == user.Id);
                    foreach (var l in logins)
                    {
                        client.DeleteEntity(l);
                    }
                    client.Close();
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
