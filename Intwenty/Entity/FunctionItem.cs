using Intwenty.DataClient.Reflection;
using System;

namespace Intwenty.Entity
{

    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_FunctionItem")]
   public class FunctionItem
    {
        public FunctionItem()
        {

        }

        [AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string SystemMetaCode { get; set; }
        [NotNull]
        public string AppMetaCode { get; set; }
        [NotNull]
        public string MetaCode { get; set; }
        [NotNull]
        public string MetaType { get; set; }
        [NotNull]
<<<<<<< HEAD
        public string ViewMetaCode{ get; set; }
=======
        public string OwnerMetaType{ get; set; }
        [NotNull]
        public string OwnerMetaCode { get; set; }
>>>>>>> master
        [NotNull]
        public string DataTableMetaCode { get; set; }
        [NotNull]
        public string Title { get; set; }
        public string TitleLocalizationKey { get; set; }
<<<<<<< HEAD
        public int RequiredAuthorization { get; set; }
        public string Path { get; set; }
=======
        public bool IsModalAction { get; set; }
        public string ActionPath { get; set; }
        public string ActionUserInterfaceMetaCode { get; set; }
>>>>>>> master
        public string Properties { get; set; }



    }

   

}
