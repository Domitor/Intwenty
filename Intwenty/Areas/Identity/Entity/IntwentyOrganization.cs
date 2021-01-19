using Intwenty.DataClient.Reflection;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_Organization")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyOrganization
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }


    }
}
