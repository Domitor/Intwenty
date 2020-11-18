using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Intwenty.Model.UIDesign
{
   

    public class TranslationVm : TranslationModelItem
    {
        public int CreateOnApplicationModelId { get; set; }

        public int CreateOnUserInterfaceModelId { get; set; }

        public string ModelTitle { get; set; }

        public static TranslationVm CreateTranslationVm(TranslationModelItem model)
        {
            return new TranslationVm() { Culture = model.Culture, Id = model.Id, Key = model.Key, Properties = model.Properties, Text = model.Properties };
        }

        public static TranslationModelItem CreateTranslationModelItem(TranslationVm model)
        {
            var t = new TranslationModelItem() { Culture = model.Culture, Id = model.Id, Key = model.Key, Properties = model.Properties, Text = model.Properties };
            return t;
        }

    }

  
    
}
