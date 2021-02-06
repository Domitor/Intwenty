using Intwenty.Entity;
using Intwenty.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Intwenty.Model
{
   
    public class ApplicationVm 
   {
      

        public ApplicationVm()
        {
            MetaType = ApplicationModelItem.MetaTypeApplication;
            SetEmptyStrings();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(Description)) Description = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(TitleLocalizationKey)) TitleLocalizationKey = string.Empty;
            if (string.IsNullOrEmpty(SystemMetaCode)) TitleLocalizationKey = string.Empty;
        }

        public int Id { get; set; }
        public string MetaType { get; set; }
        public int TenantIsolationLevel { get; set; }
        public int TenantIsolationMethod { get; set; }
        public int DataMode { get; set; }
        public string Title { get; set; }
        public string MetaCode { get; set; }
        public string SystemMetaCode { get; set; }
        public string TitleLocalizationKey { get; set; }
        public string Description { get; set; }
        public string DbName { get; set; }
        public string Properties { get; set; }
        public bool UseVersioning { get; set; }

      

    }
    
}
