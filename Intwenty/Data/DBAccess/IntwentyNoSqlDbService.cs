using System;
using Microsoft.Extensions.Options;
using System.Text;
using Shared;
using System.Collections.Generic;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Entity;

namespace Intwenty.Data.DBAccess
{



    public class IntwentyNoSqlDbService : IIntwentyDbNoSql
    {

        private IOptions<SystemSettings> Settings { get; }

        private IOptions<ConnectionStrings> Connections { get; }

        private IntwentyNoSqlDbClient DBClient { get; }

        public IntwentyNoSqlDbService(IOptions<SystemSettings> settings, IOptions<ConnectionStrings> connections)
        {
            Settings = settings;
            Connections = connections;
            DBClient = new IntwentyNoSqlDbClient((DBMS)Settings.Value.IntwentyDBMS, Connections.Value.IntwentyConnection, "IntwentyDb");
        }

        public string ConnectionString
        {
            get { return DBClient.ConnectionString; }
        }

        public DBMS DbEngine
        {
            get { return DBClient.DbEngine; }
        }

        public bool IsNoSql
        {
            get { return DBClient.IsNoSql; }
        }

        public T GetOne<T>(int id) where T : new()
        {
            return DBClient.GetOne<T>(id);
        }
        public T GetOne<T>(string id) where T : new()
        {
            return DBClient.GetOne<T>(id);
        }
        public List<T> GetAll<T>() where T : new()
        {
            return DBClient.GetAll<T>();
        }
        public bool DeleteJsonDocumentById(string collectionname, int id, int version)
        {
            return DBClient.DeleteJsonDocumentById(collectionname, id, version);
        }

        public int InsertJsonDocument(string json, string collectionname, int id, int version)
        {
            return DBClient.InsertJsonDocument(json, collectionname,id, version);
        }

        public int UpdateJsonDocument(string json, string collectionname, int id, int version)
        {
            return DBClient.UpdateJsonDocument(json, collectionname, id, version);
        }

        public int GetNewSystemId(SystemID model)
        {
            return DBClient.GetNewSystemId(model);
        }

        public int GetDocumentCount(string collectionname)
        {
            return DBClient.GetDocumentCount(collectionname);
        }

        public int GetMaxIntValue(string collectionname, string expression, string fieldname)
        {
            return DBClient.GetMaxIntValue(collectionname, expression, fieldname);
        }

        public StringBuilder GetJSONArray(string collectionname, string expression, List<IIntwentyDataColum> returnfields, int minrow = 0, int maxrow = 0)
        {
            return DBClient.GetJSONArray(collectionname, expression, returnfields, minrow, maxrow);
        }

        public StringBuilder GetJSONArray(string collectionname, int minrow = 0, int maxrow = 0)
        {
            return DBClient.GetJSONArray(collectionname, minrow, maxrow);
        }

        public StringBuilder GetJSONObject(string collectionname, int id, int version)
        {
            return DBClient.GetJSONObject(collectionname, id, version);
        }

        public StringBuilder GetJSONObject(string collectionname, string expression, List<IIntwentyDataColum> returnfields)
        {
            return DBClient.GetJSONObject(collectionname, expression, returnfields);
        }

        public void CreateTable<T>(bool checkexisting = false)
        {
            DBClient.CreateTable<T>(checkexisting);
        }

       

        public int Insert<T>(T model)
        {
            return DBClient.Insert<T>(model);
        }

        public int Update<T>(T model)
        {
            return DBClient.Update<T>(model);
        }

        public int Delete<T>(T model)
        {
            return DBClient.Delete<T>(model);
        }

        public int DeleteRange<T>(IEnumerable<T> model)
        {
            return DBClient.DeleteRange<T>(model);
        }

      
    }

      


}
