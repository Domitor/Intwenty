

using System.Collections.Generic;

namespace Intwenty.DataClient
{

    public enum NoSqlDBMS { MongoDb, LiteDb };

  
    public class NoSqlClient : INoSqlClient
    {

        public NoSqlDBMS DBMSType { get; }

        public string ConnectionString { get; }

        public string DatabaseName { get; }

        private INoSqlClient InternalClient { get; }

        public NoSqlClient(NoSqlDBMS database, string connectionstring, string databasename)
        {
            DBMSType = database;
            ConnectionString = connectionstring;
            DatabaseName = databasename;

            if (DBMSType == NoSqlDBMS.LiteDb)
                InternalClient = new Databases.NoSql.LiteDb(connectionstring, databasename);
            if (DBMSType == NoSqlDBMS.MongoDb)
                InternalClient = new Databases.NoSql.MongoDb(connectionstring, databasename);
        }

        public void CreateCollection<T>()
        {
            InternalClient.CreateCollection<T>();
        }

        public void DropCollection(string collectionname)
        {
            InternalClient.DropCollection(collectionname);
        }

        public T GetEntity<T>(int id) where T : new()
        {
            return InternalClient.GetEntity<T>(id);
        }

        public T GetEntity<T>(string id) where T : new()
        {
            return InternalClient.GetEntity<T>(id);
        }

        public List<T> GetEntities<T>(string expression = "") where T : new()
        {
            return InternalClient.GetEntities<T>(expression);
        }

        public int InsertEntity<T>(T model)
        {
            return InternalClient.InsertEntity<T>(model);
        }

        public int UpdateEntity<T>(T model)
        {
            return InternalClient.UpdateEntity<T>(model);
        }

        public int DeleteEntity<T>(T model)
        {
            return InternalClient.DeleteEntity<T>(model);
        }

        public int DeleteEntities<T>(IEnumerable<T> model)
        {
            return InternalClient.DeleteEntities<T>(model);
        }

        public bool DeleteJSONObject(string collectionname, int id, int version = 1)
        {
            return InternalClient.DeleteJSONObject(collectionname,id,version);
        }

        public int InsertJSONObject(string json, string collectionname, int id, int version = 1)
        {
            return InternalClient.InsertJSONObject(json, collectionname, id, version);
        }

        public int UpdateJSONObject(string json, string collectionname, int id, int version = 1)
        {
            return InternalClient.InsertJSONObject(json, collectionname, id, version);
        }

        public int Count(string collectionname)
        {
            return InternalClient.Count(collectionname);
        }

        public string GetJSONArray(string collectionname, string expression = "", int minrow = 0, int maxrow = 0, IIntwentyResultColumn[] resultcolumns = null)
        {
            return InternalClient.GetJSONArray(collectionname, expression, minrow, maxrow, resultcolumns);
        }

        public string GetJSONObject(string collectionname, int id, int version = 1, IIntwentyResultColumn[] resultcolumns = null)
        {
            return InternalClient.GetJSONObject(collectionname, id, version, resultcolumns);
        }

        public ResultSet GetResultSet(string collectionname, string expression = "", int minrow = 0, int maxrow = 0, IIntwentyResultColumn[] resultcolumns = null)
        {
            return InternalClient.GetResultSet(collectionname, expression, minrow, maxrow, resultcolumns);
        }
    }

}
