using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Intwenty.Model
{
    public class HashTagPropertyObject
    {

        public HashTagPropertyObject()
        {
            PropertyPresentations = new List<PropertyPresentation>();
        }

        public string Properties { get; set; }

        public List<PropertyPresentation> PropertyPresentations { get; set; }


        public void SetPresentationsFromPropertyString()
        {
            PropertyPresentations = new List<PropertyPresentation>();

            if (string.IsNullOrEmpty(Properties))
                return;

            var arr = Properties.Split("#".ToCharArray());

            foreach (var v in arr)
            {
                var keyval = v.Split("=".ToCharArray());
                if (keyval.Length == 2)
                {
                    var t = new PropertyPresentation() { PropertyCode = keyval[0].ToUpper(), PropertyValue = keyval[1].ToUpper(), Title = keyval[0].ToUpper(), PresentationValue = keyval[1].ToUpper() };
                    if (string.IsNullOrEmpty(t.PresentationValue))
                        t.PresentationValue = t.PropertyValue;

                    PropertyPresentations.Add(t);
                }
            }
        }

        public string GetPropertyStringFromPresentations()
        {
            if (PropertyPresentations == null)
                return string.Empty;

            if (PropertyPresentations.Count == 0)
                return string.Empty;

            var res = string.Empty;

            foreach (var t in PropertyPresentations)
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

    public class PropertyPresentation
    {

        public string Title { get; set; }

        public string PropertyCode { get; set; }

        public string PropertyValue { get; set; }

        public string PresentationValue { get; set; }

    }
}
