using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace Intwenty.Model
{
    public abstract class BaseModelItem : HashTagPropertyObject
    {
        public int Id { get; set; }

        public static readonly string MetaTypeRoot = "ROOT";

        private static int MetaCodeCounter = 1;

        public string Title { get; set; }

        public string LocalizedTitle { get; set; }

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public string MetaType { protected set;  get; }

        public abstract bool HasValidMetaType { get; }

        public abstract bool HasValidProperties { get; }

        public abstract string ModelCode { get; }

      

        public bool IsRoot
        {
            get
            {
                return ParentMetaCode == "" || ParentMetaCode == "ROOT";
            }
        }

       

        public static string GenerateNewMetaCode(BaseModelItem item)
        {
            if (item == null)
                return GetQuiteUniqueString();

            var res = GetMetaTypeAbbreviation(item);
           
            var title = "";
            if (item is UserInterfaceStructureModelItem ui)
            {
                if (ui.IsDataColumn1Connected)
                    title += ui.DataColumn1Info.DbName.ToUpper();
                else if (ui.IsDataTableConnected)
                    title += ui.DataTableInfo.DbName.ToUpper();
            }
            else if (item is DatabaseModelItem db)
            {
                title += db.DbName.ToUpper();
            }
            else
            {
                title = item.Title;
            }

            if (title.Length > 0)
            {
                var titlechars = Array.FindAll(title.ToCharArray(), (c => (char.IsLetterOrDigit(c))));
                if (titlechars.Length > 0)
                    title = new string(titlechars);

                if (title.Length > 9)
                    title = title.Substring(0, 9).ToUpper();
            }

            if (title.Length > 0)
                res += "_" + title;
           

            MetaCodeCounter += 1;

            res += "_" + MetaCodeCounter;

            Guid g = Guid.NewGuid();
            var str = Convert.ToBase64String(g.ToByteArray());
            char[] arr = str.ToCharArray();
            arr = Array.FindAll(arr, (c => (char.IsLetterOrDigit(c))));
            str = new string(arr);
            res += "_" + str.Substring(0, 6).ToUpper();

            return res;
        }

        private static string GetMetaTypeAbbreviation(BaseModelItem item)
        {
            if (item.MetaType == ApplicationModelItem.MetaTypeApplication || item is ApplicationModelItem)
                return "APP";
            if (item.MetaType == SystemModelItem.MetaTypeSystem || item is SystemModelItem)
                return "SYS";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeCheckBox)
                return "CBOX";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeCheckBox)
                return "CBOX";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeComboBox)
                return "CB";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeDatePicker)
                return "DP";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeEditGrid)
                return "TBL";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeEditGridCheckBox)
                return "TBL_CBOX";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeEditGridComboBox)
                return "TBL_CB";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeEditGridDatePicker)
                return "TBL_DP";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeEditGridNumBox)
                return "TBL_NUMBOX";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeEditGridTextBox)
                return "TBL_TB";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeTextListColumn)
                return "TLCOL";
            if (item.MetaType == ViewModelItem.MetaTypeUIView)
                return "IVIEW";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeInputUI)
                return "IUI";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeListUI)
                return "LUI";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeLookUp)
                return "LOOKUP";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeNumBox)
                return "NUMBOX";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypePanel)
                return "PNL";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeSection)
                return "SECT";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeTextArea)
                return "TA";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeTextBox)
                return "TB";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeEditGridLookUp)
                return "TBL_LOOKUP";
            if (item.MetaType == UserInterfaceStructureModelItem.MetaTypeEmailBox)
                return "EB";
            if (item.MetaType == DatabaseModelItem.MetaTypeDataColumn)
                return "DCOL";
            if (item.MetaType == DatabaseModelItem.MetaTypeDataTable)
                return "DTBL";
            if (item.MetaType == DataViewModelItem.MetaTypeDataView)
                return "DV";
            if (item.MetaType == DataViewModelItem.MetaTypeDataViewColumn)
                return "DV_COL";
            if (item.MetaType == DataViewModelItem.MetaTypeDataViewKeyColumn)
                return "DV_KFCOL";
            if (item.MetaType == EndpointModelItem.MetaTypeDataViewList)
                return "EP_DV";
            if (item.MetaType == EndpointModelItem.MetaTypeTableList)
                return "EP_TB_L";
            if (item.MetaType == EndpointModelItem.MetaTypeTableGet)
                return "EP_TB_G";
            if (item.MetaType == EndpointModelItem.MetaTypeTableSave)
                return "EP_TB_S";

            return item.MetaType;
        }


        public static string GetQuiteUniqueString()
        {
            Guid g = Guid.NewGuid();
            var str = Convert.ToBase64String(g.ToByteArray());
            var t = DateTime.Now.ToLongTimeString().Replace(":", "").Replace(" ", "");

            if (str.Length > 4)
                str = str.Insert(3, t);

            char[] arr = str.ToCharArray();
            arr = Array.FindAll(arr, (c => (char.IsLetterOrDigit(c))));
            str = new string(arr);

            if (str.Length > 20)
                str = str.Substring(0, 20).ToUpper();


            return str;

        }

    }
}
