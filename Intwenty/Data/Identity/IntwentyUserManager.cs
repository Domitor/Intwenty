using Intwenty.Data.DBAccess;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Data.Identity
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

        public Task<IdentityResult> AddGroupMembershipAsync(IntwentyUser user, string groupname, string membershiptype, string membershipstatus)
        {
            IIntwentyDbORM client;
            if (Settings.IsNoSQL)
                client = new IntwentyNoSqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            else
                client = new IntwentySqlDbClient(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            var t = new IntwentyUserGroup();
            t.Id = Guid.NewGuid().ToString();
            t.UserId = user.Id;
            t.UserName = user.UserName;
            t.GroupName = groupname;
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



    }
}
