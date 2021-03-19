using Intwenty.DataClient.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Intwenty.Areas.Identity.Entity
{

    [DbTableName("security_UserSetting")]
    [DbTablePrimaryKey("UserId")]
    public class IntwentyUserSetting
    {
        public string UserId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

    }
}
