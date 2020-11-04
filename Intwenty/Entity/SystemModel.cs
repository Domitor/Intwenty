using System;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Entity
{
    public class SystemModel
    {
        public bool DeleteCurrentModel { get; set; }

        public DateTime ModelDate { get; set; }

        public List<ApplicationItem> Applications { get; set; }

        public List<DatabaseItem> DatabaseItems { get; set; }

        public List<UserInterfaceItem> UserInterfaceItems { get; set; }

        public List<DataViewItem> DataViewItems { get; set; }

        public List<MenuItem> MenuItems { get; set; }

        public List<ValueDomainItem> ValueDomains { get; set; }

        public List<EndpointItem> Endpoints { get; set; }


        public SystemModel()
        {
            ModelDate = DateTime.Now;
            Applications = new List<ApplicationItem>();
            DatabaseItems = new List<DatabaseItem>();
            UserInterfaceItems = new List<UserInterfaceItem>();
            DataViewItems = new List<DataViewItem>();
            MenuItems = new List<MenuItem>();
            ValueDomains = new List<ValueDomainItem>();
            Endpoints = new List<EndpointItem>();
        }


    }
}
