
﻿using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Model.UIDesign
{

    public class UserInterfaceListDesignVm : BaseModelVm
    {
        public int ApplicationId { get; set; }
        public string MetaType { get; set; }
        public string MetaCode { get; set; }
        public UITable Table { get; set; }

        public UserInterfaceListDesignVm()
        {

            Table = new UITable();
            MetaType = UserInterfaceModelItem.MetaTypeListInterface;
            Properties = "";

         
        }

    }

}