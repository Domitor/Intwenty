using Intwenty.DataClient.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text;

namespace Intwenty.DataClient.Databases
{
    public static class CommandMap
    {
        private static string CACHETYPE = "SQLCOMANDS";

        public static List<CommandMapItem> GetCommandMap()
        {
            List<CommandMapItem> result;

            var cache = MemoryCache.Default;
            result = cache.Get(CACHETYPE) as List<CommandMapItem>;
            if (result != null)
                return result;

            result = new List<CommandMapItem>();

            result.Add(new CommandMapItem() { Key = "AUTOINC", DbEngine = SqlDBMS.MSSqlServer, Command = "IDENTITY(1,1)" });
            result.Add(new CommandMapItem() { Key = "AUTOINC", DbEngine = SqlDBMS.MariaDB, Command = "AUTO_INCREMENT" });
            result.Add(new CommandMapItem() { Key = "AUTOINC", DbEngine = SqlDBMS.MySql, Command = "AUTO_INCREMENT" });
            result.Add(new CommandMapItem() { Key = "AUTOINC", DbEngine = SqlDBMS.PostgreSQL, Command = "SERIAL" });
            result.Add(new CommandMapItem() { Key = "AUTOINC", DbEngine = SqlDBMS.SQLite, Command = "PRIMARY KEY AUTOINCREMENT" });
            result.Add(new CommandMapItem() { Key = "GETDATE", DbEngine = SqlDBMS.MSSqlServer, Command = "GETDATE()" });
            result.Add(new CommandMapItem() { Key = "GETDATE", DbEngine = SqlDBMS.MariaDB, Command = "NOW()" });
            result.Add(new CommandMapItem() { Key = "GETDATE", DbEngine = SqlDBMS.MySql, Command = "NOW()" });
            result.Add(new CommandMapItem() { Key = "GETDATE", DbEngine = SqlDBMS.PostgreSQL, Command = "now()" });
            result.Add(new CommandMapItem() { Key = "GETDATE", DbEngine = SqlDBMS.SQLite, Command = "DATETIME('now', 'localtime')" });

            cache.Add(CACHETYPE, result, DateTime.Now.AddYears(1));

            return result;
        }
    }
}
