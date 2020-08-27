using Intwenty.Data.Entity;
using Intwenty.Interface;
using System.Collections.Generic;


namespace Intwenty.Model
{
    public class ValueDomainModelItem : BaseModelItem, ILocalizableTitle
    {

        public ValueDomainModelItem()
        {
            MetaType = MetaTypeValueDomain;
            ParentMetaCode = MetaTypeRoot;
            SetEmptyStrings();
        }

        public ValueDomainModelItem(ValueDomainItem entity)
        {
            Id = entity.Id;
            DomainName = entity.DomainName;
            Code = entity.Code;
            Value = entity.Value;
            Title = entity.Value;
            TitleLocalizationKey = entity.ValueLocalizationKey;
            Properties = entity.Properties;
            MetaType = MetaTypeValueDomain;
            MetaCode = DomainName;
            ParentMetaCode = MetaTypeRoot;
            SetEmptyStrings();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(DomainName)) DomainName = string.Empty;
            if (string.IsNullOrEmpty(Code)) Code = string.Empty;
            if (string.IsNullOrEmpty(Value)) Value = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(TitleLocalizationKey)) TitleLocalizationKey = string.Empty;
        }

        public static readonly string MetaTypeValueDomain = "VALUEDOMAIN";

        public static List<string> ValidProperties
        {
            get
            {
                var t = new List<string>();
                return t;
            }
        }

        public static List<string> ValidMetaTypes
        {
            get
            {
                var t = new List<string>();
                t.Add(MetaTypeValueDomain);
                return t;
            }
        }

        public string TitleLocalizationKey { get; set; }

        public string DomainName { get; set; }

        public string Code { get; set; }

        public string Value { get; set; }

        public override bool HasValidMetaType
        {
            get { return true;  }
        }


        public override bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(DomainName) || string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(Value))
                    return false;

                return true;
            }
        }
    }

   
}
