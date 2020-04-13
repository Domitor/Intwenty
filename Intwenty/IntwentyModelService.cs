using System;
using System.Collections.Generic;
using System.Linq;
using Intwenty.Data.Entity;
using Intwenty.Model;
using Microsoft.Extensions.Caching.Memory;
using Intwenty.Data;
using Shared;
using Microsoft.Extensions.Options;
using Intwenty.Data.DBAccess;

namespace Intwenty
{
    /// <summary>
    /// Interface for operations on meta data
    /// </summary>
    public interface IIntwentyModelService
    {
 
        List<ApplicationModel> GetApplicationModels();



        List<ApplicationModelItem> GetApplicationModelItems();

        ApplicationModelItem SaveApplication(ApplicationModelItem model);

        void DeleteApplicationModel(ApplicationModelItem model);



        List<DatabaseModelItem> GetDatabaseModelItems();

        void SaveApplicationDB(List<DatabaseModelItem> model, int applicationid);

        void DeleteApplicationDB(int id);
       



        List<UserInterfaceModelItem> GetUserInterfaceModelItems();

        void SaveUserInterfaceModel(List<UserInterfaceModelItem> model);

        


        List<DataViewModelItem> GetDataViewModels();

        void SaveDataView(List<DataViewModelItem> model);

        void DeleteDataView(int id);



        List<ValueDomainModelItem> GetValueDomains();
        void SaveValueDomains(List<ValueDomainModelItem> model);

        void DeleteValueDomain(int id);




        List<MenuModelItem> GetMenuModelItems();

        List<NoSerieModelItem> GetNewNoSeriesValues(int applicationid);

        List<NoSerieModelItem> GetNoSeries();

        List<string> GetTestDataBatches();

        void DeleteTestDataBatch(string batchname);

    }




    public class IntwentyModelService : IIntwentyModelService
    {

        private IIntwentySqlDbAccessService SqlClient { get; }

        private IMemoryCache ModelCache { get; }

        private SystemSettings Settings { get; }

        private ConnectionStrings DbConnections { get; }

        private static readonly string AppModelCacheKey = "APPMODELS";

        public IntwentyModelService(IOptions<SystemSettings> settings, IOptions<ConnectionStrings> connections, IMemoryCache cache)
        {
            ModelCache = cache;
            Settings = settings.Value;
            DbConnections = connections.Value;
            if (!Settings.IntwentyDBMSIsNoSQL)
            {
                this.SqlClient = new IntwentySqlDbClient();
            }
           
        }



