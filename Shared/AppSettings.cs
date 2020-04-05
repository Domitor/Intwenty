using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared
{
    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }

    public class SystemSettings
    {
        public int DBMS { get; set; }

        public string SiteLanguage { get; set; }

        public bool AllowExternalLogins { get; set; }

        public bool AllowMFA { get; set; }

        public bool ForceMFA { get; set; }

        public bool EnableSMSMfa { get; set; }

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

    }
}
