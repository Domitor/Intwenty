using System;
using System.Collections.Generic;
using System.Linq;


namespace Intwenty.MetaDataService.Model
{
    public abstract class BaseModelItem
    {
        public int Id { get; set; }

        public static readonly string MetaTypeRoot = "ROOT";

        private static int MetaCodeCounter = 1;

        public virtual bool IsValid
        {
            get { return false; }
        }

        public string Title { get; set; }

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public string MetaType { protected set;  get; }

        public abstract bool HasValidMetaType { get; }

        public virtual bool HasValidProperties
        {
            get { return false; }
        }

        public string Properties { get; set; }

     
        public bool IsRoot
        {
            get
            {
                return ParentMetaCode == "" || ParentMetaCode == "ROOT";
            }
        }

        public bool HasProperties
        {
            get { return !string.IsNullOrEmpty(Properties); }
        }

        public void AddUpdateProperty(string key, string value)
        {
            var res = string.Empty;

            try
            {

                if (!string.IsNullOrEmpty(Properties))
                {
                    var arr = Properties.Split("#".ToCharArray());
                    foreach (String v in arr)
                    {
                        String[] keyval = v.Split("=".ToCharArray());
                        if (keyval[0].ToUpper() != key.ToUpper())
                        {
                            if (string.IsNullOrEmpty(res))
                                res = v;
                            else
                                res += "#" + v;
                        }
                    }
                }

                if (string.IsNullOrEmpty(res))
                    res = key + "=" + value;
                else
                    res += "#" + key + "=" + value;

                Properties = res;
            }
            catch { }

        }

        public string GetPropertyValue(string propertyname)
        {
            if (string.IsNullOrEmpty(Properties))
                return string.Empty;

            if (string.IsNullOrEmpty(propertyname))
                return string.Empty;

            var arr = Properties.Split("#".ToCharArray());

            foreach (var pair in arr)
            {
                if (pair != string.Empty)
                {
                    var keyval = pair.Split("=".ToCharArray());
                    if (keyval.Length < 2)
                        return string.Empty;

                    if (keyval[0].ToUpper() == propertyname.ToUpper())
                        return keyval[1];
                }
            }

            return string.Empty;
        }

        public bool HasProperty(string propertyname)
        {
            try
            {
                if (string.IsNullOrEmpty(propertyname))
                    return false;

                if (!string.IsNullOrEmpty(Properties))
                {
                    var arr = Properties.Split("#".ToCharArray());
                    foreach (var v in arr)
                    {
                        var keyval = v.Split("=".ToCharArray());
                        if (keyval[0].ToUpper() == propertyname.ToUpper())
                            return true;
                    }
                }
            }
            catch { }

            return false;
        }

        public bool HasPropertyWithValue(string propertyname, object value)
        {
            var t = GetPropertyValue(propertyname);
            if (string.IsNullOrEmpty(t))
                return false;

            var compare = string.Empty;
            if (value != null)
                compare = Convert.ToString(value);


            if (t == compare)
                return true;


            return false;
        }

        public List<string> GetProperties()
        {
            var res = new List<string>();

            try
            {
                if (string.IsNullOrEmpty(Properties))
                    return res;

                var arr = Properties.Split("#".ToCharArray());

                foreach (var v in arr)
                {
                    var keyval = v.Split("=".ToCharArray());
                    res.Add(keyval[0].ToUpper());
                }
                
            }
            catch { }

            return res;
        }


        public static string GenerateNewMetaCode(BaseModelItem item)
        {
            if (item == null)
                return GetQuiteUniqueString();

            var res = GetMetaTypeAbbreviation(item);
            var title = "";
            if (item is UserInterfaceModelItem ui)
            {
                if (ui.IsDataConnected)
                    title += ui.DataInfo.DbName.ToUpper();
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
            if (item.MetaType == UserInterfaceModelItem.MetaTypeListViewField)
                return "LV_FLD";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeLookUp)
                return "LOOKUP";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeLookUpField)
                return "LOOKUP_FLD";
            if (item.MetaType == UserInterfaceModelItem.MetaTypeLookUpKeyField)
                return "LOOKUP_KFLD";
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
            if (item.MetaType == UserInterfaceModelItem.MetaTypeEditGridLookUpKeyField)
                return "TBL_LOOKUP_KF";
            if (item.MetaType == DatabaseModelItem.MetaTypeDataColumn)
                return "DCOL";
            if (item.MetaType == DatabaseModelItem.MetaTypeDataTable)
                return "DTBL";
            if (item.MetaType == ValueDomainModelItem.MetaTypeValueDomain)
                return "VDOM";
            if (item.MetaType == DataViewModelItem.MetaTypeDataView)
                return "DV";
            if (item.MetaType == DataViewModelItem.MetaTypeDataViewField)
                return "DV_FLD";
            if (item.MetaType == DataViewModelItem.MetaTypeDataViewKeyField)
                return "DV_KFLD";
            if (item.MetaType == MenuModelItem.MetaTypeMainMenu)
                return "MAINMENU";
            if (item.MetaType == MenuModelItem.MetaTypeMenuItem)
                return "MENITM";

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
