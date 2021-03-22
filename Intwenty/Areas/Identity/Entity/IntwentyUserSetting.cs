using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableIndex("USRSETTING_IDX_1", true, "UserId,Key")]
    [DbTableName("security_UserSetting")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyUserSetting
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

    }
}
