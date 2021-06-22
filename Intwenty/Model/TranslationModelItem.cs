using Intwenty.Entity;
using System.Collections.Generic;


namespace Intwenty.Model
{
    public class TranslationModelItem 
    {
        public int Id { get; set; }

        public string Culture { get; set; }

        public string Key { get; set; }

        public string Text { get; set; }

        public string Properties { get; set; }


        public TranslationModelItem()
        {
            SetDefaults();
        }

        public TranslationModelItem(TranslationItem entity)
        {
            Id = entity.Id;
            Culture = entity.Culture;
            Key = entity.TransKey;
            Text = entity.Text;
            SetDefaults();
        }

        private void SetDefaults()
        {
            if (string.IsNullOrEmpty(Culture)) Culture = string.Empty;
            if (string.IsNullOrEmpty(Key)) Key = string.Empty;
            if (string.IsNullOrEmpty(Text)) Text = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;

        }

      


    }

   
}
