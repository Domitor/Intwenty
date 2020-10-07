using Intwenty.DataClient.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Intwenty.DataClient.Databases.SqlServer
{
    sealed class SqlServerClient : BaseDb, IDataClient
    {

        private SqlConnection connection;
        private SqlTransaction transaction;

        public SqlServerClient(string connectionstring) : base(connectionstring)
        {

        }

        public override DBMS Database { get { return DBMS.MSSqlServer; } }

        public override void Dispose()
        {
            connection = null;
            transaction = null;
            IsInTransaction = false;

        }

        public override void Open()
        {

        }

        public override void Close()
        {
            if (connection != null && connection.State != ConnectionState.Closed)
                connection.Close();

            Dispose();
        }

        private SqlConnection GetConnection()
        {

            if (connection != null && connection.State == ConnectionState.Open)
                return connection;

            connection = new SqlConnection();
            connection.ConnectionString = this.ConnectionString;
            connection.Open();
            return connection;
        }

        protected override IDbCommand GetCommand()
        {
            var command = new SqlCommand();
            command.Connection = GetConnection();
            if (IsInTransaction && transaction != null)
                command.Transaction = transaction;

            return command;
        }

        protected override IDbTransaction GetTransaction()
        {
            return transaction;
        }

        protected override void AddCommandParameters(IIntwentySqlParameter[] parameters, IDbCommand command)
        {
            if (parameters == null)
                return;

            foreach (var p in parameters)
            {
                var param = new SqlParameter() { ParameterName = p.Name, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                command.Parameters.Add(param);

            }
        }


        protected override BaseSqlBuilder GetSqlBuilder()
        {
            return new SqlServerBuilder();
        }

        protected override void SetPropertyValues<T>(IDataReader reader, IntwentyDbColumnDefinition column, T instance)
        {
            if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.INT32")
                column.Property.SetValue(instance, reader.GetInt32(column.Order), null);
            else if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.BOOLEAN")
                column.Property.SetValue(instance, Convert.ToBoolean(reader.GetInt32(column.Order)), null);
            else if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.DECIMAL")
                column.Property.SetValue(instance, Convert.ToDecimal(reader.GetValue(column.Order)), null);
            else if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.SINGLE")
                column.Property.SetValue(instance, Convert.ToSingle(reader.GetValue(column.Order)), null);
            else if (column.Property.PropertyType.ToString().ToUpper() == "SYSTEM.DOUBLE")
                column.Property.SetValue(instance, Convert.ToDouble(reader.GetValue(column.Order)), null);
            else
                column.Property.SetValue(instance, reader.GetValue(column.Order), null);

          
        }

        protected override void HandleInsertAutoIncrementation<T>(IntwentyDbTableDefinition model, List<IntwentySqlParameter> parameters, T entity, IDbCommand command)
        {
            if (model == null)
                return;

            if (command == null)
                return;

            if (command.Parameters == null || parameters == null)
                return;

            var output = parameters.Find(p => p.Direction == ParameterDirection.Output);
            if (output == null)
                return;

            foreach (SqlParameter p in command.Parameters)
            {
                if (p.Direction == ParameterDirection.Output && p.ParameterName.ToLower() == output.Name.ToLower())
                {
                    

                    if (!model.HasAutoIncrementalColumn)
                        return;

                    var autoinccol = model.Columns.Find(p => p.IsAutoIncremental);
                    if (autoinccol == null)
                        return;

                    autoinccol.Property.SetValue(entity, p.Value);

                    break;

                }
                   
            }



          

        }


    }
}
