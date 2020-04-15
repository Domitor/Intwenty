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

        public DBMS DbEngine
        {
            get { return DBClient.DbEngine; }
        }

        public bool IsNoSql
        {
            get { return DBClient.IsNoSql; }
        }

        public T GetOne<T>(int id, int version) where T : new()
        {
            return DBClient.GetOne<T>(id, version);
        }

        public List<T> GetAll<T>() where T : new()
        {
            return DBClient.GetAll<T>();
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

        public int GetCollectionCount(string collectionname)
        {
            return DBClient.GetCollectionCount(collectionname);
        }

        public int GetMaxValue(string collectionname, string filter, string fieldname)
        {
            return DBClient.GetMaxValue(collectionname, filter, fieldname);
        }

        public StringBuilder GetAsJSONArray(string collectionname, string filter, string returnfields, int minrow = 0, int maxrow = 0)
        {
            return DBClient.GetAsJSONArray(collectionname, filter, returnfields, minrow, maxrow);
        }

        public StringBuilder GetAsJSONArray(string collectionname, int minrow = 0, int maxrow = 0)
        {
            return DBClient.GetAsJSONArray(collectionname, minrow, maxrow);
        }

        public StringBuilder GetAsJSONObject(string collectionname, int id, int version)
        {
            return DBClient.GetAsJSONObject(collectionname, id, version);
        }

        public StringBuilder GetAsJSONObject(string collectionname, string filter, string returnfields)
        {
            return DBClient.GetAsJSONObject(collectionname, filter, returnfields);
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
