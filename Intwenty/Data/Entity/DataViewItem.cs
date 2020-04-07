using Intwenty.Data.DBAccess.Annotations;



namespace Intwenty.Data.Entity
{
    [DbTableIndex("IDX_1", true, "MetaCode,ParentMetaCode")]
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_DataViewItem")]
    public class DataViewItem
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string MetaType { get; set; }

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public string Title { get; set; }

        public string SQLQuery { get; set; }

        public string SQLQueryFieldName { get; set; }

        public int Order { get; set; }

    }

}
