using Intwenty.Data.DBAccess.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Data.Identity
{
    [DbTableName("security_Group")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyGroup
    {
        public string Id { get; set; }

        public string Name { get; set; }


    }
}
