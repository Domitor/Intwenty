using Intwenty.DataClient.Reflection;
using System;

namespace Intwenty.Entity
{


    /// <summary>
    /// Connects a view with one ore more userinterfaces
    /// </summary>
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_ViewUserInterfaceItem")]
   public class ViewUserInterfaceItem
    {
        public ViewUserInterfaceItem()
        {

        }

        [AutoIncrement]
        public int Id { get; set; }
        public string SystemMetaCode { get; set; }
        public string AppMetaCode { get; set; }
        public string ViewMetaCode { get; set; }
        public string UserInterfaceMetaCode { get; set; }
       
       

    }

   

}
