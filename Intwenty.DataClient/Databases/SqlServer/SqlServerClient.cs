using Intwenty.DataClient.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Intwenty.DataClient.Databases.SqlServer
{
    sealed class SqlServerClient : BaseDb, ISqlClient
    {

        private SqlConnection connection;
        private SqlCommand command;
        private SqlTransaction transaction;

        public SqlServerClient(string connectionstring) : base(connectionstring)
        {

        }

        public override void Dispose()
        {
            transaction = null;
            command = null;
            transaction = null;
            IsInTransaction = false;

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
            command = new SqlCommand();
            command.Connection = GetConnection();
            if (IsInTransaction && transaction != null)
                command.Transaction = transaction;

            return command;
        }

        protected override IDbTransaction GetTransaction()
        {
            return transaction;
        }

        protected override void AddCommandParameters(IIntwentySqlParameter[] parameters)
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

        protected override void HandleInsertAutoIncrementation<T>(IntwentyDbTableDefinition model, List<IntwentySqlParameter> parameters, T entity)
        {
        }


    }
}
