using System.Data.SQLite;
using System.Data;
using Intwenty.DataClient.Model;
using System;
using System.Collections.Generic;


namespace Intwenty.DataClient.Databases.SQLite
{
    sealed class SQLiteClient : BaseDb, IDataClient
    {

        private SQLiteConnection connection;
        private SQLiteCommand command;
        private SQLiteTransaction transaction;

        public SQLiteClient(string connectionstring) : base(connectionstring)
        {

        }

        public DBMS Database { get { return DBMS.SQLite; } }

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

     

        protected override void SetPropertyValues<T>(IDataReader reader, IntwentyDbColumnDefinition column, T instance)
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

      

        protected override void AddCommandParameters(IIntwentySqlParameter[] parameters)
        {
            if (parameters == null)
                return;

            foreach (var p in parameters)
            {
                var param = new SQLiteParameter() { ParameterName = p.Name, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                command.Parameters.Add(param);

            }
        }

        protected override void HandleInsertAutoIncrementation<T>(IntwentyDbTableDefinition model, List<IntwentySqlParameter> parameters, T entity)
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
            return new SqlLiteBuilder();
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
