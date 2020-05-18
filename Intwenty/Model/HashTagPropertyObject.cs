using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model
{
    public class HashTagPropertyObject
    {

        private List<PropertyPresentation> _presentationmap = new List<PropertyPresentation>();
        private List<PropertyPresentation> _currentpresentations = new List<PropertyPresentation>();
        private string _propertystore = string.Empty;

        public string Properties
        {
            get
            {
                return _propertystore;
            }
            set
            {
                _propertystore = value;
            }
        }

        public List<PropertyPresentation> PropertyPresentations
        {
            get
            {
                return GetPropertyList();

            }
            set
            {
                _currentpresentations = value;
                if (_currentpresentations != null) 
                {
                    _propertystore = string.Empty;
                    foreach (var p in _currentpresentations) 
                    {
                        AddUpdateProperty(p.PropertyCode, p.PropertyValue);
                    }
                } 
            }
        }


        public void SetPropertyPresentationMap(List<PropertyPresentation> map) 
        {
            _presentationmap = map;
        }

        public void AddUpdateProperty(string key, string value)
        {
            var res = string.Empty;

            try
            {

                if (!string.IsNullOrEmpty(_propertystore))
                {
                    var arr = _propertystore.Split("#".ToCharArray());
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

                _propertystore = res;
            }
            catch { }

        }

        protected virtual List<PropertyPresentation> PropertyPresentationMap
        {
            get
            {
                return _presentationmap;

            }
        }

        public string GetPropertyValue(string propertyname)
        {
            if (string.IsNullOrEmpty(_propertystore))
                return string.Empty;

            if (string.IsNullOrEmpty(propertyname))
                return string.Empty;

            var arr = _propertystore.Split("#".ToCharArray());

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

                if (!string.IsNullOrEmpty(_propertystore))
                {
                    var arr = _propertystore.Split("#".ToCharArray());
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
                if (string.IsNullOrEmpty(_propertystore))
                    return res;

                var arr = _propertystore.Split("#".ToCharArray());

                foreach (var v in arr)
                {
                    var keyval = v.Split("=".ToCharArray());
                    res.Add(keyval[0].ToUpper());
                }

            }
            catch { }

            return res;
        }

        private List<PropertyPresentation> GetPropertyList()
        {
            _currentpresentations = new List<PropertyPresentation>();

            if (string.IsNullOrEmpty(_propertystore))
                return new List<PropertyPresentation>();

            var arr = _propertystore.Split("#".ToCharArray());

            foreach (var v in arr)
            {
                var keyval = v.Split("=".ToCharArray());
                if (keyval.Length == 2)
                {
                    var prop = _presentationmap.Find(p => p.PropertyCode == keyval[0].ToUpper());
                    if (prop != null)
                    {
                        var t = new PropertyPresentation() { PropertyCode = keyval[0].ToUpper(), PropertyValue = keyval[1].ToUpper(), Title = prop.Title, PresentationValue = prop.PresentationValue };
                        if (string.IsNullOrEmpty(t.PresentationValue))
                            t.PresentationValue = t.PropertyValue;
                        _currentpresentations.Add(t);

                    }
                    else
                    {
                        var t = new PropertyPresentation() { PropertyCode = keyval[0].ToUpper(), PropertyValue = keyval[1].ToUpper(), Title = keyval[0].ToUpper(), PresentationValue = keyval[1].ToUpper() };
                        if (string.IsNullOrEmpty(t.PresentationValue))
                            t.PresentationValue = t.PropertyValue;
                        _currentpresentations.Add(t);

                    }


                }
            }

            return _currentpresentations;

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
