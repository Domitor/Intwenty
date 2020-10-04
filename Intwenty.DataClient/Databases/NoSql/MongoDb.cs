using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.Databases.NoSql
{
    public sealed class MongoDb : BaseNoSqlDb, INoSqlClient
    {
        public MongoDb(string connectionstring, string databasename) : base(connectionstring, databasename)
        {

        }

        public int Count(string collectionname)
        {
            throw new NotImplementedException();
        }

        public void CreateCollection<T>()
        {
            throw new NotImplementedException();
        }

        public int DeleteEntities<T>(IEnumerable<T> model)
        {
            throw new NotImplementedException();
        }

        public int DeleteEntity<T>(T model)
        {
            throw new NotImplementedException();
        }

        public bool DeleteJSONObject(string collectionname, int id, int version = 1)
        {
            throw new NotImplementedException();
        }

        public void DropCollection(string collectionname)
        {
            throw new NotImplementedException();
        }

        public List<T> GetEntities<T>(string expression = "") where T : new()
        {
            throw new NotImplementedException();
        }

        public T GetEntity<T>(int id) where T : new()
        {
            throw new NotImplementedException();
        }

        public T GetEntity<T>(string id) where T : new()
        {
            throw new NotImplementedException();
        }

        public string GetJSONArray(string collectionname, string expression = "", int minrow = 0, int maxrow = 0, IIntwentyResultColumn[] resultcolumns = null)
        {
            throw new NotImplementedException();
        }

        public string GetJSONObject(string collectionname, int id, int version = 1, IIntwentyResultColumn[] resultcolumns = null)
        {
            throw new NotImplementedException();
        }

        public ResultSet GetResultSet(string collectionname, string expression = "", int minrow = 0, int maxrow = 0, IIntwentyResultColumn[] resultcolumns = null)
        {
            throw new NotImplementedException();
        }

        public int InsertEntity<T>(T model)
        {
            throw new NotImplementedException();
        }

        public int InsertJSONObject(string json, string collectionname, int id, int version = 1)
        {
            throw new NotImplementedException();
        }

        public int UpdateEntity<T>(T model)
        {
            throw new NotImplementedException();
        }

        public int UpdateJSONObject(string json, string collectionname, int id, int version = 1)
        {
            throw new NotImplementedException();
        }
    }
}
