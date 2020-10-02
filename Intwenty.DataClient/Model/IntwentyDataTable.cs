using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.Model
{
    sealed class IntwentyDataTable : DbBaseObject
    {
       

        public string PrimaryKeyColumnNames 
        {
            get { return pkcolnames; }
            set 
            {
                pkcolnames = value;
                CreateStringList(PrimaryKeyColumnNamesList, pkcolnames);
            }
        }

        public List<string> PrimaryKeyColumnNamesList { get; private set; }

        public List<IntwentyDataTableIndex> Indexes { get; set; }

        public List<IntwentyDataColumn> Columns { get; set; }




        private string pkcolnames { get; set; }

       

        public IntwentyDataTable()
        {

            Columns = new List<IntwentyDataColumn>();
            Indexes = new List<IntwentyDataTableIndex>();
            pkcolnames = string.Empty;
            PrimaryKeyColumnNamesList = new List<string>();
        }

     
       

    }
}
