using System;
using System.Collections.Generic;
using System.Data;
using Intwenty.DataClient.Model;


namespace Intwenty.DataClient.Reflection
{
    public sealed class TypesCache
    {
        private TypesCache()
        {
            List = new List<IntwentyDataTable>();
        }

        private static readonly object padlock = new object();
        private static TypesCache instance = null;
        public static TypesCache Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new TypesCache();
                    }
                    return instance;
                }
            }
        }


        public List<IntwentyDataTable> List { get; private set; }

    }

    public static class TypeDataHandler
    {

       

        public static IntwentyDataTable GetTableInfoByTypeAndUsage<T>(string key)
        {
            var currenttype = typeof(T);
            var compositekey = currenttype.Name.ToUpper() + "_" + key.Replace(" ", "").ToUpper();
            return GetTableInfoByTypeAndUsageInternal<T>(compositekey, currenttype);
        }

        public static IntwentyDataTable GetTableInfoByTypeAndUsage<T>()
        {
            var currenttype = typeof(T);
            var key = currenttype.Name.ToUpper();
            return GetTableInfoByTypeAndUsageInternal<T>(key, currenttype);
        }

        public static void AdjustColumnsToUserQuery(IntwentyDataTable t, IDataReader reader)
        {
            if (reader == null)
                return;

            foreach (var c in t.Columns)
                c.IsInQueryResult = false;

            var schematable = reader.GetSchemaTable();
         
            for (int i = 0; i < schematable.Columns.Count; i++)
            {
                var resultcolname = schematable.Columns[i].ColumnName;
                var column = t.Columns.Find(p => p.Name.ToUpper() == resultcolname.ToUpper() && !p.IsIgnore);
                if (column != null)
                {
                    column.IsInQueryResult = true;
                    column.Order = i;
                }
               
            }

            t.Columns.RemoveAll(p => !p.IsInQueryResult);

        }

        private static IntwentyDataTable GetTableInfoByTypeAndUsageInternal<T>(string key, Type currenttype)
        {

            IntwentyDataTable result = TypesCache.Instance.List.Find(p => p.Id == key);
            if (result != null)
            {
                if (result.Name.ToUpper() == currenttype.Name.ToUpper())
                    return result;

                TypesCache.Instance.List.Remove(result);
            }

            result = new IntwentyDataTable() { Id = key, Name = currenttype.Name };

            var tablename = currenttype.GetCustomAttributes(typeof(DbTableName), false);
            if (tablename != null && tablename.Length > 0)
                result.Name = ((DbTableName)tablename[0]).Name;

            var primarykey = currenttype.GetCustomAttributes(typeof(DbTablePrimaryKey), false);
            if (primarykey != null && primarykey.Length > 0)
                result.PrimaryKeyColumnNames = ((DbTablePrimaryKey)primarykey[0]).Columns;

            var indexes = currenttype.GetCustomAttributes(typeof(DbTableIndex), false);
            if (indexes != null && indexes.Length > 0)
            {
                foreach (var a in indexes)
                {
                    var idx = (DbTableIndex)a;
                    var tblindex = new IntwentyDataTableIndex() { Id = idx.Name, Name = idx.Name, ColumnNames = idx.Columns, IsUnique = idx.IsUnique };
                    result.Indexes.Add(tblindex);
                }
            }

            var order = -1;
            var memberproperties = currenttype.GetProperties();
            foreach (var property in memberproperties)
            {
                var membername = property.Name;
                var columnname = property.GetCustomAttributes(typeof(DbColumnName), false);
                if (columnname != null && columnname.Length > 0)
                    membername = ((DbColumnName)columnname[0]).Name;

                var column = new IntwentyDataColumn() { Id=membername,  Name = membername, Property = property };

                var autoinc = property.GetCustomAttributes(typeof(AutoIncrement), false);
                if (autoinc != null && autoinc.Length > 0)
                    column.IsAutoIncremental = true;

                var notnull = property.GetCustomAttributes(typeof(NotNull), false);
                if (notnull != null && notnull.Length > 0)
                    column.IsNullNotAllowed = true;

                var ignore = property.GetCustomAttributes(typeof(Ignore), false);
                if (ignore != null && ignore.Length > 0)
                    column.IsIgnore = true;

                if (property.PropertyType.IsArray)
                    column.IsIgnore = true;

             

                if (result.PrimaryKeyColumnNames.Length == 0 && membername.ToUpper() == "ID")
                {
                    column.IsPrimaryKeyColumn = true;
                    result.PrimaryKeyColumnNames = membername;
                }
                else
                {
                    if (result.PrimaryKeyColumnNamesList.Exists(p => p.ToUpper() == membername.ToUpper()))
                        column.IsPrimaryKeyColumn = true;
                }

                if (!column.IsIgnore)
                    order += 1;

                column.Order = order;
                result.Columns.Add(column);
               

            }

            TypesCache.Instance.List.Add(result);

            return result;


        }



    }
}
