using Intwenty.DataClient.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Caching;
using System.Text;

namespace Intwenty.DataClient.Databases
{
    public static class TypeMap
    {
        private static string CACHETYPE = "DATATYPES";

        public static List<TypeMapItem> GetTypeMap()
        {
            List<TypeMapItem> res;
            var cache = MemoryCache.Default;
            res = cache.Get(CACHETYPE) as List<TypeMapItem>;
            if (res != null)
                return res;

            res = new List<TypeMapItem>();

            //STRING
            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "NVARCHAR(300)" });
            res.Add(new TypeMapItem() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "NVARCHAR(max)", Length = StringLength.Long });
            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "NVARCHAR(30)", Length = StringLength.Short });

            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "VARCHAR(300)" });
            res.Add(new TypeMapItem() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "LONGTEXT", Length = StringLength.Long });
            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "VARCHAR(30)", Length = StringLength.Short });

            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.MySql, DBMSDataType = "VARCHAR(300)" });
            res.Add(new TypeMapItem() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.MySql, DBMSDataType = "LONGTEXT", Length = StringLength.Long });
            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.MySql, DBMSDataType = "VARCHAR(30)", Length = StringLength.Short });

            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "VARCHAR(300)" });
            res.Add(new TypeMapItem() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "TEXT", Length = StringLength.Long });
            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "VARCHAR(30)", Length = StringLength.Short });

            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.SQLite, DBMSDataType = "TEXT" });
            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.SQLite, DBMSDataType = "TEXT", Length = StringLength.Short });
            res.Add(new TypeMapItem() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = SqlDBMS.SQLite, DBMSDataType = "TEXT", Length = StringLength.Long });

            //BOOLEAN
            res.Add(new TypeMapItem() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "INT" });
            res.Add(new TypeMapItem() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "TINYINT(1)" });
            res.Add(new TypeMapItem() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DbEngine = SqlDBMS.MySql, DBMSDataType = "TINYINT(1)" });
            res.Add(new TypeMapItem() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "BOOLEAN" });
            res.Add(new TypeMapItem() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DbEngine = SqlDBMS.SQLite, DBMSDataType = "INTEGER" });

            //INT
            res.Add(new TypeMapItem() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "INT" });
            res.Add(new TypeMapItem() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "INTEGER(11)" });
            res.Add(new TypeMapItem() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DbEngine = SqlDBMS.MySql, DBMSDataType = "INTEGER(11)" });
            res.Add(new TypeMapItem() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "INTEGER" });
            res.Add(new TypeMapItem() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DbEngine = SqlDBMS.SQLite, DBMSDataType = "INTEGER" });

            //DATETIME
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "DATETIME" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "DATETIME" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DbEngine = SqlDBMS.MySql, DBMSDataType = "DATETIME" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "TIMESTAMP" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DbEngine = SqlDBMS.SQLite, DBMSDataType = "DATETIME" });

            //DATETIMEOFSET
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIMEOFFSET", DataDbType = DbType.DateTime, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "DATETIME" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIMEOFFSET", DataDbType = DbType.DateTime, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "DATETIME" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIMEOFFSET", DataDbType = DbType.DateTime, DbEngine = SqlDBMS.MySql, DBMSDataType = "DATETIME" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIMEOFFSET", DataDbType = DbType.DateTime, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "TIMESTAMP" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIMEOFFSET", DataDbType = DbType.DateTime, DbEngine = SqlDBMS.SQLite, DBMSDataType = "DATETIME" });


            //DECIMAL
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MySql, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.SQLite, DBMSDataType = "REAL" });

            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MySql, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.SQLite, DBMSDataType = "REAL" });

            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "DECIMAL(18,1)DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MySql, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.SQLite, DBMSDataType = "REAL" });

            //SINGLE
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MySql, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.SQLite, DBMSDataType = "REAL" });

            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MySql, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.SQLite, DBMSDataType = "REAL" });

            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "DECIMAL(18,1)DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MySql, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.SQLite, DBMSDataType = "REAL" });


            //DOUBLE
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MySql, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.SQLite, DBMSDataType = "REAL" });

            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MySql, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.SQLite, DBMSDataType = "REAL" });

            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MariaDB, DBMSDataType = "DECIMAL(18,1)DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.MySql, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = SqlDBMS.SQLite, DBMSDataType = "REAL" });

            cache.Add(CACHETYPE, res, DateTime.Now.AddYears(1));

            return res;
        }
    }
}
