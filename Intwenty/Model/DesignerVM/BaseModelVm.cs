using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.DesignerVM
{
    public class BaseModelVm : HashTagPropertyObject
    {

        public IntwentyPropertyVm CurrentProperty { get; set; }

        public BaseModelVm()
        {
            CurrentProperty = new IntwentyPropertyVm();
        }


    }
}
