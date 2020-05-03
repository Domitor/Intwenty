using System;
using Microsoft.Extensions.Options;
using System.Text;
using System.Collections.Generic;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Entity;
using Intwenty.Model;
using Intwenty.Data.Dto;

namespace Intwenty.Data.DBAccess
{



    public class IntwentyNoSqlDbService : IIntwentyDbNoSql
    {

        private IOptions<IntwentySettings> Settings { get; }

        private IntwentyNoSqlDbClient DBClient { get; }

        public IntwentyNoSqlDbService(IOptions<IntwentySettings> settings)
        {
            Settings = settings;
            DBClient = new IntwentyNoSqlDbClient(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);
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
        public bool DeleteIntwentyJsonObject(string collectionname, int id, int version)
        {
            return DBClient.DeleteIntwentyJsonObject(collectionname, id, version);
        }

        public int InsertIntwentyJsonObject(string json, string collectionname, int id, int version)
        {
            return DBClient.InsertIntwentyJsonObject(json, collectionname,id, version);
        }

        public int UpdateIntwentyJsonObject(string json, string collectionname, int id, int version)
        {
            return DBClient.UpdateIntwentyJsonObject(json, collectionname, id, version);
        }

        public int GetAutoIncrementalId(int applicationid = 0, string metatype = "INTERNAL", string metacode = "UNSPECIFIED", string properties = "")
        {
            return DBClient.GetAutoIncrementalId(applicationid,metatype,metacode,properties);
        }

        public int GetJsonDocumentCount(string collectionname)
        {
            return DBClient.GetJsonDocumentCount(collectionname);
        }

        /*
        public int GetMaxIntValue(string collectionname, string expression, string fieldname)
        {
            return DBClient.GetMaxIntValue(collectionname, expression, fieldname);
        }
        */

        public StringBuilder GetJsonArray(string collectionname, string filterexpression, List<IIntwentyDataColum> returnfields=null, int minrow = 0, int maxrow = 0)
        {
            return DBClient.GetJsonArray(collectionname, filterexpression, returnfields, minrow, maxrow);
        }

        public StringBuilder GetJsonArray(string collectionname, List<IIntwentyDataColum> returnfields = null, int minrow = 0, int maxrow = 0)
        {
            return DBClient.GetJsonArray(collectionname, returnfields, minrow, maxrow);
        }

        public StringBuilder GetIntwentyJsonObject(string collectionname, int id, int version, List<IIntwentyDataColum> returnfields = null)
        {
            return DBClient.GetIntwentyJsonObject(collectionname, id, version, returnfields);
        }

        public StringBuilder GetJsonObject(string collectionname, string filterexpression, List<IIntwentyDataColum> returnfields)
        {
            return DBClient.GetJsonObject(collectionname, filterexpression, returnfields);
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

        public void DropCollection(string collectionname)
        {
            DBClient.DropCollection(collectionname);
        }

        public bool DeleteJsonDocument(string collectionname, int id)
        {
            return DBClient.DeleteJsonDocument(collectionname, id);
        }

        public bool DeleteJsonDocument(string collectionname, string id)
        {
            return DBClient.DeleteJsonDocument(collectionname, id);
        }

        public bool UpdateJsonDocument(string collectionname, int id)
        {
            return DBClient.UpdateJsonDocument(collectionname, id);
        }

        public bool UpdateJsonDocument(string collectionname, string id)
        {
            return DBClient.UpdateJsonDocument(collectionname, id);
        }

        public bool InsertJsonDocument(string collectionname, int id)
        {
            return DBClient.InsertJsonDocument(collectionname, id);
        }

        public bool InsertJsonDocument(string collectionname, string id)
        {
            return DBClient.InsertJsonDocument(collectionname, id);
        }

        public StringBuilder GetJsonObject(string collectionname, string id)
        {
            return DBClient.GetJsonObject(collectionname, id);
        }

        public StringBuilder GetJsonObject(string collectionname, int id)
        {
            return DBClient.GetJsonObject(collectionname, id);
        }

        public ApplicationTable GetDataSet(string collectionname, string filterexpression = "")
        {
            return DBClient.GetDataSet(collectionname, filterexpression);
        }
    }

      


}
