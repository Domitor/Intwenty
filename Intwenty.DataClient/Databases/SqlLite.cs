using System.Data.SQLite;
using System.Data;

namespace Intwenty.DataClient.Databases
{
    public class SqlLite : BaseSqlDb, ISqlClient
    {

        private SQLiteConnection connection;
        private SQLiteCommand command;

        public SqlLite(string connectionstring) : base(connectionstring)
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
            connection = new SQLiteConnection();
            connection.ConnectionString = ConnectionString;
            connection.Open();

          
        }

        public override void AddParameter(string name, object value)
        {
            this.command.Parameters.Add(new SQLiteParameter() { Value = value, ParameterName = name });
        }

    }
}
