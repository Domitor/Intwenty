using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intwenty.Model
{

    public enum DBMS { MSSqlServer, MySql, MariaDB, PostgreSQL, SQLite, MongoDb, LiteDb };

 
    public class IntwentySettings
    {
        public string DefaultConnection { get; set; }
        public DBMS DefaultConnectionDBMS { get; set; }
        public bool ReCreateDatabaseOnStartup { get; set; }

        public bool SeedDatabaseOnStartUp { get; set; }

        public bool UseDemoSettings { get; set; }

        public string DemoAdminUser { get; set; }
        public string DemoAdminPassword { get; set; }
        public string DemoUser { get; set; }
        public string DemoUserPassword { get; set; }

        public bool EnableLocalization { get; set; }
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


        public bool EnableExternalLogins { get; set; }
        public bool EnableEMailVerivication { get; set; }
        public bool EnableTwoFactorAuthentication { get; set; }
        public bool ForceTwoFactorAuthentication { get; set; }


        public bool EnableAPIKeyGeneration { get; set; }


        //EMAIL
        public string MailServiceServer { get; set; }

        public int MailServicePort { get; set; }

        public string MailServiceUser { get; set; }

        public string MailServicePwd { get; set; }

        public string MailServiceAPIKey { get; set; }

        public string MailServiceFromEmail { get; set; }

        //FOR DEBUG MODE
        public string RedirectAllOutgoingMailTo { get; set; }

        //FOR DEBUG MODE
        public string RedirectAllOutgoingSMSTo { get; set; }

        public bool StorageUseFileSystem { get; set; }

        public bool StorageUseStorageAccount { get; set; }

        //AZURE STORAGE
        public string StorageContainer { get; set; }

        public string StorageConnectionString { get; set; }

        public bool IsNoSQL
        {

            get
            {
                if (DefaultConnectionDBMS == DBMS.MongoDb  || DefaultConnectionDBMS == DBMS.LiteDb)
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
}
