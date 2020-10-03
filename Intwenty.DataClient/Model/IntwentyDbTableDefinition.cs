using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.Model
{
    sealed class IntwentyDbTableDefinition : DbBaseDefinition
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

        public List<IntwentyDbIndexDefinition> Indexes { get; set; }

        public List<IntwentyDbColumnDefinition> Columns { get; set; }




        private string pkcolnames { get; set; }

       

        public IntwentyDbTableDefinition()
        {

            Columns = new List<IntwentyDbColumnDefinition>();
            Indexes = new List<IntwentyDbIndexDefinition>();
            pkcolnames = string.Empty;
            PrimaryKeyColumnNamesList = new List<string>();
        }

     
       

    }
}
