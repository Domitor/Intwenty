using Intwenty.DataClient.Model;
using MySqlConnector;
using System.Collections.Generic;
using System.Data;



namespace Intwenty.DataClient.Databases
{
    public sealed class MariaDb : BaseSqlDb, ISqlClient
    {

        private MySqlConnection connection;
        private MySqlCommand command;
        private MySqlTransaction transaction;

        public MariaDb(string connectionstring) : base(connectionstring)
        {

        }

        public override void Dispose()
        {
            transaction = null;
            command = null;
            transaction = null;
            IsInTransaction = false;

        }

        private MySqlConnection GetConnection()
        {

            if (connection != null && connection.State == ConnectionState.Open)
                return connection;

            connection = new MySqlConnection();
            connection.ConnectionString = this.ConnectionString;
            connection.Open();
            return connection;
        }

        protected override IDbCommand GetCommand()
        {
            command = new MySqlCommand();
            command.Connection = GetConnection();
            if (IsInTransaction && transaction != null)
                command.Transaction = transaction;

            return command;
        }

        protected override IDbTransaction GetTransaction()
        {
            return transaction;
        }

        protected override void AddCommandParameters(List<IntwentySqlParameter> parameters)
        {
            foreach (var p in parameters)
            {
                var param = new MySqlParameter() { ParameterName = p.Name, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                command.Parameters.Add(param);

            }
        }

        protected override void SetInsertQueryAutoIncResult<T>(IntwentyDataTable info, List<IntwentySqlParameter> parameters, T instance)
        {
           
        }




    }
}
