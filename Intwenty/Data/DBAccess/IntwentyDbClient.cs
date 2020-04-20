using Shared;


namespace Intwenty.Data.DBAccess
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
