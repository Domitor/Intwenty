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
        public bool IsDevelopment { get; set; }
        public bool IsDemo { get; set; }
        public string DemoAdminUser { get; set; }
        public string DemoAdminPassword { get; set; }
        public string DemoUser { get; set; }
        public string DemoUserPassword { get; set; }
        public string DefaultCulture { get; set; }
        public string SiteName { get; set; }


        public bool AllowExternalLogins { get; set; }
        public bool ForceMFA { get; set; }
        public bool EnableEMailVerification { get; set; }

       

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

        public List<IntwentyLanguage> SupportedLanguages { get; set; }

    }

    public class IntwentyLanguage
    {
        public string Name { get; set; }

        public string Culture { get; set; }
    }
}
