using Intwenty.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intwenty.MetaDataService.Model
{
    public class TestDataBatch : BaseModelItem
    {

        public TestDataBatch()
        {
            SetEmptyStrings();
        }

        public TestDataBatch(SystemID entity)
        {
            Id = entity.Id;
            Properties = entity.Properties;
            MetaCode = entity.MetaCode;
            MetaType = entity.MetaType;
            SetEmptyStrings();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(MetaType)) MetaType = string.Empty;
          
        }


        public override bool HasValidMetaType
        {
            get { return true; }
        }

        public string BatchName
        {
            get
            {
                return GetPropertyValue("TESTDATABATCH");
            }

        }
    }
}
