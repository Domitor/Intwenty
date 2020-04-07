using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intwenty.Data.DBAccess.Annotations
{
   

    [AttributeUsage(AttributeTargets.Class)]
    public class DbTableName : Attribute
    {
        public string Name { get; }

        public DbTableName(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DbTableIndex : Attribute
    {
        public string Name { get; }

        public bool IsUnique { get; }

        public string Columns { get; }

        public DbTableIndex(string name, bool isunique, string columns)
        {
            Name = name;
            IsUnique = isunique;
            Columns = columns;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class DbTablePrimaryKey : Attribute
    {
        public string Columns { get; }

        public DbTablePrimaryKey(string columns)
        {
            Columns = columns;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbColumnName : Attribute
    {
        public string Name { get; }

        public DbColumnName(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbColumnType : Attribute
    {
        public string DataType { get; }
        /// <summary>
        /// Datatype including length
        /// </summary>
        public DbColumnType(string datatype)
        {
            DataType = datatype;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIncrement : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NotNull : Attribute
    {

    }


}
