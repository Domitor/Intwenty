using Intwenty.MetaDataService.Model;
using System.Collections.Generic;


namespace Intwenty.Models.MetaDesigner
{
    public static class MataDataViewCreator
    {
        public static List<DataViewModelItem> GetMetaDataView(DataViewVm model)
        {
            var res = new List<DataViewModelItem>();
            var t = new DataViewModelItem("DATAVIEW") { SQLQuery = model.SQLQuery, Title = model.Title, MetaCode = model.MetaCode, ParentMetaCode = "ROOT"  };
            if (string.IsNullOrEmpty(model.MetaCode))
                t.MetaCode = "DV_" + BaseModelItem.GetRandomUniqueString();

            res.Add(t);

            var cnt = 0;
            foreach (var f in model.Fields)
            {
                cnt += 1;
                if (cnt == 1)
                {
                    var kf = new DataViewModelItem("DATAVIEWKEYFIELD") { SQLQueryFieldName = f.SQLQueryFieldName, Title = f.Title, MetaCode = "", ParentMetaCode = t.MetaCode};
                    if (string.IsNullOrEmpty(kf.MetaCode))
                        kf.MetaCode = "DVKF_" + BaseModelItem.GetRandomUniqueString();

                    res.Add(kf);
                }
                else
                {
                    var lf = new DataViewModelItem("DATAVIEWFIELD") { SQLQueryFieldName = f.SQLQueryFieldName, Title = f.Title, MetaCode = "", ParentMetaCode = t.MetaCode };
                    if (string.IsNullOrEmpty(lf.MetaCode))
                        lf.MetaCode = "DVKF_" + BaseModelItem.GetRandomUniqueString();

                    res.Add(lf);
                }

            }

            return res;
        }
    }

    public class DataViewVm
    {
        public int Id = 0;
        public string Title = "";
        public string MetaCode = "";
        public string SQLQuery = "";
        public string Properties = "";
        public List<DataViewFieldVm> Fields = new List<DataViewFieldVm>();

        public static List<DataViewVm> GetDataViews(List<DataViewModelItem> viewmeta)
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
