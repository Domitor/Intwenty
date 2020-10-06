
using Intwenty.Model;

namespace Intwenty.Data.XDBAccess
{
    public abstract class IntwentyDbClient : IIntwentyDb
    {
         public string ConnectionString { get; set; }

        public DBMS DbEngine { get; set; }

        public bool IsNoSql
        {
            get 
            {
                if (DbEngine == DBMS.LiteDb || DbEngine == DBMS.MongoDb)
                    return true;

                return false;
            } 
        }

    }
}