        public List<ApplicationModel> GetApplicationModels()
        {
            List<ApplicationModel> res = null;

            if (ModelCache.TryGetValue(AppModelCacheKey, out res))
            {
                 return res;
            }

            res = new List<ApplicationModel>();
            var apps =  GetApplicationModelItems();
            var ditems = SqlClient.Get<DatabaseItem>().Select(p => new DatabaseModelItem(p)).ToList();
            var uitems = SqlClient.Get<UserInterfaceItem>().Select(p => new UserInterfaceModelItem(p)).ToList();
            var views = SqlClient.Get<DataViewItem>().Select(p => new DataViewModelItem(p)).ToList();
         

            foreach (var app in apps)
            {
                var t = new ApplicationModel();
                t.Application = app;
                t.DataStructure = new List<DatabaseModelItem>();
                t.UIStructure = new List<UserInterfaceModelItem>();
                t.NoSeries = new List<NoSerieModelItem>();

                foreach (var item in ditems)
                {
                    if (item.AppMetaCode == app.MetaCode)
                    {
                        t.DataStructure.Add(item);
                        if (item.IsMetaTypeDataColumn)
                        {
                            item.ColumnName = item.DbName;
                            if (item.IsRoot)
                                item.TableName = app.DbName;
                            else
                            {
                                var table = ditems.Find(p => p.MetaCode == item.ParentMetaCode && p.IsMetaTypeDataTable && p.AppMetaCode == app.MetaCode);
                                if (table != null)
                                    item.TableName = table.DbName;
                            }
                        }
                        if (item.IsMetaTypeDataTable)
                        {
                            item.TableName = item.DbName;
                        }
                    }
                }


                foreach (var item in uitems.OrderBy(p=> p.RowOrder).ThenBy(p=> p.ColumnOrder))
                {
                    if (item.AppMetaCode == app.MetaCode)
                    {
                        t.UIStructure.Add(item);

                        if (!string.IsNullOrEmpty(item.DataMetaCode))
                        {
                            var dinf = ditems.Find(p => p.MetaCode == item.DataMetaCode && p.AppMetaCode == app.MetaCode);
                            if (dinf != null && dinf.IsMetaTypeDataColumn)
                                item.DataColumnInfo = dinf;
                            else if (dinf != null && dinf.IsMetaTypeDataTable)
                                item.DataTableInfo = dinf;

                            if (item.DataColumnInfo != null && item.DataTableInfo == null)
                            {
                                if (!item.DataColumnInfo.IsRoot)
                                {
                                    dinf = ditems.Find(p => p.MetaCode == item.DataColumnInfo.ParentMetaCode && p.AppMetaCode == app.MetaCode);
                                    if (dinf != null && dinf.IsMetaTypeDataTable)
                                        item.DataTableInfo = dinf;
                                }
                                else
                                {
                                    item.DataTableInfo = new DatabaseModelItem(DatabaseModelItem.MetaTypeDataTable) { AppMetaCode = app.MetaCode, Id=0, DbName = app.DbName, TableName = app.DbName, MetaCode= app.MetaCode, ParentMetaCode = "ROOT", Title = app.DbName   };

                                }
                            }

                        }

                        if (!string.IsNullOrEmpty(item.DataMetaCode2))
                        {
                            var dinf = ditems.Find(p => p.MetaCode == item.DataMetaCode2 && p.AppMetaCode == app.MetaCode);
                            if (dinf != null && dinf.IsMetaTypeDataColumn)
                                item.DataColumnInfo2 = dinf;
                        }

                        if (item.HasDataViewDomain)
                        {
                            var vinf = views.Find(p => p.MetaCode == item.ViewName && p.IsRoot);
                            if (vinf != null)
                                item.DataViewInfo = vinf;

                            if (!string.IsNullOrEmpty(item.ViewMetaCode))
                            {
                                vinf = views.Find(p => p.MetaCode == item.ViewMetaCode && !p.IsRoot);
                                if (vinf != null)
                                    item.DataViewColumnInfo = vinf;
                            }
                            if (!string.IsNullOrEmpty(item.ViewMetaCode2))
                            {
                                vinf = views.Find(p => p.MetaCode == item.ViewMetaCode2 && !p.IsRoot);
                                if (vinf != null)
                                    item.DataViewColumnInfo2 = vinf;
                            }
                        }

                       
                    }
                }

                res.Add(t);
            }

            ModelCache.Set(AppModelCacheKey, res);
           
            return res;

        }

       

        public List<MenuModelItem> GetMenuModelItems()
        {
    
            var apps = SqlClient.Get<ApplicationItem>().Select(p => new ApplicationModelItem(p)).ToList();
            var menu = SqlClient.Get<MenuItem>();
         

            var res = new List<MenuModelItem>();
            foreach (var m in menu.OrderBy(p=> p.OrderNo))
            {
                var s = new MenuModelItem(m);

                //DEFAULTS
                if (!string.IsNullOrEmpty(m.AppMetaCode) && s.IsMetaTypeMenuItem)
                {
                    if (string.IsNullOrEmpty(s.Controller))
                        s.Controller = "Application";

                    if (string.IsNullOrEmpty(s.Action))
                        s.Action = "GetList";

                    if (!string.IsNullOrEmpty(m.AppMetaCode))
                    {
                        var app = apps.Find(p => p.MetaCode == m.AppMetaCode);
                        if (app != null)
                            s.Application = app;
                    }
                }

                res.Add(s);
            }

            return res;
        }

