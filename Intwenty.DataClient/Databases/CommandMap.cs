using Intwenty.DataClient.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;


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

            result.Add(new CommandMapItem() { Key = "AUTOINC", DbEngine = DBMS.MSSqlServer, Command = "IDENTITY(1,1)" });
            result.Add(new CommandMapItem() { Key = "AUTOINC", DbEngine = DBMS.MariaDB, Command = "AUTO_INCREMENT" });
            result.Add(new CommandMapItem() { Key = "AUTOINC", DbEngine = DBMS.MySql, Command = "AUTO_INCREMENT" });
            result.Add(new CommandMapItem() { Key = "AUTOINC", DbEngine = DBMS.PostgreSQL, Command = "SERIAL" });
            result.Add(new CommandMapItem() { Key = "AUTOINC", DbEngine = DBMS.SQLite, Command = "PRIMARY KEY AUTOINCREMENT" });
            result.Add(new CommandMapItem() { Key = "GETDATE", DbEngine = DBMS.MSSqlServer, Command = "GETDATE()" });
            result.Add(new CommandMapItem() { Key = "GETDATE", DbEngine = DBMS.MariaDB, Command = "NOW()" });
            result.Add(new CommandMapItem() { Key = "GETDATE", DbEngine = DBMS.MySql, Command = "NOW()" });
            result.Add(new CommandMapItem() { Key = "GETDATE", DbEngine = DBMS.PostgreSQL, Command = "now()" });
            result.Add(new CommandMapItem() { Key = "GETDATE", DbEngine = DBMS.SQLite, Command = "DATETIME('now', 'localtime')" });

            cache.Add(CACHETYPE, result, DateTime.Now.AddYears(1));

            return result;
        }
    }
}
