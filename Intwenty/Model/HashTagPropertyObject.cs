using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Intwenty.Model
{
    public class HashTagPropertyObject
    {

        public string Properties { get; set; }

        public List<PropertyItem> PropertyList { get; set; }

        public HashTagPropertyObject()
        {
            PropertyList = new List<PropertyItem>();
        }

       
        public void BuildPropertyList()
        {
            PropertyList = new List<PropertyItem>();

            if (string.IsNullOrEmpty(Properties))
                return;

            var arr = Properties.Split("#".ToCharArray());

            foreach (var v in arr)
            {
                var keyval = v.Split("=".ToCharArray());
                if (keyval.Length == 2)
                {
                    var t = new PropertyItem() { PropertyCode = keyval[0].ToUpper(), PropertyValue = keyval[1].ToUpper(), Title = keyval[0].ToUpper(), PresentationValue = keyval[1].ToUpper() };
                    if (string.IsNullOrEmpty(t.PresentationValue))
                        t.PresentationValue = t.PropertyValue;

                    PropertyList.Add(t);
                }
            }
        }

        public string CompilePropertyString()
        {
            if (PropertyList == null)
                return string.Empty;

            if (PropertyList.Count == 0)
                return string.Empty;

            var res = string.Empty;

            foreach (var t in PropertyList)
            {

                var exists = false;
                if (!string.IsNullOrEmpty(res))
                {
                    var arr = res.Split("#".ToCharArray());
                    foreach (string v in arr)
                    {
                        string[] keyval = v.Split("=".ToCharArray());
                        if (keyval[0].ToUpper() == t.PropertyCode.ToUpper())
                            exists = true;
                    }
                }

                if (string.IsNullOrEmpty(res))
                    res = t.PropertyCode + "=" + t.PropertyValue;
                else
                {
                    if (!exists)
                         res += "#" + t.PropertyCode + "=" + t.PropertyValue;
                }
            }


            return res;


        }


        public void AddUpdateProperty(string key, string value)
        {
            var res = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Properties))
                {
                    var arr = Properties.Split("#".ToCharArray());
                    foreach (string v in arr)
                    {
                        string[] keyval = v.Split("=".ToCharArray());
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
            catch
            { 
            
            }

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

        public bool RemoveProperty(string propertyname)
        {
            var temp = "";
            var res = false;
            try
            {
                if (string.IsNullOrEmpty(propertyname))
                    return false;

                if (!string.IsNullOrEmpty(Properties))
                {
                    temp = this.Properties;
                    var arr = Properties.Split("#".ToCharArray());
                    this.Properties = "";
                    var sep = "";
                    foreach (var v in arr)
                    {
                        var keyval = v.Split("=".ToCharArray());
                        if (keyval[0].ToUpper() != propertyname.ToUpper())
                        {
                            this.Properties += sep + v;
                            sep = "#";
                        }
                        else
                        {
                            res = true;
                        }
                       
                    }
                }
            }
            catch {
                this.Properties = temp;
            }

            return res;
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

      


    }

    public class PropertyItem
    {

        public string Title { get; set; }

        public string PropertyCode { get; set; }

        public string PropertyValue { get; set; }

        public string PresentationValue { get; set; }

    }
}
