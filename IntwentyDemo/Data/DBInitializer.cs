using System;
using Intwenty.Data.Seed;

namespace IntwentyDemo.Data
{
    public static class DBInitializer
    {
        public static void Initialize(IServiceProvider provider)
        {
            SeedIdentity.Seed(provider);
            SeedSalesOrderDemoModel.Seed(provider); 
        }




    }
 }
