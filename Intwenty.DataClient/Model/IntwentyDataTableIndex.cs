﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.Model
{
    public class IntwentyDataTableIndex : DbBaseObject
    {

        public bool IsUnique { get; set; }

        public string ColumnNames
        {
            get { return colnames; }
            set
            {
                colnames = value;
                CreateStringList(ColumnNamesList, colnames);
            }
        }

        public List<string> ColumnNamesList { get; private set; }

        private string colnames { get; set; }

        public IntwentyDataTableIndex()
        {
            colnames = string.Empty;
        }
    }
}
