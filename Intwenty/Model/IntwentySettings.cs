using Intwenty.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intwenty.Model
{

    public enum AccountTypes { Local, Facebook, Google, BankId, FrejaEId };

    public enum MfaAuthTypes { Totp,Sms,Email,Fido2,SwedishBankId,FrejaEId };

    public enum LocalizationMethods { SiteLocalization, UserLocalization };

    public enum LogVerbosityTypes { Information, Warning, Error };


    public class IntwentySettings
    {
      

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

        #region Product
        public string ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductOrganization { get; set; }
        public string ProductSuperAdminEmail { get; set; }
        public string ProductSystemAdminEmail { get; set; }
        public string ProductUserAdminEmail { get; set; }
        #endregion

        #region StartUp

        /// <summary>
        /// Create intwenty database table on startup, if tables already exist they will not be created
        /// </summary>
        public bool StartUpIntwentyDbObjects { get; set; }

        /// <summary>
        /// If model is programaticly defined, create model container tables and seed the model
        /// </summary>
        public bool StartUpSeedModel { get; set; }
        /// <summary>
        /// Create database objects according to the model
        /// </summary>
        public bool StartUpConfigureDatabase { get; set; }
        /// <summary>
        /// Seed application data 
        /// </summary>
        public bool StartUpSeedData { get; set; }
        /// <summary>
        /// Seed localization definitions 
        /// </summary>
        public bool StartUpSeedLocalizations { get; set; }

        /// <summary>
        /// Seed product and organization info
        /// </summary>
        public bool StartUpSeedProductAndOrganization { get; set; }

        /// <summary>
        /// Seed demo user accounts
        /// </summary>
        public bool StartUpSeedDemoUserAccounts { get; set; }

        #endregion

        #region Demo
        /// <summary>
        /// If true this will show the username and password to use on the login page, DO NOT USE IN PRODUCTION
        /// </summary>
        public bool DemoShowLoginInfo { get; set; }
        public string DemoAdminUser { get; set; }
        public string DemoAdminPassword { get; set; }
        public string DemoUser { get; set; }
        public string DemoUserPassword { get; set; }
        #endregion

        #region Email
        //EMAIL
        public string MailServiceServer { get; set; }
        public int MailServicePort { get; set; }
        public string MailServiceUser { get; set; }
        public string MailServicePwd { get; set; }
        public string MailServiceAPIKey { get; set; }
        public string MailServiceFromEmail { get; set; }
        public string MailRedirectOutgoingTo { get; set; }
        public string SystemAdminEmail { get; set; }
        public string UserAdminEmail { get; set; }
        #endregion

        #region SMS
        public string SmsServiceAPIKey { get; set; }
        public string SmsServiceUser { get; set; }
        public string SmsServicePwd { get; set; }
        public string SmsServiceFrom { get; set; }
        public string SmsRedirectOutgoingTo { get; set; }
        #endregion

        #region Account
        public List<IntwentyAccount> AccountsAllowedList { get; set; }
        public bool AccountsRequireConfirmed { get; set; }
        public bool AccountsUseEmailAsUserName { get; set; }
        public bool AccountsAllowRegistration { get; set; }
        public bool AccountsRegistrationRequireName { get; set; }
        /// <summary>
        /// Comma separated  roles to assign new users
        /// </summary>
        public string AccountsRegistrationAssignRoles { get; set; }
        /// <summary>
        /// if true a new user can create a group account and invite others to be member users, or users can join a group
        /// Users can ask a group administrator to join a group
        /// A group admin can accept or reject user requests to join
        /// A group admin can invite users to the group
        /// </summary>
        public bool AccountsEnableUserGroups { get; set; }
        public string AccountsFacebookAppId { get; set; }
        public string AccountsFacebookAppSecret { get; set; }
        public string AccountsGoogleClientId { get; set; }
        public string AccountsGoogleClientSecret { get; set; }
        #endregion

        #region Localization
        /// <summary>
        /// SiteLocalization = Always use DefaultCulture to look up localization keys
        /// UserLocalization = Always use UserCulture to  look up localization keys
        /// </summary>
        public LocalizationMethods LocalizationMethod { get; set; }
        public string LocalizationDefaultCulture { get; set; }
        public List<IntwentyLanguage> LocalizationSupportedLanguages { get; set; }
        #endregion

        #region Two-Factor Authentication

        public bool TwoFactorEnable { get; set; }
        public string TwoFactorAppTitle { get; set; }
        public List<IntwentyMfaMethod> TwoFactorSupportedMethods { get; set; }
        public bool TwoFactorForced { get; set; }

        #endregion

        #region Storage

        //STORAGE
        public bool StorageUseFileSystem { get; set; }

        public bool StorageUseStorageAccount { get; set; }
      
        public string StorageContainer { get; set; }

        public string StorageConnectionString { get; set; }
        #endregion


        #region API
            public bool APIEnable { get; set; }
        #endregion






        /// <summary>
        /// Example: https://services.test.frejaeid.com
        /// </summary>
        public string FrejaBaseAddress { get; set; }

        /// <summary>
        /// Example: https://resources.test.frejaeid.com/qrcode/generate?qrcodedata={0}
        /// </summary>
        public string FrejaQRCodeEndpoint { get; set; }
        public string FrejaJWSCertificate { get; set; }

        /// <summary>
        /// The time to wait for the user to accept login in the app, 90000
        /// </summary>
        public int FrejaTimeoutInMilliseconds { get; set; }

        /// <summary>
        /// The thumbprint of the cerificate in the store
        /// </summary>
        public string FrejaClientCertThumbPrint { get; set; }

        /// <summary>
        /// BASIC, EXTENDED, PLUS, 
        /// </summary>
        public string FrejaMinRegistrationLevel { get; set; }

        /// <summary>
        /// A Comma separated list that should include one or more of:
        ///BASIC_USER_INFO,EMAIL_ADDRESS,ALL_EMAIL_ADDRESSES,ALL_PHONE_NUMBERS,DATE_OF_BIRTH,ADDRESSES,SSN,REGISTRATION_LEVEL,RELYING_PARTY_USER_ID,INTEGRATOR_SPECIFIC_USER_ID,CUSTOM_IDENTIFIER
        /// </summary>
        public string FrejaRequestedAttributes { get; set; }



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
                if (AccountsAllowedList == null)
                    return false;

            
                if (AccountsAllowedList.Exists(p => p.AccountType == AccountTypes.Facebook))
                    return true;

                if (AccountsAllowedList.Exists(p => p.AccountType == AccountTypes.Google))
                    return true;
                

                return false;
            }

        }

      

        public bool UseLocalLogins
        {

            get
            {
                if (AccountsAllowedList == null)
                    return false;

                if (AccountsAllowedList.Exists(p => p.AccountType == AccountTypes.Local))
                    return true;

                return false;
            }

        }

        public bool UseFacebookLogin
        {

            get
            {
                if (AccountsAllowedList == null)
                    return false;

                if (string.IsNullOrEmpty(AccountsFacebookAppId))
                    return false;

                if (string.IsNullOrEmpty(AccountsFacebookAppSecret))
                    return false;

                if (AccountsAllowedList.Exists(p => p.AccountType == AccountTypes.Facebook))
                    return true;
             

                return false;
            }

        }

        public bool UseGoogleLogin
        {

            get
            {
                if (AccountsAllowedList == null)
                    return false;

                if (string.IsNullOrEmpty(AccountsGoogleClientId))
                    return false;

                if (string.IsNullOrEmpty(AccountsGoogleClientSecret))
                    return false;

                if (AccountsAllowedList.Exists(p => p.AccountType == AccountTypes.Google))
                    return true;
               
                return false;
            }

        }

        public bool UseBankIdLogin
        {

            get
            {
                if (AccountsAllowedList == null)
                    return false;

                if (AccountsAllowedList.Exists(p => p.AccountType == AccountTypes.BankId))
                    return true;
              

                return false;
            }

        }

        public bool UseFrejaEIdLogin
        {

            get
            {
                if (AccountsAllowedList == null)
                    return false;

                if (AccountsAllowedList.Exists(p => p.AccountType == AccountTypes.FrejaEId))
                    return true;


                return false;
            }

        }

        public bool HasMfaMethods
        {

            get
            {
                if (TwoFactorSupportedMethods == null)
                    return false;

                if (TwoFactorSupportedMethods.Count > 0)
                    return true;

                return false;
            }

        }

        public bool UseSmsMfaAuth
        {

            get
            {
                if (TwoFactorSupportedMethods == null)
                    return false;

                if (TwoFactorSupportedMethods.Exists(p => p.MfaMethod == MfaAuthTypes.Sms))
                    return true;

                return false;
            }

        }

        public bool UseTotpMfaAuth
        {

            get
            {
                if (TwoFactorSupportedMethods == null)
                    return false;

                if (TwoFactorSupportedMethods.Exists(p => p.MfaMethod == MfaAuthTypes.Totp))
                    return true;

                return false;
            }

        }

        public bool UseEmailMfaAuth
        {

            get
            {
                if (TwoFactorSupportedMethods == null)
                    return false;

                if (TwoFactorSupportedMethods.Exists(p => p.MfaMethod == MfaAuthTypes.Email))
                    return true;

                return false;
            }

        }

        public bool UseFido2MfaAuth
        {

            get
            {
                if (TwoFactorSupportedMethods == null)
                    return false;

                if (TwoFactorSupportedMethods.Exists(p => p.MfaMethod == MfaAuthTypes.Fido2))
                    return true;

                return false;
            }

        }

        public bool UseSwedishBankIdMfaAuth
        {

            get
            {
                if (TwoFactorSupportedMethods == null)
                    return false;

                if (TwoFactorSupportedMethods.Exists(p => p.MfaMethod == MfaAuthTypes.SwedishBankId))
                    return true;

                return false;
            }

        }

        public bool UseFrejaEIdMfaAuth
        {

            get
            {
                if (TwoFactorSupportedMethods == null)
                    return false;

                if (TwoFactorSupportedMethods.Exists(p => p.MfaMethod == MfaAuthTypes.FrejaEId))
                    return true;

                return false;
            }

        }


    }

    public class IntwentyLanguage
    {
        public string Name { get; set; }

        public string Culture { get; set; }
    }

    public class IntwentyMfaMethod
    {
        public string Description { get; set; }

        public MfaAuthTypes MfaMethod { get; set; }
    }

    public class IntwentyAccount
    {
        public string Description { get; set; }

        public AccountTypes AccountType { get; set; }
    }

}
