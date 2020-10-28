using Intwenty.Model;
using System.Collections.Generic;


namespace Intwenty.Model.DesignerVM
{
    public static class DataViewModelCreator
    {
        public static List<DataViewModelItem> GetDataViewModel(DataViewVm model)
        {
            var res = new List<DataViewModelItem>();
            var t = new DataViewModelItem(DataViewModelItem.MetaTypeDataView) { Id = model.Id, SQLQuery = model.SQLQuery, Title = model.Title, MetaCode = model.MetaCode, ParentMetaCode = "ROOT"  };
            if (string.IsNullOrEmpty(model.MetaCode))
                t.MetaCode = BaseModelItem.GenerateNewMetaCode(t);

            res.Add(t);

            var cnt = 0;
            foreach (var f in model.Fields)
            {
                cnt += 1;
                if (cnt == 1)
                {
                    var kf = new DataViewModelItem(DataViewModelItem.MetaTypeDataViewKeyColumn) { Id = f.Id, SQLQueryFieldName = f.SQLQueryFieldName, Title = f.Title, MetaCode = "", ParentMetaCode = t.MetaCode};
                    if (string.IsNullOrEmpty(kf.MetaCode))
                        kf.MetaCode = BaseModelItem.GenerateNewMetaCode(kf);

                    res.Add(kf);
                }
                else
                {
                    var lf = new DataViewModelItem(DataViewModelItem.MetaTypeDataViewColumn) { Id = f.Id, SQLQueryFieldName = f.SQLQueryFieldName, Title = f.Title, MetaCode = "", ParentMetaCode = t.MetaCode };
                    if (string.IsNullOrEmpty(lf.MetaCode))
                        lf.MetaCode = BaseModelItem.GenerateNewMetaCode(lf);

                    res.Add(lf);
                }

            }

            return res;
        }

        public static DataViewModelVm GetDataViewModelVm(List<DataViewModelItem> viewmeta)
        {
            var res = new DataViewModelVm();

            foreach (var t in viewmeta)
            {
                if (t.IsMetaTypeDataView)
                {
                    var dv = new DataViewVm() { Id = t.Id, Properties = t.Properties, SQLQuery = t.SQLQuery, Title = t.Title, MetaCode = t.MetaCode };
                    foreach (var f in viewmeta)
                    {
                        if ((f.IsMetaTypeDataViewColumn || f.IsMetaTypeDataViewKeyColumn) && f.ParentMetaCode == t.MetaCode)
                        {
                            dv.Fields.Add(new DataViewFieldVm() { Id = f.Id, Properties = f.Properties, Title = f.Title, SQLQueryFieldName = f.SQLQueryFieldName });

                        }
                    }

                    res.Views.Add(dv);
                }
            }

            return res;
        }
    }

    public class DataViewModelVm
    {
        public bool ModelSaved { get; set; }

        public List<DataViewVm> Views { get; set; }
        public List<IntwentyProperty> PropertyCollection { get; set; }

        public DataViewModelVm()
        {
            Views = new List<DataViewVm>();
        }


    }


    public class DataViewVm
    {
        public int Id { get; set; }
        public string Title  { get; set; }
        public string MetaCode { get; set; }
        public string SQLQuery { get; set; }
        public string Properties { get; set; }
        public List<DataViewFieldVm> Fields { get; set; }

        public DataViewVm()
        {
            Fields = new List<DataViewFieldVm>();
            Title = "";
            MetaCode = "";
            SQLQuery = "";
            Properties = "";
        }

       
    }

    public class DataViewFieldVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SQLQueryFieldName { get; set; }
        public string Properties { get; set; }

        public DataViewFieldVm()
        {
            Title = "";
            SQLQueryFieldName = "";
            Properties = "";
        }
    }

  
}
