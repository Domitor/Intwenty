﻿using Intwenty.DataClient.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Entity
{
    [DbTableIndex("SYSITEM_IDX_1", true, "MetaCode")]
    [DbTablePrimaryKey("Id")]
    [DbTableName("sysmodel_SystemItem")]
    public class SystemItem
    {
        public SystemItem()
        {

        }

        [AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }

        public string TitleLocalizationKey { get; set; }

        public string Description { get; set; }

        public string MetaCode { get; set; }

        public string DbPrefix { get; set; }

    }
}
