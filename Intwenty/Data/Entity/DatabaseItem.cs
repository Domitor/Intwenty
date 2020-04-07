using System;
using Intwenty.Data.DBAccess.Annotations;


namespace Intwenty.Data.Entity
{
    [DbTableIndex("DBITEM_IDX_1", true, "AppMetaCode,MetaCode")]
    [DbTableIndex("DBITEM_IDX_2", true, "AppMetaCode,ParentMetaCode,MetaType,DbName")]
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_DatabaseItem")]
    public class DatabaseItem
    {
        public DatabaseItem()
        {
        }

        [AutoIncrement]
        public int Id { get; set; }

        public string MetaType { get; set; }

        public string Description { get; set; }

        public string AppMetaCode { get; set; }

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public string DbName { get; set; }

        public string DataType { get; set; }

        public string Domain { get; set; }

        public string Properties { get; set; }

        public bool Mandatory { get; set; }
    }

   

}
