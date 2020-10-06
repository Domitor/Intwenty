using Intwenty.DataClient.Reflection;
using System;


namespace Intwenty.Data.Entity
{
    [DbTableIndex("sysid_idx_pid", false, "ParentId")]
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysdata_SystemId")]
    public class SystemID
    {
        [AutoIncrement]
        public int Id { get; set; }

        public int ParentId { get; set; }

        public int ApplicationId { get; set; }

        public string MetaType { get; set; }

        public string MetaCode { get; set; }

        public DateTime GeneratedDate { get; set; }

        public string Properties { get; set; }

    }

}
