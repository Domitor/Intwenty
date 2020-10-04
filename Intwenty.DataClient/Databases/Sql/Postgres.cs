using Intwenty.DataClient.Model;
using Intwenty.DataClient.SQLBuilder;
using Npgsql;
using System.Collections.Generic;
using System.Data;


namespace Intwenty.DataClient.Databases.Sql
{
    sealed class Postgres : BaseSqlDb, ISqlClient
    {

        private NpgsqlConnection connection;
        private NpgsqlCommand command;
        private NpgsqlTransaction transaction;

        public Postgres(string connectionstring) : base(connectionstring)
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

        private NpgsqlConnection GetConnection()
        {

            if (connection != null && connection.State == ConnectionState.Open)
                return connection;

            connection = new NpgsqlConnection();
            connection.ConnectionString = this.ConnectionString;
            connection.Open();
            return connection;
        }

        protected override IDbCommand GetCommand()
        {
            command = new NpgsqlCommand();
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
                var param = new NpgsqlParameter() { ParameterName = p.Name, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                command.Parameters.Add(param);

            }
        }


        protected override BaseSqlBuilder GetSqlBuilder()
        {
            return new PostgresBuilder();
        }

       

        protected override void HandleInsertAutoIncrementation<T>(IntwentyDbTableDefinition info, List<IntwentySqlParameter> parameters, T instance)
        {
            throw new System.NotImplementedException();
        }
    }
}