        public List<NoSerieModelItem> GetNewNoSeriesValues(int applicationid)
        {
            var res = new List<NoSerieModelItem>();

            /*
            var app = SqlClient.Get<ApplicationItem>().FirstOrDefault(p => p.Id == applicationid);
            if (app == null)
                return new List<NoSerieModelItem>();

            var appseries = new List<NoSerieModelItem>();
            foreach (var n in NoSeries)
            {
                if (n.AppMetaCode == app.MetaCode)
                {
                    appseries.Add(new NoSerieModelItem(n));
                }
            }

            foreach (var item in appseries)
            {
                item.Application = new ApplicationModelItem(app);

                if (item.Counter < 1)
                    item.Counter = item.StartValue;

                item.Counter += 1;

                var t = NoSeries.FirstOrDefault(p => p.MetaCode == item.MetaCode);
                if (t == null)
                    continue;

                t.Counter = item.Counter;

                item.NewValue = item.Prefix + Convert.ToString(item.Counter); 

                res.Add(item);

            }

            SqlClient.SaveChanges();

    */


            return res;

        }

        public List<NoSerieModelItem> GetNoSeries()
        {
            var res = new List<NoSerieModelItem>();
            /*
            var res = NoSeries.Select(p => new NoSerieModelItem(p)).ToList();
            var ditems = MetaDBItem.Select(p => new DatabaseModelItem(p)).ToList();

            foreach (var item in res)
            {



                if (!string.IsNullOrEmpty(item.AppMetaCode))
                {
                    var app = AppDescription.FirstOrDefault(p => p.MetaCode == item.AppMetaCode);
                    if (app != null)
                    {
                        item.Application = new ApplicationModelItem(app);
                        if (!string.IsNullOrEmpty(item.DataMetaCode))
                        {
                            var dinf = ditems.Find(p => p.MetaCode == item.DataMetaCode && p.AppMetaCode == app.MetaCode);
                            if (dinf != null)
                                item.DataInfo = dinf;
                        }
                    }
                }

            }
            */
            return res;
        }

        #region Application
        public List<ApplicationModelItem> GetApplicationModelItems()
        {
           
            var t = SqlClient.Get<ApplicationItem>().Select(p => new ApplicationModelItem(p)).ToList();
            var menu = GetMenuModelItems();
            foreach (var app in t)
            {
                var mi = menu.Find(p => p.IsMetaTypeMenuItem && p.Application.Id == app.Id);
                if (mi != null)
                    app.MainMenuItem = mi;
            }

            return t;
        }

        public void DeleteApplicationModel(ApplicationModelItem model)
        {
           
            if (model==null)
                throw new InvalidOperationException("Missing required information when deleting application model.");

            if (model.Id < 1 || string.IsNullOrEmpty(model.MetaCode))
                throw new InvalidOperationException("Missing required information when deleting application model.");

            var existing = SqlClient.Get<ApplicationItem>().FirstOrDefault(p => p.Id == model.Id);
            if (existing == null)
                return; //throw new InvalidOperationException("Could not find application model when deleting application model.");


            var dbitems = SqlClient.Get<DatabaseItem>().Where(p => p.AppMetaCode == existing.MetaCode);
            if (dbitems != null && dbitems.Count() > 0)
                SqlClient.DeleteRange(dbitems);

            var uiitems = SqlClient.Get<UserInterfaceItem>().Where(p => p.AppMetaCode == existing.MetaCode);
            if (uiitems != null && uiitems.Count() > 0)
                SqlClient.DeleteRange(uiitems);

            var menuitems = SqlClient.Get<MenuItem>().Where(p => p.AppMetaCode == existing.MetaCode);
            if (menuitems != null && menuitems.Count() > 0)
                SqlClient.DeleteRange(menuitems);

            SqlClient.Delete(existing);


            ModelCache.Remove(AppModelCacheKey);

        }

