using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.DesignerVM
{
    public class EndpointManagementVm
    {

        public List<EndpointVm> Endpoints { get; set; }

        public List<ValueDomainModelItem> EndpointActions { get; set; }

        public List<EndpointDataSourceType> EndpointDataSourceTypes { get; set; }

        public List<EndpointDataSource> EndpointDataSources { get; set; }


    }

    public class EndpointVm : BaseModelVm
    {
        public string EndpointType { get; set; }

        public string DataSource { get; set; }

        public string Path { get; set; }

        public string Action { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public bool Expanded { get; set; }

        public static EndpointVm CreateEndpointVm(EndpointModelItem model)
        {
            var t = new EndpointVm();
            t.Action = model.Action;
            t.Path = model.Path;

            if (model.IsMetaTypeTableOperation)
                t.DataSource = model.AppMetaCode + "|" + model.DataMetaCode;
            else
                t.DataSource = model.DataMetaCode;

            t.EndpointType = model.MetaType;
            t.Id = model.Id;
            t.Properties = model.Properties;
            t.Description = model.Description;
            t.Title = model.Title;

            return t;
        }

        public static EndpointModelItem CreateEndpointModelItem(EndpointVm model)
        {
            var t = new EndpointModelItem(model.EndpointType);
            t.Action = model.Action;
            t.Path = model.Path;
            var check = model.DataSource.Split('|');
            if (t.IsMetaTypeTableOperation)
            {
                t.AppMetaCode = check[0];
                t.DataMetaCode = check[1];
            }
            else
            {
                t.DataMetaCode = model.DataSource;
            }
            t.Id = model.Id;

            t.Properties = model.Properties;
            t.Description = model.Description;
            t.Title = model.Title;
            
            t.RemoveProperty("CHANGED");


            return t;
        }

    }

    public class EndpointDataSourceType 
    {
        public string id { get; set; }

        public string title { get; set; }

    }

    public class EndpointDataSource
    {
        public string id { get; set; }

        public string type { get; set; }

        public string title { get; set; }

    }
}
