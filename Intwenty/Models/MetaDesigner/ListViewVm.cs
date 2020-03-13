using Intwenty.MetaDataService.Model;
using System.Collections.Generic;


namespace Intwenty.Models.MetaDesigner
{
    public static class MetaDataListViewCreator
    {
        public static List<UserInterfaceModelItem> GetMetaUIListView(ListViewVm model, ApplicationModel app)
        {
            var res = new List<UserInterfaceModelItem>();
            var t = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypeListView) { Title = model.Title, MetaCode = model.MetaCode, ParentMetaCode = "ROOT", Id = model.Id, AppMetaCode = app.Application.MetaCode  };
            if (string.IsNullOrEmpty(model.MetaCode))
                t.MetaCode = BaseModelItem.GenerateNewMetaCode(t);

            res.Add(t);

           
            foreach (var f in model.Fields)
            {
                var lf = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypeListViewField) { Title = f.Title, MetaCode = "", ParentMetaCode = t.MetaCode, Id = f.Id, AppMetaCode = app.Application.MetaCode };
                if (string.IsNullOrEmpty(lf.MetaCode))
                    lf.MetaCode = BaseModelItem.GenerateNewMetaCode(lf);

                if (!string.IsNullOrEmpty(f.DbName))
                {
                    var dmc = app.DataStructure.Find(p => p.DbName == f.DbName && p.IsRoot);
                    if (dmc != null)
                        lf.DataMetaCode = dmc.MetaCode;
                }

                res.Add(lf);
            }

            return res;
        }
    }

    public class ListViewVm
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string Title { get; set; }
        public string MetaCode { get; set; }
        public string Properties { get; set; }
        public List<ListViewFieldVm> Fields = new List<ListViewFieldVm>();

        public ListViewVm()
        {
            Title = "";
            MetaCode = "";
            Properties = "";
            Fields = new List<ListViewFieldVm>();
        }

        public static ListViewVm GetListView(ApplicationModel app)
        {
            var res = new ListViewVm();
            res.ApplicationId = app.Application.Id;

            foreach (var t in app.UIStructure)
            {
                if (t.IsMetaTypeListView)
                {
                    res.Id = t.Id;
                    res.Title = t.Title;
                    res.MetaCode = t.MetaCode;

                    foreach (var f in app.UIStructure)
                    {
                        if (f.IsMetaTypeListViewField && f.ParentMetaCode == t.MetaCode)
                        {
                            if (f.IsDataConnected)
                                res.Fields.Add(new ListViewFieldVm() { Id = f.Id, Properties = f.Properties, Title = f.Title, DbName = f.DataInfo.DbName });
                            else
                                res.Fields.Add(new ListViewFieldVm() { Id = f.Id, Properties = f.Properties, Title = f.Title});
                        }
                    }
                }
            }

            return res;
        }
    }

    public class ListViewFieldVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string DbName { get; set; }
        public string Properties { get; set; }

        public ListViewFieldVm()
        {
            Title = "";
            DbName = "";
            Properties = "";
        }
    }

  
}
