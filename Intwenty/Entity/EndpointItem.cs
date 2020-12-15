

using Intwenty.DataClient.Reflection;

namespace Intwenty.Entity
{
    [DbTableIndex("EP_IDX_1", true, "MetaCode,ParentMetaCode")]
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_EndpointItem")]
    public class EndpointItem
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string MetaType { get; set; }
        public string SystemMetaCode { get; set; }
        public string AppMetaCode { get; set; }
        public string MetaCode { get; set; }
        public string ParentMetaCode { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string DataMetaCode { get; set; }
        public int OrderNo { get; set; }
        public string Properties { get; set; }

    }

}
