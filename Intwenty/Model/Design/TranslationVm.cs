using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Intwenty.Model.Design
{

    public class TranslationManagementVm
    {
        public bool ModelSaved { get; set; }

        public List<TranslationVm> Translations { get; set; }
    }

    public class TranslationVm : TranslationModelItem
    {

        public TranslationVm()
        {
            TranslationType = 1;
        }

        public int ApplicationModelId { get; set; }

        public int UserInterfaceModelId { get; set; }

        public int ViewModelId { get; set; }

        public int FunctionModelId { get; set; }

        public string ModelTitle { get; set; }

        //1 Title, 2 Description
        public int TranslationType { get; set; }

        public bool Changed { get; set; }

        public static TranslationVm CreateTranslationVm(TranslationModelItem model)
        {
            return new TranslationVm() { Culture = model.Culture, Id = model.Id, Key = model.Key, Properties = model.Properties, Text = model.Text };
        }

        public static TranslationModelItem CreateTranslationModelItem(TranslationVm model)
        {
            return  new TranslationModelItem() { Culture = model.Culture, Id = model.Id, Key = model.Key, Properties = model.Properties, Text = model.Text };
        }

    }

  
    
}
