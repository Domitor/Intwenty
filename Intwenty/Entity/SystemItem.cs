using Intwenty.DataClient.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Entity
{
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_SystemItem")]
    public class SystemItem
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }

        public string TitleLocalizationKey { get; set; }

        public string Description { get; set; }

        public string MetaCode { get; set; }

        public string DbPrefix { get; set; }

        public bool RequiresAuthorization { get; set; }
    }
}
