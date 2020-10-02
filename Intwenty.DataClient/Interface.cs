using Intwenty.DataClient.Model;
using System.Collections.Generic;


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
        T GetEntity<T>(string id) where T : new();
        T GetEntity<T>(int id) where T : new();
        List<T> GetEntities<T>() where T : new();
        List<T> GetEntities<T>(string sqlcommand, bool isStoredProcedure) where T : new();
        List<T> GetEntities<T>(string sqlcommand, bool isStoredProcedure, List<IntwentySqlParameter> parameters) where T : new();
        int InsertEntity<T>(T entity);

       


    }

    public interface INoSqlClient
    {
        T GetOne<T>(int id) where T : new();
    }
}
