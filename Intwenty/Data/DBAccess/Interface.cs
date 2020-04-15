using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Intwenty.Data.DBAccess
{

    public interface IIntwentyDb
    {
        public DBMS DbEngine { get; }

        public bool IsNoSql { get; }

    }

    public interface IIntwentyDbORM
    {
        
        void CreateTable<T>(bool checkexisting = false);
        T GetOne<T>(int id, int version) where T : new();
        List<T> GetAll<T>() where T : new();
        int Insert<T>(T model);
        int Update<T>(T model);
        int Delete<T>(T model);
        int DeleteRange<T>(IEnumerable<T> model);

    }

    public interface IIntwentyDbNoSql : IIntwentyDb, IIntwentyDbORM
    {
       
        public int InsertJsonDocument(string json, string collectionname, int id, int version);

        public int UpdateJsonDocument(string json, string collectionname, int id, int version);

        public int GetNewSystemId(SystemID model);

        public int GetCollectionCount(string collectionname);

        StringBuilder GetAsJSONArray(string collectionname, int minrow = 0, int maxrow = 0);

        StringBuilder GetAsJSONObject(string collectionname, int id, int version);

    }


    public interface IIntwentyDbSql : IIntwentyDb, IIntwentyDbORM
    {
        void Open();
        void Close();
        void CreateCommand(string sql);
        void CreateSPCommand(string procedurename);
        void AddParameter(string name, object value);
        void AddParameter(IntwentySqlParameter p);
        void FillDataset(DataSet ds, string tablename);
        object ExecuteScalarQuery();
        bool TableExist(string tablename);
        bool ColumnExist(string tablename, string columnname);
        StringBuilder GetAsJSONArray(List<IIntwentyDataColum> columns, int minrow = 0, int maxrow = 0);
        StringBuilder GetAsJSONArray(int minrow = 0, int maxrow = 0);
        StringBuilder GetAsJSONObject();
        StringBuilder GetAsJSONObject(List<IIntwentyDataColum> columns);
        NonQueryResult ExecuteNonQuery();
        void CreateTable<T>(bool checkexisting = false, bool use_current_connection = false);
        T GetOne<T>(int id, int version, bool use_current_connection = false) where T : new();
        List<T> GetAll<T>(bool use_current_connection = false) where T : new();
        int Insert<T>(T model, bool use_current_connection = false);
        int Update<T>(T model, bool use_current_connection = false);
        int Delete<T>(T model, bool use_current_connection = false);
        int DeleteRange<T>(IEnumerable<T> model, bool use_current_connection = false);
    }



}
