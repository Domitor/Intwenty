using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.Design
{
    public class ApplicationViewVm :BaseModelVm
    {
        public int ApplicationId { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsPublic { get; set; }
        public bool SaveFunction { get; set; }
        public string SaveFunctionTitle { get; set; }
        public bool NavigateFunction { get; set; }
        public string NavigateFunctionTitle { get; set; }
        public string NavigateFunctionPath { get; set; }
        public string NavigateFunctionActionMetaCode { get; set; }


    }
}
