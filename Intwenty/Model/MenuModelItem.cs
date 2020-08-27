using Intwenty.Data.Entity;
using Intwenty.Interface;
using System.Collections.Generic;

namespace Intwenty.Model
{
    public class MenuModelItem : BaseModelItem, ILocalizableTitle
    {

        //META TYPES
        public static readonly string MetaTypeMainMenu = "MAINMENU";
        public static readonly string MetaTypeMenuItem = "MENUITEM";

        public MenuModelItem()
        {
            MetaType = MetaTypeMenuItem;
        }

        public MenuModelItem(string metatype)
        {
            MetaType = metatype;
        }

        public MenuModelItem(MenuItem entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            TitleLocalizationKey = entity.TitleLocalizationKey;
            MetaType = entity.MetaType;
            MetaCode = entity.MetaCode;
            ParentMetaCode = entity.ParentMetaCode;
            Controller = entity.Controller;
            Action = entity.Action;
            Properties = entity.Properties;
            OrderNo = entity.OrderNo;
            AppMetaCode = entity.AppMetaCode;
            
        }


        public ApplicationModelItem Application { get; set; }

        public string AppMetaCode { get; set; }

        public string TitleLocalizationKey { get; set; }

        public int OrderNo { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public bool IsMetaTypeMainMenu
        {
            get { return MetaType == MetaTypeMainMenu; }
        }

        public bool IsMetaTypeMenuItem
        {
            get { return MetaType == MetaTypeMenuItem; }
        }

        public override bool HasValidMetaType
        {
            get
            {
                if (string.IsNullOrEmpty(MetaType))
                    return false;

                if (ValidMetaTypes.Contains(MetaType))
                    return true;

                return false;

            }
        }

      

        public static List<string> ValidProperties
        {
            get
            {
                var t = new List<string>();
                return t;
            }
        }

        public static List<string> ValidMetaTypes
        {
            get
            {
                var t = new List<string>();
                t.Add(MetaTypeMainMenu);
                t.Add(MetaTypeMenuItem);
                return t;
            }
        }

    }

 
}
