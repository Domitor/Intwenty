using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.Dto
{

    public class ListFilter
    {
        public static readonly string DEFAULT_OWNERUSERID = "INTWENTY_USER";

        public int ApplicationId { get; set; }

        public int ApplicationViewId { get; set; }

        public string OwnerUserId { get; set; }

        public string OwnerOrganizationId { get; set; }

        public string DataViewMetaCode { get; set; }

        public int MaxCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public List<FilterValue> FilterValues { get; set; }


        public bool HasFilter
        {
            get
            {
                return FilterValues != null && FilterValues.Count > 0;

            }
        }

        public bool HasOwnerUserId
        {
            get { return !string.IsNullOrEmpty(OwnerUserId) && OwnerUserId != DEFAULT_OWNERUSERID; }
        }

        public ListFilter()
        {
            PageSize = 20;
            OwnerUserId = DEFAULT_OWNERUSERID;
            FilterValues = new List<FilterValue>();
        }

    }

    public class FilterValue : IntwentyDataColumn
    {
        public string Value { get; set; }

        public bool ExactMatch { get; set; }

    }
}
