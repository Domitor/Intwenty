using Intwenty.Interface;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace Intwenty.Model
{
    public abstract class BaseModelItem : HashTagPropertyObject, IRequestInfo
    {
        public int Id { get; set; }

        public static readonly string MetaTypeRoot = "ROOT";

        public string Title { get; set; }

        public string LocalizedTitle { get; set; }

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public string MetaType { set;  get; }

        public abstract bool HasValidMetaType { get; }

        public abstract bool HasValidProperties { get; }

        public abstract string ModelCode { get; }

        public ViewRequestInfo RuntimeRequestInfo { set; get; }




        public bool IsRoot
        {
            get
            {
                return ParentMetaCode == "" || ParentMetaCode == "ROOT";
            }
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
