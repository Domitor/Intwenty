﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intwenty.Model
{

    public enum DBMS { MSSqlServer, MySql, MariaDB, PostgreSQL, SQLite, MongoDb, LiteDb };

    public enum AllowedAccountTypes { All, Social, Manual, SocialFb, SocialGoogle };

    public enum LocalizationMethods { SiteLocalization, UserLocalization };


    public class IntwentySettings
    {

        //FOR DEBUG AND DEMO MODE
        public bool ReCreateDatabaseOnStartup { get; set; }
        public bool SeedDatabaseOnStartUp { get; set; }
        public bool UseDemoSettings { get; set; }
        public string DemoAdminUser { get; set; }
        public string DemoAdminPassword { get; set; }
        public string DemoUser { get; set; }
        public string DemoUserPassword { get; set; }
        public string RedirectAllOutgoingMailTo { get; set; }
        public string RedirectAllOutgoingSMSTo { get; set; }

        /// <summary>
        /// What kind of logins should be allowed
        /// </summary>
        public AllowedAccountTypes AllowedAccounts { get; set; }

        /// <summary>
        /// Database connection
        /// </summary>
        public string DefaultConnection { get; set; }
        public DBMS DefaultConnectionDBMS { get; set; }


        /// <summary>
        /// SiteLocalization = Always use DefaultCulture to look up localization keys
        /// UserLocalization = Always use UserCulture to  look up localization keys
        /// </summary>
        public LocalizationMethods LocalizationMethod { get; set; }
        public string DefaultCulture { get; set; }
        public List<IntwentyLanguage> SupportedLanguages { get; set; }


        /// <summary>
        /// The title of the site where intwenty is used
        /// </summary>
        public string SiteTitle { get; set; }

        /// <summary>
        /// The title to show in authenticator apps
        /// </summary>
        public string AuthenticatorTitle { get; set; }

        public bool EnableEMailVerification { get; set; }

        public bool EnableTwoFactorAuthentication { get; set; }
        public bool ForceTwoFactorAuthentication { get; set; }


        public bool EnableAPIKeyGeneration { get; set; }

        /// <summary>
        /// Comma separated  roles to assign new users
        /// </summary>
        public string NewUserRoles  { get; set; }

        /// <summary>
        /// if true a new user can create a group account and invite others to be member users, or users can join a group
        /// Users can ask a group administrator to join a group
        /// A group admin can accept or reject user requests to join
        /// A group admin can invite users to the group
        /// </summary>
        public bool EnableUserGroups { get; set; }
     

        //EMAIL
        public string MailServiceServer { get; set; }
        public int MailServicePort { get; set; }
        public string MailServiceUser { get; set; }
        public string MailServicePwd { get; set; }
        public string MailServiceAPIKey { get; set; }
        public string MailServiceFromEmail { get; set; }
        public string SystemAdminEmail { get; set; }
        public string UserAdminEmail { get; set; }


        //STORAGE
        public bool StorageUseFileSystem { get; set; }

        public bool StorageUseStorageAccount { get; set; }
      
        public string StorageContainer { get; set; }

        public string StorageConnectionString { get; set; }


        //SOCIAL LOGINS

        public string FacebookAppId { get; set; }

        public string FacebookAppSecret { get; set; }

        public string GoogleClientId { get; set; }

        public string GoogleClientSecret { get; set; }

        public bool IsNoSQL
        {

            get
            {
                if (DefaultConnectionDBMS == DBMS.MongoDb  || DefaultConnectionDBMS == DBMS.LiteDb)
                    return true;

                return false;
            }

        }

        public bool UseExternalLogins
        {

            get
            {
                if (AllowedAccounts == AllowedAccountTypes.SocialGoogle ||
                    AllowedAccounts == AllowedAccountTypes.SocialFb ||
                    AllowedAccounts == AllowedAccountTypes.Social ||
                    AllowedAccounts == AllowedAccountTypes.All)
                {
                    return true;
                }
                return false;
            }

        }

        public bool UseManualLogins
        {

            get
            {
                if (AllowedAccounts == AllowedAccountTypes.Manual ||
                    AllowedAccounts == AllowedAccountTypes.All)
                {
                    return true;
                }
                return false;
            }

        }

        public bool UseFacebookLogin
        {

            get
            {
                if (AllowedAccounts == AllowedAccountTypes.SocialFb ||
                    AllowedAccounts == AllowedAccountTypes.Social ||
                    AllowedAccounts == AllowedAccountTypes.All)
                {
                    return true;
                }
                return false;
            }

        }

        public bool UseGoogleLogin
        {

            get
            {
                if (AllowedAccounts == AllowedAccountTypes.SocialGoogle ||
                    AllowedAccounts == AllowedAccountTypes.Social ||
                    AllowedAccounts == AllowedAccountTypes.All)
                {
                    return true;
                }
                return false;
            }

        }



    }

    public class IntwentyLanguage
    {
        public string Name { get; set; }

        public string Culture { get; set; }
    }
}
