using System;
using Intwenty.Data.DBAccess;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Entity;
using Intwenty.Data.Identity;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace Intwenty.Data.Seed
{
    public static class CreateIntwentyDb
    {
     

        public static void Run(IServiceProvider provider)
        {
            var Settings = provider.GetRequiredService<IOptions<IntwentySettings>>();

            if (!Settings.Value.IsDevelopment)
                return;

            IIntwentyDbORM DataRepository = null;
            if (Settings.Value.IsNoSQL)
                DataRepository = new IntwentyNoSqlDbClient(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);
            else
                DataRepository = new IntwentySqlDbClient(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);

         
            //CREATE INTWENTY MODEL TABLES
            DataRepository.CreateTable<ApplicationItem>(true);
            DataRepository.CreateTable<DatabaseItem>(true);
            DataRepository.CreateTable<DataViewItem>(true);
            DataRepository.CreateTable<EventLog>(true);
            DataRepository.CreateTable<InformationStatus>(true);
            DataRepository.CreateTable<MenuItem>(true);
            DataRepository.CreateTable<SystemID>(true);
            DataRepository.CreateTable<UserInterfaceItem>(true);
            DataRepository.CreateTable<ValueDomainItem>(true);
            DataRepository.CreateTable<DefaultValue>(true);

            //CREATE IDENTITY TABLES
            DataRepository.CreateTable<IntwentyUser>(true); //security_User
            DataRepository.CreateTable<IntwentyRole>(true); //security_Role
            DataRepository.CreateTable<IntwentyUserRole>(true); //security_UserRoles
            //DataRepository.CreateTable<IdentityUserClaim<string>>(true); //security_UserClaims
            //DataRepository.CreateTable<IdentityUserLogin<string>>(true); //security_UserLogins
            //DataRepository.CreateTable<IdentityRoleClaim<string>>(true); //security_RoleClaims
            //DataRepository.CreateTable<IdentityUserToken<string>>(true); //security_UserTokens



        }


    }
}
