using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.DesignerVM
{
    public class EndpointCollectionVm
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

        public string Description { get; set; }

        public static EndpointVm CreateEndpointVm(EndpointModelItem model)
        {
            var t = new EndpointVm();
            t.Action = model.Action;
            t.Path = model.Path;
            t.DataSource = model.AppMetaCode + "|" + model.DataMetaCode;
            t.EndpointType = model.MetaType;
            t.Id = model.Id;
            t.Properties = model.Properties;

            return t;
        }

        public static EndpointModelItem CreateEndpointModelItem(EndpointVm model)
        {
            var t = new EndpointModelItem(model.EndpointType);
            t.Action = model.Action;
            t.Path = model.Path;
            var check = model.DataSource.Split('|');
            if (check.Length > 1)
            {
                t.AppMetaCode = check[0];
                t.DataMetaCode = check[1];
            }
            else if(check.Length == 1)
            {
                t.DataMetaCode = check[0];
            }
            t.Id = model.Id;

            t.Properties = model.Properties;


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
