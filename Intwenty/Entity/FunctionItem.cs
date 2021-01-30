﻿using Intwenty.DataClient.Reflection;
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
        public string ViewMetaCode{ get; set; }
        public string DataTableMetaCode { get; set; }
        public string Title { get; set; }
        public string TitleLocalizationKey { get; set; }
        public int RequiredAuthorization { get; set; }
        public string Path { get; set; }
        public string Properties { get; set; }



    }

   

}