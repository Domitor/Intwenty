using Intwenty.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intwenty.Model
{

    public enum AllowedAccountTypes { All, Social, Local, SocialFb, SocialGoogle };

    public enum LocalizationMethods { SiteLocalization, UserLocalization };

    public enum LogVerbosityTypes { Information, Warning, Error };


    public class IntwentySettings
    {
        public string ProductId { get; set; }

        public string DefaultProductOrganization { get; set; }

        public IntwentySettings()
        {
            LogVerbosity = LogVerbosityTypes.Error;
        }

        public LogVerbosityTypes LogVerbosity { get; set; }

        /// <summary>
        /// Database connections
        /// </summary>
        public string DefaultConnection { get; set; }
        public DBMS DefaultConnectionDBMS { get; set; }
        public string IAMConnection { get; set; }
        public DBMS IAMConnectionDBMS { get; set; }

        /// <summary>
        /// If model is programaticly defined, create model container tables and seed the model
        /// </summary>
        public bool SeedModelOnStartUp { get; set; }
        /// <summary>
        /// Create database objects according to the model
        /// </summary>
        public bool ConfigureDatabaseOnStartUp { get; set; }
        /// <summary>
        /// Seed user data 
        /// </summary>
        public bool SeedDataOnStartUp { get; set; }
        /// <summary>
        /// Seed localization definitions 
        /// </summary>
        public bool SeedLocalizationsOnStartUp { get; set; }


        //FOR DEBUG AND DEMO MODE
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

        /// <summary>
        /// Allow new visitors to register
        /// </summary>
        public bool UserRegistrationAllow { get; set; }
        public bool UserRegistrationRequireName { get; set; }

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


        //API
        public bool UseIntwentyAPI { get; set; }



        /// <summary>
        /// TEST DB CONNECTIONS
        /// </summary>
        public string TestDbConnectionSqlite { get; set; }
        public string TestDbConnectionMariaDb { get; set; }
        public string TestDbConnectionSqlServer { get; set; }
        public string TestDbConnectionPostgres { get; set; }

        public bool UseSeparateIAMDatabase
        {

            get
            {
                if (DefaultConnection.ToUpper() != IAMConnection.ToUpper())
                {
                    return true;
                }
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

        public bool UseLocalLogins
        {

            get
            {
                if (AllowedAccounts == AllowedAccountTypes.Local ||
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
