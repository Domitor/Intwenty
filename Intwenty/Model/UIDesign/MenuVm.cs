using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Intwenty.Model.UIDesign
{
    public class MenuManagementVm
    {
        public bool modelSaved { get; set; }

        public List<ApplicationModelItem> Applications { get; set; }

        public List<SystemModelItem> Systems { get; set; }

        public List<IntwentyMetaType> MenuMetaTypes { get; set; }

        public List<MenuVm> MenuItems { get; set; }

        public MenuManagementVm()
        {
            MenuMetaTypes = new List<IntwentyMetaType>();
            Systems = new List<SystemModelItem>();
            Applications = new List<ApplicationModelItem>();
            MenuItems = new List<MenuVm>();
        }



    }

    public class MenuVm : BaseModelVm
    {
        public string MetaType { get; set; }

        public string MetaCode{ get; set; }

        public string ParentMetaCode { get; set; }

        public string Title { get; set; }

        public string SystemMetaCode { get; set; }

        public string AppMetaCode { get; set; }

        public int OrderNo { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }


        public static MenuVm CreateMenuVm(MenuModelItem model)
        {
            var t = new MenuVm();
            t.MetaType = model.MetaType;
            t.ParentMetaCode = model.ParentMetaCode;
            t.MetaCode = model.MetaCode;
            t.Title = model.Title;
            t.OrderNo = model.OrderNo;
            t.Id = model.Id;
            t.Properties = model.Properties;
            t.SystemMetaCode = model.SystemMetaCode;
            t.Action = model.Action;
            t.Controller = model.Controller;
            t.AppMetaCode = model.AppMetaCode;
            return t;
        }

        public static MenuModelItem CreateMenuModelItem(MenuVm model)
        {
            var t = new MenuModelItem(model.MetaType);
            t.OrderNo = model.OrderNo;
            t.Id = model.Id;
            t.Title = model.Title;
            t.Properties = model.Properties;
            t.SystemMetaCode = model.SystemMetaCode;
            t.Action = model.Action;
            t.Controller = model.Controller;
            t.AppMetaCode = model.AppMetaCode;
            t.ParentMetaCode = model.ParentMetaCode;
            t.MetaCode = model.MetaCode;
            return t;
        }

    }

   
}
