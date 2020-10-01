using Intwenty.DataClient.Model;
using Intwenty.DataClient.Reflection;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Intwenty.DataClient.Databases
{
    public abstract class BaseSqlDb : BaseDb, IDisposable
    {

        public BaseSqlDb(string connectionstring) : base(connectionstring)
        {
        }


        public abstract void Dispose();

        public virtual void BeginTransaction()
        {
            IsInTransaction = true;
        }

        public virtual void CommitTransaction()
        {
            var transaction = GetTransaction();
            if (IsInTransaction && transaction != null)
            {
                IsInTransaction = false;
                transaction.Commit();
                if (transaction.Connection != null && transaction.Connection.State == ConnectionState.Open)
                    transaction.Connection.Close();

                transaction = null;
            }
        }

        public virtual void RollbackTransaction() 
        {
            var transaction = GetTransaction();
            if (IsInTransaction && transaction != null)
            {
                IsInTransaction = false;
                transaction.Rollback();
                if (transaction.Connection != null && transaction.Connection.State == ConnectionState.Open)
                    transaction.Connection.Close();

                transaction = null;
            }
        }

        public virtual void CreateTable<T>(bool checkExisting = false, bool useCurrentConnection = false)
        {
            var info = TypeDataHandler.GetTableInfoByTypeAndUsage<T>();
        }


        public virtual T GetOne<T>(int id) where T : new() { return default; }

        public virtual T GetOne<T>(int id, bool useCurrentConnection) where T : new() { return default; }

        public virtual List<T> GetMany<T>(string sqlcommand, bool isStoredProcedure) where T : new()
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

            if (!IsInTransaction)
                command.Connection.Close();


            return res;
        }

        public virtual List<T> GetMany<T>(string sqlcommand, bool isStoredProcedure, bool useCurrentConnection) where T : new() { return default; }

        public virtual List<T> GetMany<T>(string sqlcommand, bool isStoredProcedure, bool useCurrentConnection, List<IntwentySqlParameter> parameters) where T : new() { return default; }

        public virtual int Insert<T>(T model)
        {
            var info = TypeDataHandler.GetTableInfoByTypeAndUsage<T>();
            var query = new StringBuilder(string.Format("INSERT INTO {0} (", info.Name));
            var values = new StringBuilder(" VALUES (");
            var parameters = new List<IntwentySqlParameter>();

            foreach (var col in info.Columns.Where(p => !p.IsIgnore).OrderBy(p => p.Order))
            {
                if (col.IsAutoIncremental)
                    continue;

                var value = GetObjectValue(col, model);

                var prm = new IntwentySqlParameter();
                prm.Name = "@" + col.Name;

                if (value == null)
                    prm.Value = DBNull.Value;
                else
                    prm.Value = value;

                query.Append(GetInsertColumnSql(col));
                values.Append(GetInsertParameterValueSql(col));

                parameters.Add(prm);

            }
            query.Append(") ");
            values.Append(")");

            SetInsertQueryAutoIncSql(info, parameters, values);

            var command = GetCommand();
            command.CommandText = query.ToString() + values.ToString();
            command.CommandType = CommandType.Text;

            AddCommandParameters(parameters);

            var res = command.ExecuteNonQuery();
           
            SetInsertQueryAutoIncResult(info, parameters, model);

            if (!IsInTransaction)
                command.Connection.Close();

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
            

        protected virtual object GetObjectValue<T>(IntwentyDataColumn column, T instance)
        {
            return column.Property.GetValue(instance);
        }

        protected virtual string GetInsertColumnSql(IntwentyDataColumn col)
        {
            if (col.Order == 0)
                return string.Format("{0}", col.Name);
            else
                return string.Format(",{0}", col.Name);
        }
        protected virtual string GetInsertParameterValueSql(IntwentyDataColumn col)
        {
            if (col.Order == 0)
                return string.Format("@{0}", col.Name);
            else
                return string.Format(",@{0}", col.Name);
        }

        protected virtual void SetInsertQueryAutoIncSql(IntwentyDataTable info, List<IntwentySqlParameter> parameters, StringBuilder values)
        {
            /*
             if (!string.IsNullOrEmpty(autoinccolumn) && DbEngine == DBMS.MSSqlServer)
            {
                values.Append(" select @NewId=Scope_Identity()");
                parameters.Add(new IntwentyParameter() { ParameterName = "@NewId", Direction = ParameterDirection.Output, DataType = DbType.Int32 });
            }
            */
             
        }

        protected abstract void SetInsertQueryAutoIncResult<T>(IntwentyDataTable info, List<IntwentySqlParameter> parameters, T instance);

      
       
    }
}
