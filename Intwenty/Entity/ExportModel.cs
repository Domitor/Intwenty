using System;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Entity
{
    public class ExportModel
    {

        public bool DeleteCurrentModel { get; set; }

        public DateTime ModelDate { get; set; }

        public List<SystemItem> Systems { get; set; }

        public List<ApplicationItem> Applications { get; set; }

        public List<DatabaseItem> DatabaseItems { get; set; }

        public List<UserInterfaceItem> UserInterfaceItems { get; set; }

        public List<DataViewItem> DataViewItems { get; set; }

        public List<TranslationItem> Translations { get; set; }

        public List<ValueDomainItem> ValueDomains { get; set; }

        public List<EndpointItem> Endpoints { get; set; }


        public ExportModel()
        {
            ModelDate = DateTime.Now;
            Applications = new List<ApplicationItem>();
            DatabaseItems = new List<DatabaseItem>();
            UserInterfaceItems = new List<UserInterfaceItem>();
            DataViewItems = new List<DataViewItem>();
            Translations = new List<TranslationItem>();
            ValueDomains = new List<ValueDomainItem>();
            Endpoints = new List<EndpointItem>();
            Systems = new List<SystemItem>();
        }


    }
}
