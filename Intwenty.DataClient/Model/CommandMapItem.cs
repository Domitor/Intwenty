using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.Model
{
    public class CommandMapItem
    {
        public string Key { get; set; }

        public SqlDBMS DbEngine { get; set; }

        public string Command { get; set; }

        public CommandMapItem()
        {
            Command = "";
        }
    }
}