        public ApplicationModelItem SaveApplication(ApplicationModelItem model)
        {
            ModelCache.Remove(AppModelCacheKey);

            var AppDescription = SqlClient.Get<ApplicationItem>();



            if (model == null)
                return null;

            if (model.Id < 1)
            {
                var max = 10;
                if (AppDescription.Count() > 0)
                {
                    max = AppDescription.Max(p => p.Id);
                    max += 10;
                }

                var entity = new ApplicationItem();
                entity.Id = max;
                entity.MetaCode = model.MetaCode;
                entity.Title = model.Title;
                entity.DbName = model.DbName;
                entity.Description = model.Description;

                SqlClient.Insert(entity);

                CreateApplicationMenuItem(model);

                return new ApplicationModelItem(entity);

            }
            else
            {

                var entity = AppDescription.FirstOrDefault(p => p.Id == model.Id);
                if (entity == null)
                    return model;

                entity.MetaCode = model.MetaCode;
                entity.Title = model.Title;
                entity.DbName = model.DbName;
                entity.Description = model.Description;

                var menuitem = SqlClient.Get<MenuItem>().FirstOrDefault(p => p.AppMetaCode == entity.MetaCode && p.MetaType == MenuModelItem.MetaTypeMenuItem);
                if (menuitem != null)
                {
                    menuitem.Action = model.MainMenuItem.Action;
                    menuitem.Controller = model.MainMenuItem.Controller;
                    menuitem.Title = model.MainMenuItem.Title;
          
                    SqlClient.Update(menuitem);
                
                }
                else
                {
                    CreateApplicationMenuItem(model);
                }

                return new ApplicationModelItem(entity);

            }

        }

        private void CreateApplicationMenuItem(ApplicationModelItem model)
        {

            if (!string.IsNullOrEmpty(model.MainMenuItem.Title))
            {
                var max = 0;
                var menu = GetMenuModelItems();
                var root = menu.Find(p => p.IsMetaTypeMainMenu);
                if (root == null)
                {
                    var main = new MenuItem() { AppMetaCode = "", MetaType = "MAINMENU", OrderNo = 1, ParentMetaCode = "ROOT", Properties = "", Title = "Applications" };
                    SqlClient.Insert(main);
                    max = 1;
                }
                else
                {
                    max = menu.Max(p => p.Order);
                }

                var appmi = new MenuItem() { AppMetaCode = model.MetaCode, MetaType = MenuModelItem.MetaTypeMenuItem, OrderNo = max + 10, ParentMetaCode = root.MetaCode, Properties = "", Title = model.MainMenuItem.Title };
                appmi.Action = model.MainMenuItem.Action;
                appmi.Controller = model.MainMenuItem.Controller;
                appmi.MetaCode = BaseModelItem.GenerateNewMetaCode(model.MainMenuItem);
                SqlClient.Insert(appmi);

            }

        }
        #endregion

        #region UI
        public List<UserInterfaceModelItem> GetUserInterfaceModelItems()
        {
            var res = new List<UserInterfaceModelItem>();

            var models = GetApplicationModels();

            foreach (var m in models)
            {
                res.AddRange(m.UIStructure);
            }

            return res;
        }

