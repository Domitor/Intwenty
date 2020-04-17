using System;
using Intwenty.Model;
using System.Collections.Generic;

namespace Intwenty.Model.DesignerVM
{
   

    public class NoSeriesVm
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int StartValue { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string Prefix { get; set; }
        public string Description { get; set; }

        public NoSeriesVm()
        {
            StartValue = 1000;
            TableName = "";
            ColumnName = "";
            Prefix = "";
            Description = "";
        }


    }
}
