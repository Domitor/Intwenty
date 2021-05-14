
﻿using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Model.Design
{

    public class UserInterfaceListDesignVm : BaseModelVm
    {
        public int ApplicationId { get; set; }
        public string MetaType { get; set; }
        public string MetaCode { get; set; }
        public UITable Table { get; set; }
        public DatabaseTableVm DataTable { get; set; }
        public string ViewPath { get; set; }
        public bool IsSubTableUserInterface { get; set; }

        public List<ActionUserInterface> ActionUserInterfaces { get; set; }
        public List<ActionView> ActionViews { get; set; }

        public List<FunctionVm> Functions { get; set; }

        public UserInterfaceListDesignVm()
        {
            Functions = new List<FunctionVm>();
            ActionUserInterfaces = new List<ActionUserInterface>();
            ActionViews = new List<ActionView>();
            Table = new UITable();
            MetaType = UserInterfaceModelItem.MetaTypeListInterface;
            Properties = "";
            ViewPath = "";

        }

    }

    public class ActionUserInterface
    {
        public int Id { get; set; }
        public string MetaCode { get; set; }
        public string Title { get; set; }

        public static ActionUserInterface Create(UserInterfaceModelItem model)
        {
            var title = model.Description;
            return new ActionUserInterface() { Id= model.Id, MetaCode = model.MetaCode, Title = title };
        }

    }

    public class ActionView
    {
        public int Id { get; set; }
        public string MetaCode { get; set; }
        public string Title { get; set; }

        public static ActionView Create(ViewModel model)
        {
            var title = model.Title;
            return new ActionView() { Id = model.Id, MetaCode = model.MetaCode, Title = title };
        }

    }

}