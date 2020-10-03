using Intwenty.DataClient.Model;
using System.Collections.Generic;
using System.Data;

namespace Intwenty.DataClient
{
    public interface ISqlClient
    {
        void Open();
        void Close();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void CreateTable<T>();
        bool TableExists<T>();
        void RunCommand(string sql, bool isprocedure=false, IIntwentySqlParameter[] parameters=null);
        T GetEntity<T>(string id) where T : new();
        T GetEntity<T>(int id) where T : new();
        List<T> GetEntities<T>() where T : new();
        List<T> GetEntities<T>(string sql, bool isprocedure=false, IIntwentySqlParameter[] parameters=null) where T : new();
        int InsertEntity<T>(T entity);
        string GetJSONObject(string sql, bool isprocedure=false, IIntwentySqlParameter[] parameters=null, IIntwentyResultColumn[] resultcolumns=null);





    }

    public interface INoSqlClient
    {
        T GetOne<T>(int id) where T : new();
    }

    public interface IIntwentyResultColumn
    {
        public string Name { get; }
        public bool IsNumeric { get; }
        public bool IsDateTime { get; }
    }

    public interface IIntwentySqlParameter
    {
        public string Name { get;  }
        public object Value { get; }
        public DbType DataType { get; }
        public ParameterDirection Direction { get; }
    }
}
