using Intwenty.DataClient.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_ProductGroup")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyProductGroup
    {
        public string Id { get; set; }

        public string ProductId { get; set; }

        public string Name { get; set; }


    }
}
