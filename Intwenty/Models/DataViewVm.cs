using Moley.Data.Dto;
using System.Collections.Generic;


namespace Moley.Models
{
    public class DataViewVm
    {
        public int Id = 0;
        public string Title = "";
        public string MetaCode = "";
        public string SQLQuery = "";
        public string Properties = "";
        public List<DataViewFieldVm> Fields = new List<DataViewFieldVm>();

        public static List<DataViewVm> GetDataViews(List<MetaDataViewDto> viewmeta)
        {
            var res = new List<DataViewVm>();

            foreach (var t in viewmeta)
            {
                if (t.IsMetaTypeDataView)
                {
                    var dv = new DataViewVm() { Id = t.Id, Properties = t.Properties, SQLQuery = t.SQLQuery, Title = t.Title, MetaCode = t.MetaCode };
                    foreach (var f in viewmeta)
                    {
                        if ((f.IsMetaTypeDataViewField || f.IsMetaTypeDataViewKeyField) && f.ParentMetaCode == t.MetaCode)
                        {
                            dv.Fields.Add(new DataViewFieldVm() { Id = f.Id, Properties = f.Properties, Title = f.Title, SQLQueryFieldName = f.SQLQueryFieldName });

                        }
                    }

                    res.Add(dv);
                }
            }

            return res;
        }
    }

    public class DataViewFieldVm
    {
        public int Id = 0;
        public string Title = "";
        public string SQLQueryFieldName = "";
        public string Properties = "";
    }

  
}
