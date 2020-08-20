using System;
using Intwenty.Data.DBAccess.Annotations;

namespace Intwenty.Data.Entity
{

    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_TranslationItem")]
   public class TranslationItem
    {
        public TranslationItem()
        {

        }

        public int Id { get; set; }

        public string Key { get; set; }

        public string Culture { get; set; }

        public string Text { get; set; }

     
    }

   

}
