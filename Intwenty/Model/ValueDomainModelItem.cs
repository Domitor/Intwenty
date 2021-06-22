using Intwenty.Entity;
using Intwenty.Interface;
using System.Collections.Generic;


namespace Intwenty.Model
{
    public class ValueDomainModelItem : HashTagPropertyObject, ILocalizableTitle
    {

        public ValueDomainModelItem()
        {
            SetDefaults();
        }

        public ValueDomainModelItem(ValueDomainItem entity)
        {
            Id = entity.Id;
            DomainName = entity.DomainName;
            Code = entity.Code;
            Value = entity.Value;
            Title = entity.Value;
            LocalizedTitle = entity.Value;
            TitleLocalizationKey = entity.ValueLocalizationKey;
            Properties = entity.Properties;
            SetDefaults();
        }

        private void SetDefaults()
        {
            if (string.IsNullOrEmpty(DomainName)) DomainName = string.Empty;
            if (string.IsNullOrEmpty(Code)) Code = string.Empty;
            if (string.IsNullOrEmpty(Value)) Value = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(LocalizedTitle)) LocalizedTitle = string.Empty;
            if (string.IsNullOrEmpty(TitleLocalizationKey)) TitleLocalizationKey = string.Empty;
        }

      
        public int Id { get; set; }

        public string TitleLocalizationKey { get; set; }

        public string DomainName { get; set; }

        public string Code { get; set; }

        public string Value { get; set; }

        public string Title { get; set; }

        public string LocalizedTitle { get; set; }

        public bool IsValid
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
