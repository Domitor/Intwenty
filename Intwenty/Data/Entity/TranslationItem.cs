using Intwenty.DataClient.Reflection;
using System;

namespace Intwenty.Data.Entity
{
    [DbTableIndex("TRANSITEM_IDX_1", true, "Key,Culture")]
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_TranslationItem")]
   public class TranslationItem
    {
        public TranslationItem()
        {

        }

        [AutoIncrement]
        public int Id { get; set; }

        public string Key { get; set; }

        public string Culture { get; set; }

        public string Text { get; set; }

     
    }

   

}
