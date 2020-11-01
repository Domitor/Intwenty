using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Interface
{
    public interface IUIBinding
    {
        string UIId { get; }

        bool Mandatory { get; }

        string Title { get; }

        string DataTableDbName { get; }

        string DataColumnDbName { get; }

        string RawHTML { get; }

        bool ReadOnly { get; }

        bool HasProperty(string propertyname);

        bool HasPropertyWithValue(string propertyname, object value);

    }

    public interface IUIComplexBinding : IUIBinding
    {

        string DataViewName { get; }

        string DomainName { get; }

        string DataColumn2DbName { get; }

        string DataViewColumnDbName { get; }

        string DataViewColumn2DbName { get; }

       
    }
}
