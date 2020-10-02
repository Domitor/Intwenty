using System.Data.SQLite;
using System.Data;
using Intwenty.DataClient.Model;
using System;
using System.Collections.Generic;
using Intwenty.DataClient.SQLBuilder;

namespace Intwenty.DataClient.Databases.Sql
{
    sealed class SqlLite : BaseSqlDb, ISqlClient
    {

        private SQLiteConnection connection;
        private SQLiteCommand command;
        private SQLiteTransaction transaction;

        public SqlLite(string connectionstring) : base(connectionstring)
        {

        }

        public override void Dispose()
        {
            transaction = null;
            command = null;
            transaction = null;
            IsInTransaction = false;
             
        }

        private SQLiteConnection GetConnection()
        {

            if (connection != null && connection.State == ConnectionState.Open)
                return connection;

            connection = new SQLiteConnection();
            connection.ConnectionString = this.ConnectionString;
            connection.Open();

            if (IsInTransaction && transaction == null)
                transaction = connection.BeginTransaction();

            return connection;
        }

        protected override IDbCommand GetCommand()
        {
            command = new SQLiteCommand();
            command.Connection = GetConnection();
            if (IsInTransaction && transaction != null)
                command.Transaction = transaction;

            return command;
        }

        protected override IDbTransaction GetTransaction()
        {
            return transaction;
        }

     

        protected override void SetPropertyValues<T>(IDataReader reader, IntwentyDataColumn column, T instance)
        {
            if (column.Property.PropertyType.FullName.ToUpper().Contains("SYSTEM.DATETIMEOFFSET"))
            {
                column.Property.SetValue(instance, new DateTimeOffset(reader.GetDateTime(column.Order)), null);
            }
            else
            {
                base.SetPropertyValues(reader, column, instance);
            }
        }

      

        protected override void AddCommandParameters(List<IntwentySqlParameter> parameters)
        {
            foreach (var p in parameters)
            {
                var param = new SQLiteParameter() { ParameterName = p.Name, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                command.Parameters.Add(param);

            }
        }

        protected override void HandleInsertAutoIncrementation<T>(IntwentyDataTable model, List<IntwentySqlParameter> parameters, T entity)
        {
            var autoinccol = model.Columns.Find(p => p.IsAutoIncremental);
            if (autoinccol == null)
                return;

            var command = GetCommand();
            command.CommandText = "SELECT Last_Insert_Rowid()";
            command.CommandType = CommandType.Text;

            autoinccol.Property.SetValue(entity, Convert.ToInt32(command.ExecuteScalar()), null);

        }

        protected override BaseSqlBuilder GetSqlBuilder()
        {
            return new SqlLiteSqlBuilder();
        }

        public override void Open()
        {
            
        }

        public override void Close()
        {
            if (connection != null && connection.State != ConnectionState.Closed)
            {
                connection.Close();
                Dispose();
            }
        }
    }
}
