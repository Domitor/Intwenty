using Intwenty.DataClient.Reflection;
using System;

namespace Intwenty.Entity
{


    /// <summary>
    /// Defines UI:s in a view
    /// </summary>
    [DbTableIndex("UI_IDX_1", true, "SystemMetaCode,AppMetaCode,ViewMetaCode,MetaCode")]
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_UserInterfaceItem")]
   public class UserInterfaceItem
    {
        public UserInterfaceItem()
        {

        }

        [AutoIncrement]
        public int Id { get; set; }
        public string SystemMetaCode { get; set; }
        public string AppMetaCode { get; set; }
        public string ViewMetaCode { get; set; }
        public string MetaCode { get; set; }


       
       

    }

   

}
