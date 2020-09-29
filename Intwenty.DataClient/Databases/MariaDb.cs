using MySqlConnector;
using System.Data;



namespace Intwenty.DataClient.Databases
{
    public class MariaDb : BaseSqlDb, ISqlClient
    {

        private MySqlConnection connection;
        private MySqlCommand command;

        public MariaDb(string connectionstring) : base(connectionstring)
        {

        }

        protected override IDbConnection GetConnection()
        {
            return connection;
        }

        protected override IDbCommand GetCommand()
        {
            return command;
        }

        public override void Open()
        {
            connection = new MySqlConnection();
            connection.ConnectionString = ConnectionString;
            connection.Open();
        }

        public override void AddParameter(string name, object value)
        {
            this.command.Parameters.Add(new MySqlParameter() { Value = value, ParameterName = name });
        }

    }
}