        public void SaveUserInterfaceModel(List<UserInterfaceModelItem> model)
        {
            ModelCache.Remove(AppModelCacheKey);

            foreach (var t in model)
            {
                if (t.Id > 0 && t.HasProperty("REMOVED"))
                {
                    var existing = SqlClient.Get<UserInterfaceItem>().FirstOrDefault(p => p.Id == t.Id);
                    if (existing != null)
                    {
                        SqlClient.Delete(existing);
                    }
                }
            }

            foreach (var uic in model)
            {
                if (uic.HasProperty("REMOVED"))
                    continue;

                if (uic.Id < 1)
                {
                    if (string.IsNullOrEmpty(uic.MetaType))
                        throw new InvalidOperationException("Can't save an ui model item without a MetaType");

                    if (string.IsNullOrEmpty(uic.ParentMetaCode))
                        throw new InvalidOperationException("Can't save an ui model item of type " + uic.MetaType + " without a ParentMetaCode");

                    if (string.IsNullOrEmpty(uic.MetaCode))
                        throw new InvalidOperationException("Can't save an ui model item of type " + uic.MetaType + " without a MetaCode");

                    SqlClient.Insert(CreateMetaUIItem(uic));

                }
                else
                {
                    var existing = SqlClient.Get<UserInterfaceItem>().FirstOrDefault(p => p.Id == uic.Id);
                    if (existing != null)
                    {
                        existing.Title = uic.Title;
                        existing.RowOrder = uic.RowOrder;
                        existing.ColumnOrder = uic.ColumnOrder;
                        existing.DataMetaCode = uic.DataMetaCode;
                        existing.DataMetaCode2 = uic.DataMetaCode2;
                        existing.ViewMetaCode = uic.ViewMetaCode;
                        existing.ViewMetaCode2 = uic.ViewMetaCode2;
                        existing.Domain = uic.Domain;
                        existing.Description = uic.Description;
                        existing.Properties = uic.Properties;
                        SqlClient.Update(existing);
                    }

                }

            }

           
        }

      

       


        private UserInterfaceItem CreateMetaUIItem(UserInterfaceModelItem dto)
        {
            var res = new UserInterfaceItem()
            {
                AppMetaCode = dto.AppMetaCode,
                ColumnOrder = dto.ColumnOrder,
                DataMetaCode = dto.DataMetaCode,
                DataMetaCode2 = dto.DataMetaCode2,
                ViewMetaCode = dto.ViewMetaCode,
                ViewMetaCode2 = dto.ViewMetaCode2,
                Description = dto.Description,
                Domain = dto.Domain,
                MetaCode = dto.MetaCode,
                MetaType = dto.MetaType,
                ParentMetaCode = dto.ParentMetaCode,
                RowOrder = dto.RowOrder,
                Title = dto.Title,
                Properties = dto.Properties
                
            };

            return res;

        }
        #endregion

        #region Database

        public List<DatabaseModelItem> GetDatabaseModelItems()
        {
            return SqlClient.Get<DatabaseItem>().Select(p => new DatabaseModelItem(p)).ToList();
        }

        public void SaveApplicationDB(List<DatabaseModelItem> model, int applicationid)
        {
          

            var app = GetApplicationModels().Find(p => p.Application.Id == applicationid);
            if (app == null)
                throw new InvalidOperationException("Could not find application when saving application database model.");

            foreach (var dbi in model)
            {
                dbi.AppMetaCode = app.Application.MetaCode;

                //ASSUME ALL IS ROOT, CORRECT LATER
                dbi.ParentMetaCode = "ROOT";

                if (dbi.IsMetaTypeDataColumn && string.IsNullOrEmpty(dbi.TableName))
                    throw new InvalidOperationException("Could not identify parent table when saving application database model.");

                if (dbi.IsMetaTypeDataColumn && dbi.TableName == app.Application.DbName)
                {
                    dbi.ParentMetaCode = "ROOT";
                }

                if (!string.IsNullOrEmpty(dbi.DbName))
                    dbi.DbName = dbi.DbName.Replace(" ", "");

                if (!dbi.HasValidMetaType)
                    throw new InvalidOperationException("Invalid meta type when saving application database model.");

                if (dbi.IsMetaTypeDataColumn && !dbi.HasValidDataType)
                    throw new InvalidOperationException("Invalid datatype type when saving application database model.");


            }

            foreach (var dbi in model)
            {
                if (dbi.Id < 1)
                {
                    if (string.IsNullOrEmpty(dbi.MetaCode))
                        dbi.MetaCode = BaseModelItem.GenerateNewMetaCode(dbi);

                    var t = CreateMetaDataItem(dbi);


                    //SET PARENT META CODE
                    if (dbi.IsMetaTypeDataColumn && dbi.TableName != app.Application.DbName)
                    {
                        var tbl = model.Find(p => p.IsMetaTypeDataTable && p.DbName == dbi.TableName);
                        if (tbl != null)
                            t.ParentMetaCode = tbl.MetaCode;
                    }

                    //Don't save main table, it's implicit for application
                    if (dbi.IsMetaTypeDataTable && dbi.DbName == app.Application.DbName)
                        continue;

                    SqlClient.Insert(t);

                }
                else
                {
                  

                    var existing = SqlClient.Get<DatabaseItem>().FirstOrDefault(p => p.Id == dbi.Id);
                    if (existing != null)
                    {
                        existing.DataType = dbi.DataType;
                        existing.Domain = dbi.Domain;
                        existing.MetaType = dbi.MetaType;
                        existing.Description = dbi.Description;
                        existing.ParentMetaCode = dbi.ParentMetaCode;
                        existing.DbName = dbi.DbName;
                        existing.Mandatory = dbi.Mandatory;
                        SqlClient.Update(existing);
                    }

                }

            }


            ModelCache.Remove(AppModelCacheKey);

        }

