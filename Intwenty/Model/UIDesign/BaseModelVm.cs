using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.UIDesign
{
    public class BaseModelVm : HashTagPropertyObject
    {
        public int Id { get; set; }

        public IntwentyProperty CurrentProperty { get; set; }

        public BaseModelVm()
        {
            CurrentProperty = new IntwentyProperty();
        }


    }
}
