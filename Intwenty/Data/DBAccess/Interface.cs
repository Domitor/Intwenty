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
        public string ConnectionString { get; }

        public DBMS DbEngine { get; }

        public bool IsNoSql { get; }

    }

    public interface IIntwentyDbORM
    {
        void CreateTable<T>(bool checkexisting = false);
        T GetOne<T>(int id) where T : new();
        T GetOne<T>(string id) where T : new();
        List<T> GetAll<T>() where T : new();
        int Insert<T>(T model);
        int Update<T>(T model);
        int Delete<T>(T model);
        int DeleteRange<T>(IEnumerable<T> model);

    }

    public interface IIntwentyDbNoSql : IIntwentyDb, IIntwentyDbORM
    {
        public bool DeleteJsonDocumentById(string collectionname, int id, int version);

        public int InsertJsonDocument(string json, string collectionname, int id, int version);

        public int UpdateJsonDocument(string json, string collectionname, int id, int version);

        public int GetNewSystemId(SystemID model);

        /// <summary>
        /// Counts the documents in the collection
        /// </summary>
        public int GetDocumentCount(string collectionname);

        /// <summary>
        /// Returns the maxvalue for the integer field [fieldname].
        /// </summary>
        public int GetMaxIntValue(string collectionname, string expression, string fieldname);

        StringBuilder GetJSONArray(string collectionname, int minrow = 0, int maxrow = 0);

        StringBuilder GetJSONArray(string collectionname, string expression, List<IIntwentyDataColum> returnfields, int minrow = 0, int maxrow = 0);

        StringBuilder GetJSONObject(string collectionname, int id, int version);

        StringBuilder GetJSONObject(string collectionname, string expression, List<IIntwentyDataColum> returnfields);

     
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
        StringBuilder GetJSONArray(List<IIntwentyDataColum> returnfields, int minrow = 0, int maxrow = 0);
        StringBuilder GetJSONArray(int minrow = 0, int maxrow = 0);
        StringBuilder GetJSONObject();
        StringBuilder GetJSONObject(List<IIntwentyDataColum> returnfields);
        NonQueryResult ExecuteNonQuery();
        void CreateTable<T>(bool checkexisting = false, bool use_current_connection = false);
        T GetOne<T>(int id, bool use_current_connection = false) where T : new();
        List<T> GetAll<T>(bool use_current_connection = false) where T : new();
        int Insert<T>(T model, bool use_current_connection = false);
        int Update<T>(T model, bool use_current_connection = false);
        int Delete<T>(T model, bool use_current_connection = false);
        int DeleteRange<T>(IEnumerable<T> model, bool use_current_connection = false);
    }



}
