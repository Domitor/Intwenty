using System;
using Microsoft.Extensions.Options;
using System.Text;
using System.Collections.Generic;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Model;
using Intwenty.Data.Dto;


namespace Intwenty.Data.DBAccess
{



    public class IntwentySqlDbService : IIntwentyDbSql
    {

        private IOptions<IntwentySettings> Settings { get; }

        private IntwentySqlDbClient DBClient { get; }

        public IntwentySqlDbService(IOptions<IntwentySettings> settings)
        {
            Settings = settings;
            DBClient = new IntwentySqlDbClient(Settings.Value.DefaultConnectionDBMS, Settings.Value.DefaultConnection);
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

        public void Open()
        {
            DBClient.Open();
        }

        public void Close()
        {
            DBClient.Close();
        }

        public void CreateCommand(string sqlstmt)
        {
            DBClient.CreateCommand(sqlstmt);
        }

        public void CreateSPCommand(string procedurename)
        {
            DBClient.CreateSPCommand(procedurename);
        }

        public void AddParameter(string name, object value)
        {
            DBClient.AddParameter(name, value);
        }

        public void AddParameter(IntwentyParameter p)
        {
            DBClient.AddParameter(p);
        }


        public object ExecuteScalarQuery()
        {
            return DBClient.ExecuteScalarQuery();
        }

        public NonQueryResult ExecuteNonQuery()
        {
            return DBClient.ExecuteNonQuery();
        }

        public StringBuilder GetJsonArray(List<IIntwentyDataColum> returnfields, int minrow = 0, int maxrow = 0)
        {
            return DBClient.GetJsonArray(returnfields, minrow, maxrow);
        }

        public StringBuilder GetJsonArray(int minrow = 0, int maxrow=0)
        {
            return DBClient.GetJsonArray(minrow, maxrow);
        }

        public StringBuilder GetJsonObject()
        {
            return DBClient.GetJsonObject();
        }

        public StringBuilder GetJsonObject(List<IIntwentyDataColum> returnfields)
        {
            return DBClient.GetJsonObject(returnfields);
        }


        public void CreateTable<T>(bool checkexisting = false)
        {
            DBClient.CreateTable<T>(checkexisting, false);
        }

        public void CreateTable<T>(bool checkexisting=false, bool use_current_connection = false)
        {
            DBClient.CreateTable<T>(checkexisting, use_current_connection);
        }

        public T GetOne<T>(int id, bool use_current_connection = false) where T : new()
        {
            return DBClient.GetOne<T>(id, use_current_connection);
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
            return DBClient.GetAll<T>(false);
        }
        public List<T> GetAll<T>(bool use_current_connection = false) where T : new()
        {
            return DBClient.GetAll<T>(use_current_connection);
        }

        public List<T> GetByExpression<T>(string expression, List<IntwentyParameter> parameters) where T : new()
        {
            throw new NotImplementedException();
        }


        public int Insert<T>(T model)
        {
            return DBClient.Insert(model, false);
        }

        public int Insert<T>(T model, bool use_current_connection = false)
        {
            return DBClient.Insert(model, use_current_connection);
        }

        public int Update<T>(T model)
        {
            return DBClient.Update(model, false);
        }

        public int Update<T>(T model, bool use_current_connection = false)
        {
            return DBClient.Update(model, use_current_connection);
        }

        public int Delete<T>(T model)
        {
            return DBClient.Delete(model, false);
        }

        public int Delete<T>(T model, bool use_current_connection = false)
        {
            return DBClient.Delete(model, use_current_connection);
        }

        public int DeleteRange<T>(IEnumerable<T> model)
        {
            return DBClient.DeleteRange(model, false);
        }

        public int DeleteRange<T>(IEnumerable<T> model, bool use_current_connection = false)
        {
            return DBClient.DeleteRange(model, use_current_connection);
        }

        public bool TableExist(string tablename)
        {
            return DBClient.TableExist(tablename);
        }

        public bool ColumnExist(string tablename, string columnname)
        {
            return DBClient.ColumnExist(tablename, columnname);
        }

        public ApplicationTable GetDataSet()
        {
            return DBClient.GetDataSet();
        }
    }

      


}
