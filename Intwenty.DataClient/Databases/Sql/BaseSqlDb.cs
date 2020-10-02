using Intwenty.DataClient.Model;
using Intwenty.DataClient.Reflection;
using Intwenty.DataClient.SQLBuilder;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Intwenty.DataClient.Databases.Sql
{
    abstract class BaseSqlDb : BaseDb, IDisposable
    {

        public BaseSqlDb(string connectionstring) : base(connectionstring)
        {
        }


        public abstract void Dispose();
        public abstract void Open();
        public abstract void Close();
        protected abstract BaseSqlBuilder GetSqlBuilder();

        public void BeginTransaction()
        {
            IsInTransaction = true;
        }

        public void CommitTransaction()
        {
            var transaction = GetTransaction();
            if (IsInTransaction && transaction != null)
            {
                IsInTransaction = false;
                transaction.Commit();
                Close();
            }
           
        }

        public void RollbackTransaction() 
        {
            var transaction = GetTransaction();
            if (IsInTransaction && transaction != null)
            {
                IsInTransaction = false;
                transaction.Rollback();
                Close();
            }
        }

        public void CreateTable<T>()
        {
            var info = TypeDataHandler.GetTableInfoByTypeAndUsage<T>();

            if (TableExists<T>())
                return;

            var command = GetCommand();
            command.CommandText = GetSqlBuilder().GetCreateTableSql(info);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();

            foreach (var index in info.Indexes)
            {
                command = GetCommand();
                command.CommandText = GetSqlBuilder().GetCreateIndexSql(index);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

       

        public bool TableExists<T>()
        {
            var info = TypeDataHandler.GetTableInfoByTypeAndUsage<T>();

            try
            {
                var checkcommand = GetCommand();
                checkcommand.CommandText = string.Format("select 1 from {0}", info.Name);
                checkcommand.CommandType = CommandType.Text;
                checkcommand.ExecuteScalar();

                return true;
            }
            catch {  }

            return false;
     
        }

        public virtual T GetEntity<T>(int id) where T : new()
        {
            return GetEntity<T>(Convert.ToString(id));
        }

        public virtual T GetEntity<T>(string id) where T : new() 
        {
            var res = new T();
            var info = TypeDataHandler.GetTableInfoByTypeAndUsage<T>();

            if (info.PrimaryKeyColumnNamesList.Count == 0)
                throw new InvalidOperationException("No primary key column found");
            if (info.PrimaryKeyColumnNamesList.Count > 1)
                throw new InvalidOperationException(string.Format("The table {0} uses a composite primary key", info.Name));

            var command = GetCommand();
            command.CommandText = string.Format("SELECT * FROM {0} WHERE {1}={2}", new[] {info.Name,info.PrimaryKeyColumnNamesList[0], id });
            command.CommandType = CommandType.Text;

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                foreach (var col in info.Columns.Where(p => !p.IsIgnore).OrderBy(p => p.Order))
                {

                    if (reader.IsDBNull(col.Order))
                        continue;

                    SetPropertyValues(reader, col, res);

                }

            }

            return res;
        }

        public virtual List<T> GetEntities<T>() where T : new()
        {
            var res = new List<T>();
            var info = TypeDataHandler.GetTableInfoByTypeAndUsage<T>();

            var command = GetCommand();
            command.CommandText = string.Format("SELECT * FROM {0}", info.Name);
            command.CommandType = CommandType.Text;

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var m = new T();
                foreach (var col in info.Columns.Where(p => !p.IsIgnore).OrderBy(p => p.Order))
                {

                    if (reader.IsDBNull(col.Order))
                        continue;

                    SetPropertyValues(reader, col, m);

                }
                res.Add(m);
            }

            return res;
        }

        public virtual List<T> GetEntities<T>(string sqlcommand, bool isStoredProcedure) where T : new()
        {
            var res = new List<T>();
            var info = TypeDataHandler.GetTableInfoByTypeAndUsage<T>(sqlcommand);

            var command = GetCommand();
            command.CommandText = sqlcommand;
            if (isStoredProcedure)
                command.CommandType = CommandType.StoredProcedure;
            else
                command.CommandType = CommandType.Text;

            var reader = command.ExecuteReader();

            TypeDataHandler.AdjustColumnsToUserQuery(info, reader);

            while (reader.Read())
            {
                var m = new T();
                foreach (var col in info.Columns.Where(p=> !p.IsIgnore).OrderBy(p=> p.Order))
                {
                 
                    if (reader.IsDBNull(col.Order))
                        continue;

                    SetPropertyValues(reader, col, m);

                }
                res.Add(m);
            }

            return res;
        }

        public virtual List<T> GetEntities<T>(string sqlcommand, bool isStoredProcedure, List<IntwentySqlParameter> parameters) where T : new() { return default; }

        public virtual int InsertEntity<T>(T entity)
        {
            var info = TypeDataHandler.GetTableInfoByTypeAndUsage<T>();
            var parameters = new List<IntwentySqlParameter>();


            var command = GetCommand();
            command.CommandText = GetSqlBuilder().GetInsertSql(info, entity, parameters);
            command.CommandType = CommandType.Text;

            AddCommandParameters(parameters);

            var res = command.ExecuteNonQuery();

            HandleInsertAutoIncrementation(info, parameters, entity);

            return res;

        }

        protected abstract IDbCommand GetCommand();

        protected abstract IDbTransaction GetTransaction();

        protected virtual void SetPropertyValues<T>(IDataReader reader, IntwentyDataColumn column, T instance)
        {
            if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.INT32")
                column.Property.SetValue(instance, reader.GetInt32(column.Order), null);
            else if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.BOOLEAN")
                column.Property.SetValue(instance, reader.GetBoolean(column.Order), null);
            else if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.DECIMAL")
                column.Property.SetValue(instance, reader.GetDecimal(column.Order), null);
            else if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.SINGLE")
                column.Property.SetValue(instance, reader.GetFloat(column.Order), null);
            else if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.DOUBLE")
                column.Property.SetValue(instance, reader.GetDouble(column.Order), null);
            else
                column.Property.SetValue(instance, reader.GetValue(column.Order), null);
        }

        protected abstract void AddCommandParameters(List<IntwentySqlParameter> parameters);
            

        protected abstract void HandleInsertAutoIncrementation<T>(IntwentyDataTable info, List<IntwentySqlParameter> parameters, T entity);

      
       
    }
}
