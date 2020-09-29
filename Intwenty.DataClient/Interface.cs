using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient
{
    public interface ISqlClient
    {
        public void Open();

        public void Close();

        public void AddParameter(string name, object value);

        T GetOne<T>(int id) where T : new();
    }

    public interface INoSqlClient
    {
        T GetOne<T>(int id) where T : new();
    }
}
