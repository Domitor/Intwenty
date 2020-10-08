using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.DataClient.Model
{
    class DbBaseDefinition
    {
        public int Index { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        protected void CreateStringList(List<string> list, string stringtosplit)
        {
            if (list == null)
                list = new List<string>();

            list.Clear();

            var splits = stringtosplit.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (var c in splits)
            {
                list.Add(c);
            }
        }
    }

   
}
