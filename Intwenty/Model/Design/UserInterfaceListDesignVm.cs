
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

        public bool IsSubTableUserInterface { get; set; }

        public List<ActionUserInterface> ActionUserInterfaces { get; set; }

        public List<FunctionVm> Functions { get; set; }

        public UserInterfaceListDesignVm()
        {
            Functions = new List<FunctionVm>();
            ActionUserInterfaces = new List<ActionUserInterface>();
            Table = new UITable();
            MetaType = UserInterfaceModelItem.MetaTypeListInterface;
            Properties = "";

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

}