        public void DeleteApplicationDB(int id)
        {
           

            var existing = SqlClient.Get<DatabaseItem>().FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                var dto = new DatabaseModelItem(existing);
                var app = GetApplicationModels().Find(p => p.Application.MetaCode == dto.AppMetaCode);
                if (app == null)
                    return;

                if (dto.IsMetaTypeDataTable && dto.DbName != app.Application.DbName)
                {
                    var childlist = SqlClient.Get<DatabaseItem>().Where(p => (p.MetaType == "DATACOLUMN") && p.ParentMetaCode == existing.MetaCode).ToList();
                    SqlClient.Delete(existing);
                    SqlClient.DeleteRange(childlist);
                }
                else
                {
                    SqlClient.Delete(existing);
                }

                ModelCache.Remove(AppModelCacheKey);
            }
        }

        private DatabaseItem CreateMetaDataItem(DatabaseModelItem dto)
        {
            var res = new DatabaseItem()
            {
                AppMetaCode = dto.AppMetaCode,
                Description = dto.Description,
                Domain = dto.Domain,
                MetaCode = dto.MetaCode,
                MetaType = dto.MetaType,
                ParentMetaCode = dto.ParentMetaCode,
                DbName = dto.DbName,
                DataType = dto.DataType,
                Mandatory = dto.Mandatory
            };


            return res;

        }
        #endregion

        #region Data Views
        public List<DataViewModelItem> GetDataViewModels()
        {
            return SqlClient.Get<DataViewItem>().Select(p => new DataViewModelItem(p)).ToList();
        }

        public void SaveDataView(List<DataViewModelItem> model)
        {
            foreach (var dv in model)
            {

                if (dv.IsMetaTypeDataView)
                    dv.ParentMetaCode = "ROOT";

                if (string.IsNullOrEmpty(dv.MetaCode))
                    dv.MetaCode = BaseModelItem.GenerateNewMetaCode(dv);

            }

            foreach (var dv in model)
            {
                if (dv.Id < 1)
                {
                    var t = CreateMetaDataView(dv);
                    SqlClient.Insert(t);
                }
                else
                {
                    var existing = SqlClient.Get<DataViewItem>().FirstOrDefault(p => p.Id == dv.Id);
                    if (existing != null)
                    {
                        existing.SQLQuery = dv.SQLQuery;
                        existing.SQLQueryFieldName = dv.SQLQueryFieldName;
                        existing.Title = dv.Title;
                        SqlClient.Update(existing);
                    }

                }

            }

        }


