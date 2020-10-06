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
            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.MSSqlServer, DBMSDataType = "NVARCHAR(300)" });
            res.Add(new TypeMapItem() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.MSSqlServer, DBMSDataType = "NVARCHAR(max)", Length = StringLength.Long });
            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.MSSqlServer, DBMSDataType = "NVARCHAR(30)", Length = StringLength.Short });

            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.MariaDB, DBMSDataType = "VARCHAR(300)" });
            res.Add(new TypeMapItem() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.MariaDB, DBMSDataType = "LONGTEXT", Length = StringLength.Long });
            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.MariaDB, DBMSDataType = "VARCHAR(30)", Length = StringLength.Short });

            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.MySql, DBMSDataType = "VARCHAR(300)" });
            res.Add(new TypeMapItem() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.MySql, DBMSDataType = "LONGTEXT", Length = StringLength.Long });
            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.MySql, DBMSDataType = "VARCHAR(30)", Length = StringLength.Short });

            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.PostgreSQL, DBMSDataType = "VARCHAR(300)" });
            res.Add(new TypeMapItem() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.PostgreSQL, DBMSDataType = "TEXT", Length = StringLength.Long });
            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.PostgreSQL, DBMSDataType = "VARCHAR(30)", Length = StringLength.Short });

            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.SQLite, DBMSDataType = "TEXT" });
            res.Add(new TypeMapItem() { IntwentyType = "STRING", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.SQLite, DBMSDataType = "TEXT", Length = StringLength.Short });
            res.Add(new TypeMapItem() { IntwentyType = "TEXT", NetType = "SYSTEM.STRING", DataDbType = DbType.String, DbEngine = DBMS.SQLite, DBMSDataType = "TEXT", Length = StringLength.Long });

            //BOOLEAN
            res.Add(new TypeMapItem() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DbEngine = DBMS.MSSqlServer, DBMSDataType = "INT" });
            res.Add(new TypeMapItem() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DbEngine = DBMS.MariaDB, DBMSDataType = "TINYINT(1)" });
            res.Add(new TypeMapItem() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DbEngine = DBMS.MySql, DBMSDataType = "TINYINT(1)" });
            res.Add(new TypeMapItem() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DbEngine = DBMS.PostgreSQL, DBMSDataType = "BOOLEAN" });
            res.Add(new TypeMapItem() { IntwentyType = "BOOLEAN", NetType = "SYSTEM.BOOLEAN", DataDbType = DbType.Boolean, DbEngine = DBMS.SQLite, DBMSDataType = "INTEGER" });

            //INT
            res.Add(new TypeMapItem() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DbEngine = DBMS.MSSqlServer, DBMSDataType = "INT" });
            res.Add(new TypeMapItem() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DbEngine = DBMS.MariaDB, DBMSDataType = "INTEGER(11)" });
            res.Add(new TypeMapItem() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DbEngine = DBMS.MySql, DBMSDataType = "INTEGER(11)" });
            res.Add(new TypeMapItem() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DbEngine = DBMS.PostgreSQL, DBMSDataType = "INTEGER" });
            res.Add(new TypeMapItem() { IntwentyType = "INTEGER", NetType = "SYSTEM.INT32", DataDbType = DbType.Int32, DbEngine = DBMS.SQLite, DBMSDataType = "INTEGER" });

            //DATETIME
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DbEngine = DBMS.MSSqlServer, DBMSDataType = "DATETIME" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DbEngine = DBMS.MariaDB, DBMSDataType = "DATETIME" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DbEngine = DBMS.MySql, DBMSDataType = "DATETIME" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DbEngine = DBMS.PostgreSQL, DBMSDataType = "TIMESTAMP" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIME", DataDbType = DbType.DateTime, DbEngine = DBMS.SQLite, DBMSDataType = "DATETIME" });

            //DATETIMEOFSET
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIMEOFFSET", DataDbType = DbType.DateTime, DbEngine = DBMS.MSSqlServer, DBMSDataType = "DATETIME" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIMEOFFSET", DataDbType = DbType.DateTime, DbEngine = DBMS.MariaDB, DBMSDataType = "DATETIME" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIMEOFFSET", DataDbType = DbType.DateTime, DbEngine = DBMS.MySql, DBMSDataType = "DATETIME" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIMEOFFSET", DataDbType = DbType.DateTime, DbEngine = DBMS.PostgreSQL, DBMSDataType = "TIMESTAMP" });
            res.Add(new TypeMapItem() { IntwentyType = "DATETIME", NetType = "SYSTEM.DATETIMEOFFSET", DataDbType = DbType.DateTime, DbEngine = DBMS.SQLite, DBMSDataType = "DATETIME" });


            //DECIMAL
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.MariaDB, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.MySql, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.SQLite, DBMSDataType = "REAL" });

            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.MariaDB, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.MySql, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.SQLite, DBMSDataType = "REAL" });

            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.MariaDB, DBMSDataType = "DECIMAL(18,1)DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.MySql, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DECIMAL", DataDbType = DbType.Decimal, DbEngine = DBMS.SQLite, DBMSDataType = "REAL" });

            //SINGLE
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MariaDB, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MySql, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.SQLite, DBMSDataType = "REAL" });

            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MariaDB, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MySql, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.SQLite, DBMSDataType = "REAL" });

            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MariaDB, DBMSDataType = "DECIMAL(18,1)DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MySql, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.SINGLE", DataDbType = DbType.Decimal, DbEngine = DBMS.SQLite, DBMSDataType = "REAL" });


            //DOUBLE
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MariaDB, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MySql, DBMSDataType = "DECIMAL(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,3)" });
            res.Add(new TypeMapItem() { IntwentyType = "3DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.SQLite, DBMSDataType = "REAL" });

            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MariaDB, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MySql, DBMSDataType = "DECIMAL(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,2)" });
            res.Add(new TypeMapItem() { IntwentyType = "2DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.SQLite, DBMSDataType = "REAL" });

            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MSSqlServer, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MariaDB, DBMSDataType = "DECIMAL(18,1)DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.MySql, DBMSDataType = "DECIMAL(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.PostgreSQL, DBMSDataType = "NUMERIC(18,1)" });
            res.Add(new TypeMapItem() { IntwentyType = "1DECIMAL", NetType = "SYSTEM.DOUBLE", DataDbType = DbType.Decimal, DbEngine = DBMS.SQLite, DBMSDataType = "REAL" });

            cache.Add(CACHETYPE, res, DateTime.Now.AddYears(1));

            return res;
        }
    }
}
