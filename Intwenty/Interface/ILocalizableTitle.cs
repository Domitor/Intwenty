using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Interface
{
    interface ILocalizableTitle
    {
        string Title { get; set; }

        string TitleLocalizationKey { get; }

    }
}