        private DataViewItem CreateMetaDataView(DataViewModelItem dto)
        {
            var res = new DataViewItem()
            {

                MetaCode = dto.MetaCode,
                MetaType = dto.MetaType,
                ParentMetaCode = dto.ParentMetaCode,
                Title = dto.Title,
                SQLQuery = dto.SQLQuery,
                SQLQueryFieldName = dto.SQLQueryFieldName

            };

            return res;

        }

      

      

        public void DeleteDataView(int id)
        {
            var existing = SqlClient.Get<DataViewItem>().FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                var dto = new DataViewModelItem(existing);
                if (dto.IsMetaTypeDataView)
                {
                    var childlist = SqlClient.Get<DataViewItem>().Where(p => (p.MetaType == "DATAVIEWFIELD" || p.MetaType == "DATAVIEWKEYFIELD") && p.ParentMetaCode == existing.MetaCode).ToList();
                    SqlClient.Delete(existing);
                    SqlClient.DeleteRange(childlist);
                }
                else
                {
                    SqlClient.Delete(existing);
                }
            }
        }
        #endregion

        #region Value Domains

        public void SaveValueDomains(List<ValueDomainModelItem> model)
        {

            foreach (var vd in model)
            {
                if (!vd.IsValid)
                    throw new InvalidOperationException("Missing required information on value domain, can't save.");

                if (vd.Id < 1)
                {
                    SqlClient.Insert(new ValueDomainItem() { DomainName = vd.DomainName, Value = vd.Value, Code = vd.Code });
                }
                else
                {
                    var existing = SqlClient.Get<ValueDomainItem>().FirstOrDefault(p => p.Id == vd.Id);
                    if (existing != null)
                    {
                        existing.Code = vd.Code;
                        existing.Value = vd.Value;
                        existing.DomainName = vd.DomainName;
                        SqlClient.Update(existing);
                    }

                }

            }
        }

        public List<ValueDomainModelItem> GetValueDomains()
        {
            var t = SqlClient.Get<ValueDomainItem>().Select(p => new ValueDomainModelItem(p)).ToList();
            return t;
        }

        public void DeleteValueDomain(int id)
        {
            var existing = SqlClient.Get<ValueDomainItem>().FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                SqlClient.Delete(existing);
            }
        }





        #endregion

        #region misc

        public List<string> GetTestDataBatches()
        {
            var filtered = SqlClient.Get<SystemID>().Where(p => !string.IsNullOrEmpty(p.Properties)).Select(p => new TestDataBatch(p)).ToList();

            var res = new List<string>();
            foreach (var s in filtered)
            {
                if (string.IsNullOrEmpty(s.BatchName))
                    continue;

                if (!res.Contains(s.BatchName))
                    res.Add(s.BatchName);
            }

            return res;

        }

        public void DeleteTestDataBatch(string batchname)
        {
            var filtered = SqlClient.Get<SystemID>().Where(p => !string.IsNullOrEmpty(p.Properties)).Select(p => new TestDataBatch(p)).ToList();

            SqlClient.Open();
            foreach (var t in filtered)
            {
                if (string.IsNullOrEmpty(t.BatchName))
                    continue;

                if (t.BatchName == batchname)
                {
                    if (t.MetaType == "APPLICATION")
                    {
                        var app = GetApplicationModelItems().Find(p => p.MetaCode == t.MetaCode);
                        if (app != null && t.Id > 0)
                        {
                            SqlClient.CreateCommand("delete from " + app.DbName + " where ID = " + t.Id);
                            SqlClient.ExecuteNonQuery();

                            SqlClient.CreateCommand("delete from " + app.VersioningTableName + " where ID = " + t.Id);
                            SqlClient.ExecuteNonQuery();

                            SqlClient.CreateCommand("delete from sysdata_InformationStatus where ID = " + t.Id);
                            SqlClient.ExecuteNonQuery();

                            SqlClient.CreateCommand("delete from sysdata_SystemID where ID = " + t.Id);
                            SqlClient.ExecuteNonQuery();

                        }
                    }
                }

            }
            SqlClient.Close();

         

        }

        #endregion


    }

}
