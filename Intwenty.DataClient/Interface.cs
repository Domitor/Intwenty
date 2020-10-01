using Intwenty.DataClient.Model;
using System.Collections.Generic;


namespace Intwenty.DataClient
{
    public interface ISqlClient
    {

        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        public void CreateTable<T>(bool checkExisting = false, bool useCurrentConnection = false);

        T GetOne<T>(int id) where T : new();

        T GetOne<T>(int id, bool useCurrentConnection) where T : new();

        List<T> GetMany<T>(string sqlcommand, bool isStoredProcedure) where T : new();

        List<T> GetMany<T>(string sqlcommand, bool isStoredProcedure, bool useCurrentConnection) where T : new();

        List<T> GetMany<T>(string sqlcommand, bool isStoredProcedure, bool useCurrentConnection, List<IntwentySqlParameter> parameters) where T : new();

        int Insert<T>(T model);

    }

    public interface INoSqlClient
    {
        T GetOne<T>(int id) where T : new();
    }
}
