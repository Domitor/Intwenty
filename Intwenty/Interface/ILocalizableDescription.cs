using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Interface
{
    interface ILocalizableDescription
    {
        string Description { get; set; }

        string LocalizedDescription { get; set; }

        string DescriptionLocalizationKey { get; }

    }
}
