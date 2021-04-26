using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Intwenty.Model.Dto
{

    public class ListFilter
    {
       
        public UserInfo User { get; set; }

        public int ApplicationId { get; set; }

        public int ApplicationViewId { get; set; }

        /// <summary>
        /// If this is blank the main table of the application will be used otherwise the table specified
        /// </summary>
        public string DataTableDbName { get; set; }

        public bool SkipPaging { get; set; }

        public bool ForceCurrentUserFilter { get; set; }

        public bool ForceCurrentOrganizationFilter { get; set; }

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

       

        public ListFilter()
        {
            PageSize = 20;
            User = new UserInfo();
            FilterValues = new List<FilterValue>();
        }

        public ListFilter(ClaimsPrincipal user)
        {
            PageSize = 20;
            User = new UserInfo(user);
            FilterValues = new List<FilterValue>();
        }

        public void SetUser(ClaimsPrincipal user)
        {
            User = new UserInfo(user);
        }

    }

    public class FilterValue : IntwentyDataColumn
    {
        public string Value { get; set; }

        public bool ExactMatch { get; set; }

    }
}
