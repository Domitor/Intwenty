using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.Dto
{
    public class ListFilter
    {
        public static readonly string DEFAULT_OWNERUSERID = "SYSTEM";

        public int ApplicationId { get; set; }

        public string OwnerUserId { get; set; }

        public string DataViewMetaCode { get; set; }

        public int MaxCount { get; set; }

        public int CurrentRowNum { get; set; }

        public int BatchSize { get; set; }

        public string FilterField { get; set; }

        public string FilterValue { get; set; }

        public bool HasOwnerUserId
        {
            get { return !string.IsNullOrEmpty(OwnerUserId) && OwnerUserId != DEFAULT_OWNERUSERID; }
        }

        public ListFilter()
        {
            BatchSize = 50;
            OwnerUserId = DEFAULT_OWNERUSERID;
        }

    }
}
