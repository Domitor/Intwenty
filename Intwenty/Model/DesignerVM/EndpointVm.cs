using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.DesignerVM
{
    public class EndpointVm
    {

        public List<EndpointModelItem> Endpoints { get; set; }

        public List<ValueDomainModelItem> EndpointActions { get; set; }

        public List<EndpointDataSourceType> EndpointDataSourceTypes { get; set; }

        public List<EndpointDataSource> EndpointDataSources { get; set; }


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
