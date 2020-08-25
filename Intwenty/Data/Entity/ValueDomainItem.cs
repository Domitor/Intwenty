using Intwenty.Data.DBAccess.Annotations;

namespace Intwenty.Data.Entity
{

    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_ValueDomainItem")]
    public class ValueDomainItem
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string DomainName { get; set; }

        public string Code { get; set; }

        public string Value { get; set; }

        public string Properties { get; set; }

        public string ValueLocalizationKey { get; set; }

    }

}
