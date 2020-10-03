using System.Data;


namespace Intwenty.DataClient.Model
{
    public class IntwentySqlParameter : IIntwentySqlParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public DbType DataType { get; set; }

        public ParameterDirection Direction { get; set; }

        public IntwentySqlParameter()
        {
            Direction = ParameterDirection.Input;
            DataType = DbType.String;
        }
    }
}
