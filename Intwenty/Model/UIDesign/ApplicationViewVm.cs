﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.UIDesign
{
    public class ApplicationViewVm :BaseModelVm
    {
        public int ApplicationId { get; set; }
        public string Title { get; set; }

        public string Path { get; set; }
    }
}