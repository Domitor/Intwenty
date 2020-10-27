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

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public string MetaType { protected set;  get; }

        public abstract bool HasValidMetaType { get; }

        public abstract bool HasValidProperties { get; }


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
            if (item is UserInterfaceModelItem ui)
            {
                if (ui.IsDataColumnConnected)
                    title += ui.DataColumnInfo.DbName.ToUpper();
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
            if (item.MetaType == UserInterfaceModelItem.MetaTypeCheckBox)
                return "CBOX";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeComboBox)
                return "CB";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeDatePicker)
                return "DP";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeEditGrid)
                return "TBL";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeEditGridCheckBox)
                return "TBL_CBOX";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeEditGridComboBox)
                return "TBL_CB";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeEditGridDatePicker)
                return "TBL_DP";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeEditGridNumBox)
                return "TBL_NUMBOX";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeEditGridTextBox)
                return "TBL_TB";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeListView)
                return "LV";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeListViewColumn)
                return "LV_COL";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeLookUp)
                return "LOOKUP";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeNumBox)
                return "NUMBOX";
            if (item.MetaType == UserInterfaceModelItem.MetaTypePanel)
                return "PNL";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeSection)
                return "SECT";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeTextArea)
                return "TA";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeTextBox)
                return "TB";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeEditGridLookUp)
                return "TBL_LOOKUP";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeEmailBox)
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
            if (item.MetaType == MenuModelItem.MetaTypeMainMenu)
                return "MAINMENU";
            if (item.MetaType == MenuModelItem.MetaTypeMenuItem)
                return "MENITM";
          
        
            if (item.MetaType == EndpointModelItem.MetaTypeDataViewGetData)
                return "EP_DV_GD";
            if (item.MetaType == EndpointModelItem.MetaTypeTableGetAll)
                return "EP_TB_GA";
            if (item.MetaType == EndpointModelItem.MetaTypeTableGetById)
                return "EP_TB_GB";
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
