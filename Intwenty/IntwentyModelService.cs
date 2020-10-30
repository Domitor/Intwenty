using System;
using System.Collections.Generic;
using System.Linq;
using Intwenty.Entity;
using Intwenty.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Intwenty.Model.Dto;
using Intwenty.Areas.Identity.Models;
using Microsoft.Extensions.Localization;
using Intwenty.Localization;
using Intwenty.Interface;
using Microsoft.AspNetCore.Identity;
using Intwenty.DataClient;
using Intwenty.DataClient.Model;

namespace Intwenty
{
   

    public class IntwentyModelService : IIntwentyModelService
    {

        private IDataClient Client { get; }

        private IMemoryCache ModelCache { get; }

        private IntwentySettings Settings { get; }

        private string CurrentCulture { get; }

        private List<TypeMapItem> DataTypes { get; set; }

        private string AppModelCacheKey = "APPMODELS";

        private static readonly string DefaultVersioningTableColumnsCacheKey = "DEFVERTBLCOLS";

        private static readonly string ValueDomainsCacheKey = "VALUEDOMAINS";

        private static readonly string TranslationsCacheKey = "TRANSLATIONS";

        private static readonly string EndpointsCacheKey = "INTWENTYENDPOINTS";

        private static readonly string DataViewCacheKey = "INTWENTYDATAVIEWS";


        public IntwentyModelService(IOptions<IntwentySettings> settings, IMemoryCache cache)
        {
            ModelCache = cache;
            Settings = settings.Value;
            Client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
            DataTypes = Client.GetDbTypeMap();
            CurrentCulture = Settings.DefaultCulture;
            if (Settings.LocalizationMethod == LocalizationMethods.UserLocalization)
            {
                
                if (Settings.SupportedLanguages != null && Settings.SupportedLanguages.Count > 0)
                    CurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                else
                    CurrentCulture = Settings.DefaultCulture;
            }

            AppModelCacheKey += "-" + CurrentCulture.Replace("-", "").ToUpper();
        }

        public void ClearCache(string key = "ALL")
        {
            var clearall = false;

            if (string.IsNullOrEmpty(key))
                clearall = true;
            if (key.ToUpper()=="ALL")
                clearall = true;

            if (clearall)
            {
                ModelCache.Remove(AppModelCacheKey);
                ModelCache.Remove(DefaultVersioningTableColumnsCacheKey);
                ModelCache.Remove(ValueDomainsCacheKey);
                ModelCache.Remove(TranslationsCacheKey);
                ModelCache.Remove(EndpointsCacheKey);
                ModelCache.Remove(DataViewCacheKey);
            }
            else
            {
                ModelCache.Remove(key);
            }
          

        }


        public SystemModel GetSystemModel()
        {
            Client.Open();
            var t = new SystemModel();
            var apps = Client.GetEntities<ApplicationItem>();
            foreach (var a in apps)
                t.Applications.Add(a);
            var dbitems = Client.GetEntities<DatabaseItem>();
            foreach (var a in dbitems)
                t.DatabaseItems.Add(a);
            var viewitems = Client.GetEntities<DataViewItem>();
            foreach (var a in viewitems)
                t.DataViewItems.Add(a);
            var menuitems = Client.GetEntities<MenuItem>();
            foreach (var a in menuitems)
                t.MenuItems.Add(a);
            var uiitems = Client.GetEntities<UserInterfaceItem>();
            foreach (var a in uiitems)
                t.UserInterfaceItems.Add(a);
            var valuedomainitems = Client.GetEntities<ValueDomainItem>();
            foreach (var a in valuedomainitems)
                t.ValueDomains.Add(a);

            Client.Close();

            return t;

        }

        public OperationResult InsertSystemModel(SystemModel model)
        {
            if (model == null)
            {
                var error = new OperationResult();
                error.SetError("Import model. Model was null","The model to import was empty, check the file !");
                return error;
            }

            var result = new OperationResult();

            try
            {
                ModelCache.Remove(AppModelCacheKey);

                Client.Open();

                if (model.DeleteCurrentModel)
                {
                    Client.DeleteEntities(Client.GetEntities<ApplicationItem>());
                    Client.DeleteEntities(Client.GetEntities<DatabaseItem>());
                    Client.DeleteEntities(Client.GetEntities<DataViewItem>());
                    Client.DeleteEntities(Client.GetEntities<MenuItem>());
                    Client.DeleteEntities(Client.GetEntities<UserInterfaceItem>());
                    Client.DeleteEntities(Client.GetEntities<ValueDomainItem>());
                }

                foreach (var a in model.Applications)
                    Client.InsertEntity(a);

                foreach (var a in model.DatabaseItems)
                    Client.InsertEntity(a);

                foreach (var a in model.DataViewItems)
                    Client.InsertEntity(a);

                foreach (var a in model.MenuItems)
                    Client.InsertEntity(a);

                foreach (var a in model.UserInterfaceItems)
                    Client.InsertEntity(a);

                foreach (var a in model.ValueDomains)
                    Client.InsertEntity(a);

                result.IsSuccess = true;
                result.AddMessage(MessageCode.RESULT, "The model was imported successfully");

                Client.Close();
            }
            catch (Exception ex)
            {
                Client.Close();
                result.IsSuccess = false;
                if (!model.DeleteCurrentModel)
                    result.SetError(ex.Message, "Error importing model, this is probably due to conflict with the current model. Try to upload with the delete option.");
                else
                    result.SetError(ex.Message, "Error importing model");
            }

            return result;

        }

        public List<ApplicationModel> GetLocalizedApplicationModels()
        {
            var res = GetApplicationModels();
            foreach (var a in res) 
            {
                LocalizeTitle(a.Application);
                LocalizeTitle(a.Application.MainMenuItem);
                LocalizeTitles(a.UIStructure.ToList<ILocalizableTitle>());
            }
            return res;

        }
            
        public List<ApplicationModel> GetApplicationModels()
        {
            List<ApplicationModel> res = null;

            if (ModelCache.TryGetValue(AppModelCacheKey, out res))
            {
                 return res;
            }

            res = new List<ApplicationModel>();
            var appitems =  GetAppModels();
            var dbitems = GetDatabaseModels();
            var uiitems = GetUserInterfaceModels();


            foreach (var app in appitems)
            {
                var t = new ApplicationModel();
                t.Application = app;
                t.DataStructure = new List<DatabaseModelItem>();
                t.UIStructure = new List<UserInterfaceModelItem>();
                t.DataStructure.AddRange(dbitems.Where(p=> p.AppMetaCode== app.MetaCode));
                t.UIStructure.AddRange(uiitems.Where(p => p.AppMetaCode == app.MetaCode));
                res.Add(t);
            }

            ModelCache.Set(AppModelCacheKey, res);
           
            return res;

        }

        public List<MenuModelItem> GetLocalizedMenuModels()
        {
            Client.Open();
            var apps = Client.GetEntities<ApplicationItem>().Select(p => new ApplicationModelItem(p)).ToList();
            var menu = Client.GetEntities<MenuItem>().Select(p => new MenuModelItem(p)).ToList();
            Client.Close();

            //Localization
            LocalizeTitles(apps.ToList<ILocalizableTitle>());
            LocalizeTitles(menu.ToList<ILocalizableTitle>());


            var res = new List<MenuModelItem>();
            foreach (var m in menu)
            {

                //DEFAULTS
                if (!string.IsNullOrEmpty(m.AppMetaCode) && m.IsMetaTypeMenuItem)
                {
                    if (string.IsNullOrEmpty(m.Controller))
                        m.Controller = "Application";

                    if (string.IsNullOrEmpty(m.Action))
                        m.Action = "EditList";

                    if (!string.IsNullOrEmpty(m.AppMetaCode))
                    {
                        var app = apps.Find(p => p.MetaCode == m.AppMetaCode);
                        if (app != null)
                            m.Application = app;
                    }
                }

            }

            return menu.OrderBy(p => p.OrderNo).ToList();
        }
        public List<MenuModelItem> GetMenuModels()
        {
            Client.Open();
            var apps = Client.GetEntities<ApplicationItem>().Select(p => new ApplicationModelItem(p)).ToList();
            var menu = Client.GetEntities<MenuItem>().Select(p => new MenuModelItem(p)).ToList();
            Client.Close();

            var res = new List<MenuModelItem>();
            foreach (var m in menu)
            {

                //DEFAULTS
                if (!string.IsNullOrEmpty(m.AppMetaCode) && m.IsMetaTypeMenuItem)
                {
                    if (string.IsNullOrEmpty(m.Controller))
                        m.Controller = "Application";

                    if (string.IsNullOrEmpty(m.Action))
                        m.Action = "EditList";

                    if (!string.IsNullOrEmpty(m.AppMetaCode))
                    {
                        var app = apps.Find(p => p.MetaCode == m.AppMetaCode);
                        if (app != null)
                            m.Application = app;
                    }
                }

            }

            return menu.OrderBy(p=> p.OrderNo).ToList();
        }



        #region Application
      

        public List<ApplicationModelItem> GetAppModels()
        {

            Client.Open();
            var res = Client.GetEntities<ApplicationItem>().Select(p => new ApplicationModelItem(p)).ToList();
            Client.Close();

            var menu = GetMenuModels();
            foreach (var app in res)
            {
                var mi = menu.Find(p => p.IsMetaTypeMenuItem && p.Application.Id == app.Id);
                if (mi != null)
                    app.MainMenuItem = mi;
            }

            return res;
        }

        public void DeleteAppModel(ApplicationModelItem model)
        {
           
            if (model==null)
                throw new InvalidOperationException("Missing required information when deleting application model.");

            if (model.Id < 1 || string.IsNullOrEmpty(model.MetaCode))
                throw new InvalidOperationException("Missing required information when deleting application model.");

            var existing = Client.GetEntities<ApplicationItem>().FirstOrDefault(p => p.Id == model.Id);
            if (existing == null)
                return; //throw new InvalidOperationException("Could not find application model when deleting application model.");


         
            var dbitems = Client.GetEntities<DatabaseItem>().Where(p => p.AppMetaCode == existing.MetaCode);
            if (dbitems != null && dbitems.Count() > 0)
                Client.DeleteEntities(dbitems);

            var uiitems = Client.GetEntities<UserInterfaceItem>().Where(p => p.AppMetaCode == existing.MetaCode);
            if (uiitems != null && uiitems.Count() > 0)
                Client.DeleteEntities(uiitems);

            var menuitems = Client.GetEntities<MenuItem>().Where(p => p.AppMetaCode == existing.MetaCode);
            if (menuitems != null && menuitems.Count() > 0)
                Client.DeleteEntities(menuitems);

            Client.DeleteEntity(existing);

            Client.Close();

            ModelCache.Remove(AppModelCacheKey);

        }

        public ApplicationModelItem SaveAppModel(ApplicationModelItem model)
        {
            ModelCache.Remove(AppModelCacheKey);

            var apps = Client.GetEntities<ApplicationItem>();

            if (model == null)
                return null;

            if (model.Id < 1)
            {
                var max = 10;
                if (apps.Count > 0)
                {
                    max = apps.Max(p => p.Id);
                    max += 10;
                }

                var entity = new ApplicationItem();
                entity.Id = max;
                entity.MetaCode = model.MetaCode;
                entity.Title = model.Title;
                //entity.TitleLocalizationKey = model.TitleLocalizationKey;
                entity.DbName = model.DbName;
                entity.Description = model.Description;

                Client.InsertEntity(entity);
                Client.Close();

                CreateApplicationMenuItem(model);

                return new ApplicationModelItem(entity);

            }
            else
            {

                var entity = apps.FirstOrDefault(p => p.Id == model.Id);
                if (entity == null)
                    return model;

                //entity.MetaCode = model.MetaCode;
                entity.Title = model.Title;
                //entity.TitleLocalizationKey = model.TitleLocalizationKey;
                entity.DbName = model.DbName;
                entity.Description = model.Description;

                var menuitem = Client.GetEntities<MenuItem>().FirstOrDefault(p => p.AppMetaCode == entity.MetaCode && p.MetaType == MenuModelItem.MetaTypeMenuItem);
                if (menuitem != null)
                {
                    menuitem.Action = model.MainMenuItem.Action;
                    menuitem.Controller = model.MainMenuItem.Controller;
                    menuitem.Title = model.MainMenuItem.Title;
                    //menuitem.TitleLocalizationKey = model.MainMenuItem.TitleLocalizationKey;

                    Client.UpdateEntity(menuitem);
                    Client.Close();

                }
                else
                {
                    CreateApplicationMenuItem(model);
                }

                Client.UpdateEntity(entity);
                Client.Close();

                return new ApplicationModelItem(entity);

            }

        }

        private void CreateApplicationMenuItem(ApplicationModelItem model)
        {

            if (!string.IsNullOrEmpty(model.MainMenuItem.Title))
            {
                var max = 0;
                var menu = GetMenuModels();
                var root = menu.Find(p => p.IsMetaTypeMainMenu);
                if (root == null)
                {
                    var main = new MenuItem() { AppMetaCode = "", MetaType = "MAINMENU", OrderNo = 1, ParentMetaCode = "ROOT", Properties = "", Title = "Applications" };
                    Client.InsertEntity(main);
                    Client.Close();
                    max = 1;
                }
                else
                {
                    max = menu.Max(p => p.OrderNo);
                }

                var appmi = new MenuItem() { AppMetaCode = model.MetaCode, MetaType = MenuModelItem.MetaTypeMenuItem, OrderNo = max + 10, ParentMetaCode = root.MetaCode, Properties = "", Title = model.MainMenuItem.Title };
                appmi.Action = model.MainMenuItem.Action;
                appmi.Controller = model.MainMenuItem.Controller;
                appmi.MetaCode = BaseModelItem.GenerateNewMetaCode(model.MainMenuItem);
                Client.InsertEntity(appmi);
                Client.Close();

            }

        }
        #endregion

        #region Endpoints
        public List<EndpointModelItem> GetEndpointModels()
        {

            List<EndpointModelItem> res;
           

            if (ModelCache.TryGetValue(EndpointsCacheKey, out res))
            {
                return res;
            }

            var appmodels = GetAppModels();
            var dbmodels = GetDatabaseModels();
            var dataviews = GetDataViewModels();

            Client.Open();
            res = Client.GetEntities<EndpointItem>().Select(p => new EndpointModelItem(p)).ToList();
            Client.Close();

            foreach (var ep in res)
            {
                if (ep.Path.Length > 0)
                {
                    ep.Path.Trim();
                    if (ep.Path[0] != '/')
                        ep.Path = "/" + ep.Path;
                    if (ep.Path[ep.Path.Length - 1] != '/')
                        ep.Path = ep.Path + "/";

                }

                if ((ep.IsMetaTypeTableGetById || ep.IsMetaTypeTableGetAll || ep.IsMetaTypeTableSave)
                    && !string.IsNullOrEmpty(ep.AppMetaCode) && !string.IsNullOrEmpty(ep.DataMetaCode))
                {
                   
                    var appmodel = appmodels.Find(p => p.MetaCode == ep.AppMetaCode);
                    if (appmodel != null && ep.DataMetaCode == appmodel.MetaCode)
                        ep.DataTableInfo = new DatabaseModelItem(DatabaseModelItem.MetaTypeDataTable) { AppMetaCode = appmodel.MetaCode, Id = 0, DbName = appmodel.DbName, TableName = appmodel.DbName, MetaCode = appmodel.MetaCode, ParentMetaCode = "ROOT", Title = appmodel.DbName, IsFrameworkItem = true }; ;

                    if (ep.DataTableInfo == null && appmodel != null) 
                    {
                        var table = dbmodels.Find(p => p.IsMetaTypeDataTable && p.MetaCode == ep.DataMetaCode);
                        if (table != null)
                            ep.DataTableInfo = table;
                    }
                }

                if (ep.IsMetaTypeDataViewGetData && !string.IsNullOrEmpty(ep.DataMetaCode))
                {
                    var dv = dataviews.Find(p => p.IsMetaTypeDataView && p.MetaCode == ep.DataMetaCode);
                    if (dv != null)
                        ep.DataViewInfo = dv;
                   
                }

            }

            ModelCache.Set(EndpointsCacheKey, res);

            return res;
        }

        public void SaveEndpointModels(List<EndpointModelItem> model)
        {
            ModelCache.Remove(EndpointsCacheKey);

            foreach (var ep in model)
            {
                 ep.ParentMetaCode = "ROOT";

               
            }

            Client.Open();
            foreach (var ep in model)
            {
                if (ep.Id < 1)
                {
                    var t = new EndpointItem()
                    {
                        MetaType = ep.MetaType,
                        ParentMetaCode = ep.ParentMetaCode,
                        Title = ep.Title,
                        AppMetaCode = ep.AppMetaCode,
                        DataMetaCode = ep.DataMetaCode,
                        Description = ep.Description,
                        OrderNo = ep.OrderNo,
                        Path = ep.Path,
                        Properties = ep.Properties

                    };

                    if (string.IsNullOrEmpty(t.MetaCode))
                        t.MetaCode = BaseModelItem.GenerateNewMetaCode(ep);

                    Client.InsertEntity(t);
                }
                else
                {
                    var existing = Client.GetEntities<EndpointItem>().FirstOrDefault(p => p.Id == ep.Id);
                    if (existing != null)
                    {
                        existing.AppMetaCode = ep.AppMetaCode;
                        existing.OrderNo = ep.OrderNo;
                        existing.Path = ep.Path;
                        existing.Properties = ep.Properties;
                        existing.Title = ep.Title;
                        existing.DataMetaCode = ep.DataMetaCode;
                        existing.Description = ep.Description;
                        
                        Client.UpdateEntity(existing);
                    }

                }

            }
            Client.Close();
        }

        public void DeleteEndpointModel(int id)
        {
            ModelCache.Remove(EndpointsCacheKey);

            Client.Open();
            var existing = Client.GetEntities<EndpointItem>().FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
               Client.DeleteEntity(existing);
            }
            Client.Close();
        }
        #endregion

        #region UI
        public List<UserInterfaceModelItem> GetUserInterfaceModels()
        {

            Client.Open();
            var res = Client.GetEntities<UserInterfaceItem>().Select(p => new UserInterfaceModelItem(p)).ToList();
            Client.Close();

            var dbmodelitems = GetDatabaseModels();
            var apps = GetAppModels();
            var views = GetDataViewModels();

            foreach (var app in apps)
            {
                foreach (var item in res.OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
                {
                    if (item.AppMetaCode == app.MetaCode)
                    {
                        if (!string.IsNullOrEmpty(item.DataMetaCode))
                        {
                            var dinf = dbmodelitems.Find(p => p.MetaCode == item.DataMetaCode && p.AppMetaCode == app.MetaCode);
                            if (dinf != null && dinf.IsMetaTypeDataColumn)
                                item.DataColumnInfo = dinf;
                            else if (dinf != null && dinf.IsMetaTypeDataTable)
                                item.DataTableInfo = dinf;


                            if (item.DataColumnInfo != null && item.DataTableInfo == null)
                            {
                                if (!item.DataColumnInfo.IsRoot)
                                {
                                    dinf = dbmodelitems.Find(p => p.MetaCode == item.DataColumnInfo.ParentMetaCode && p.AppMetaCode == app.MetaCode && p.IsMetaTypeDataTable);
                                    if (dinf != null)
                                        item.DataTableInfo = dinf;
                                }
                                else
                                {
                                    item.DataTableInfo = new DatabaseModelItem(DatabaseModelItem.MetaTypeDataTable) { AppMetaCode = app.MetaCode, Id = 0, DbName = app.DbName, TableName = app.DbName, MetaCode = app.MetaCode, ParentMetaCode = "ROOT", Title = app.DbName, IsFrameworkItem = true };
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(item.DataMetaCode2))
                        {
                            var dinf = dbmodelitems.Find(p => p.MetaCode == item.DataMetaCode2 && p.AppMetaCode == app.MetaCode && p.IsMetaTypeDataColumn);
                            if (dinf != null)
                                item.DataColumnInfo2 = dinf;
                        }

                        if (item.HasDataViewDomain)
                        {
                            var vinf = views.Find(p => p.MetaCode == item.DataViewName && p.IsRoot);
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
            }


            return res;
        }

        public void SaveUserInterfaceModels(List<UserInterfaceModelItem> model)
        {
            ModelCache.Remove(AppModelCacheKey);

            foreach (var t in model)
            {
                if (t.Id > 0 && t.HasProperty("REMOVED"))
                {
                    var existing = Client.GetEntities<UserInterfaceItem>().FirstOrDefault(p => p.Id == t.Id);
                    if (existing != null)
                    {
                        Client.DeleteEntity(existing);
                        Client.Close();
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

                    Client.InsertEntity(CreateMetaUIItem(uic));

                }
                else
                {
                    var existing = Client.GetEntities<UserInterfaceItem>().FirstOrDefault(p => p.Id == uic.Id);
                    if (existing != null)
                    {
                        existing.Title = uic.Title;
                        //existing.TitleLocalizationKey = uic.TitleLocalizationKey;
                        existing.RowOrder = uic.RowOrder;
                        existing.ColumnOrder = uic.ColumnOrder;
                        existing.DataMetaCode = uic.DataMetaCode;
                        existing.DataMetaCode2 = uic.DataMetaCode2;
                        existing.ViewMetaCode = uic.ViewMetaCode;
                        existing.ViewMetaCode2 = uic.ViewMetaCode2;
                        existing.Domain = uic.Domain;
                        existing.Description = uic.Description;
                        existing.Properties = uic.Properties;
                        Client.UpdateEntity(existing);
                    }

                }

            }

            Client.Close();


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
                //TitleLocalizationKey = dto.TitleLocalizationKey,
                Properties = dto.Properties
                
            };

            return res;

        }
        #endregion

        #region Database

        public List<DatabaseModelItem> GetDatabaseModels()
        {
            var idgen = 260001;
            var apps = GetAppModels();
            Client.Open();
            var dbitems = Client.GetEntities<DatabaseItem>().Select(p => new DatabaseModelItem(p)).ToList();
            Client.Close();
            var res = new List<DatabaseModelItem>();

            foreach (var app in apps)
            {
                res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, app.DbName, "Id", DatabaseModelItem.DataTypeInt));
                res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, app.DbName, "Version", DatabaseModelItem.DataTypeInt));
                res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, app.DbName, "ApplicationId", DatabaseModelItem.DataTypeInt));
                res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, app.DbName, "CreatedBy", DatabaseModelItem.DataTypeString));
                res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, app.DbName, "ChangedBy", DatabaseModelItem.DataTypeString));
                res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, app.DbName, "OwnedBy", DatabaseModelItem.DataTypeString));
                res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, app.DbName, "ChangedDate", DatabaseModelItem.DataTypeDateTime));

                foreach (var column in dbitems.Where(p => p.IsMetaTypeDataColumn && p.AppMetaCode == app.MetaCode && p.IsRoot))
                {
                    if (res.Exists(p => p.DbName.ToUpper() == column.DbName.ToUpper() &&
                                        p.IsRoot &&
                                        p.AppMetaCode == app.MetaCode &&
                                        p.IsMetaTypeDataColumn))
                        continue;

                    column.TableName = app.DbName;
                    res.Add(column);
                }

                foreach (var table in dbitems.Where(p => p.IsMetaTypeDataTable && p.AppMetaCode == app.MetaCode))
                {
                    res.Add(table);
                    res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, table.DbName, "Id", DatabaseModelItem.DataTypeInt, table.MetaCode));
                    res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, table.DbName, "Version", DatabaseModelItem.DataTypeInt, table.MetaCode));
                    res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, table.DbName, "ApplicationId", DatabaseModelItem.DataTypeInt, table.MetaCode));
                    res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, table.DbName, "CreatedBy", DatabaseModelItem.DataTypeString, table.MetaCode));
                    res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, table.DbName, "ChangedBy", DatabaseModelItem.DataTypeString, table.MetaCode));
                    res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, table.DbName, "OwnedBy", DatabaseModelItem.DataTypeString, table.MetaCode));
                    res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, table.DbName, "ChangedDate", DatabaseModelItem.DataTypeDateTime, table.MetaCode));
                    res.Add(DatabaseModelItem.CreateFrameworkColumn(idgen++, app.MetaCode, table.DbName, "ParentId", DatabaseModelItem.DataTypeInt, table.MetaCode));

                    foreach (var column in dbitems.Where(p => p.IsMetaTypeDataColumn && p.AppMetaCode == app.MetaCode && p.ParentMetaCode == table.MetaCode && !p.IsRoot))
                    {
                        if (res.Exists(p => p.DbName.ToUpper() == column.DbName.ToUpper() && 
                                            p.ParentMetaCode == table.ParentMetaCode &&
                                            p.AppMetaCode == app.MetaCode && 
                                            p.IsMetaTypeDataColumn))
                            continue;

                        column.TableName = table.DbName;


                       res.Add(column);
                        
                    }
                }

               

            }

           
            return res;
        }

        public void SaveDatabaseModels(List<DatabaseModelItem> model, int applicationid)
        {
          

            var app = GetApplicationModels().Find(p => p.Application.Id == applicationid);
            if (app == null)
                throw new InvalidOperationException("Could not find application when saving application database model.");

            foreach (var dbi in model)
            {
                if (dbi.IsFrameworkItem)
                    continue;

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
                if (dbi.IsFrameworkItem)
                    continue;

                if (app.DataStructure.Exists(p => p.IsFrameworkItem && p.DbName.ToUpper() == dbi.DbName.ToUpper()))
                    continue;

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

                    Client.InsertEntity(t);

                }
                else
                {
                  

                    var existing = Client.GetEntities<DatabaseItem>().FirstOrDefault(p => p.Id == dbi.Id);
                    if (existing != null)
                    {
                        existing.DataType = dbi.DataType;
                        existing.MetaType = dbi.MetaType;
                        existing.Description = dbi.Description;
                        existing.ParentMetaCode = dbi.ParentMetaCode;
                        existing.DbName = dbi.DbName;
                        existing.Properties = dbi.Properties;
                        Client.UpdateEntity(existing);
                    }

                }

            }

            Client.Close();


            ModelCache.Remove(AppModelCacheKey);

        }

        public void DeleteDatabaseModel(int id)
        {
           

            var existing = Client.GetEntities<DatabaseItem>().FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                var dto = new DatabaseModelItem(existing);
                var app = GetApplicationModels().Find(p => p.Application.MetaCode == dto.AppMetaCode);
                if (app == null)
                    return;

                if (dto.IsMetaTypeDataTable && dto.DbName != app.Application.DbName)
                {
                    Client.Open();
                    var childlist = Client.GetEntities<DatabaseItem>().Where(p => (p.MetaType == DatabaseModelItem.MetaTypeDataColumn) && p.ParentMetaCode == existing.MetaCode).ToList();
                    Client.DeleteEntity(existing);
                    Client.DeleteEntities(childlist);
                    Client.Close();
                }
                else
                {
                    Client.Open();
                    Client.DeleteEntity(existing);
                    Client.Close();
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
                MetaCode = dto.MetaCode,
                MetaType = dto.MetaType,
                ParentMetaCode = dto.ParentMetaCode,
                DbName = dto.DbName,
                DataType = dto.DataType,
                Properties = dto.Properties
            };


            return res;

        }
        #endregion

        #region Data Views

        public List<DataViewModelItem> GetLocalizedDataViewModels()
        {
            var res = GetDataViewModels();
            LocalizeTitles(res.ToList<ILocalizableTitle>());
            return res;
        }

        public List<DataViewModelItem> GetDataViewModels()
        {
            List<DataViewModelItem> res;
            if (ModelCache.TryGetValue(DataViewCacheKey, out res))
            {
                return res;
            }

            Client.Open();
            res = Client.GetEntities<DataViewItem>().Select(p => new DataViewModelItem(p)).ToList();
            Client.Close();

            ModelCache.Set(DataViewCacheKey, res);

            return res;
        }

        public void SaveDataViewModels(List<DataViewModelItem> model)
        {
            ModelCache.Remove(DataViewCacheKey);

            foreach (var dv in model)
            {

                if (dv.IsMetaTypeDataView)
                    dv.ParentMetaCode = "ROOT";

                if (string.IsNullOrEmpty(dv.MetaCode))
                    dv.MetaCode = BaseModelItem.GenerateNewMetaCode(dv);

            }

            Client.Open();
            foreach (var dv in model)
            {
                if (dv.Id < 1)
                {
                    var t = new DataViewItem()
                    {

                        MetaCode = dv.MetaCode,
                        MetaType = dv.MetaType,
                        ParentMetaCode = dv.ParentMetaCode,
                        Title = dv.Title,
                        //TitleLocalizationKey = dto.TitleLocalizationKey,
                        SQLQuery = dv.SQLQuery,
                        SQLQueryFieldName = dv.SQLQueryFieldName


                    };
                    Client.InsertEntity(t);
                }
                else
                {
                    var existing = Client.GetEntities<DataViewItem>().FirstOrDefault(p => p.Id == dv.Id);
                    if (existing != null)
                    {
                        existing.SQLQuery = dv.SQLQuery;
                        existing.SQLQueryFieldName = dv.SQLQueryFieldName;
                        existing.Title = dv.Title;
                        //existing.TitleLocalizationKey = dv.TitleLocalizationKey;
                        Client.UpdateEntity(existing);
                    }

                }

            }
            Client.Close();

        }



      

        public void DeleteDataViewModel(int id)
        {

            ModelCache.Remove(DataViewCacheKey);

            Client.Open();
            var existing = Client.GetEntities<DataViewItem>().FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                var dto = new DataViewModelItem(existing);
                if (dto.IsMetaTypeDataView)
                {
                    var childlist = Client.GetEntities<DataViewItem>().Where(p => (p.MetaType == "DATAVIEWFIELD" || p.MetaType == "DATAVIEWKEYFIELD") && p.ParentMetaCode == existing.MetaCode).ToList();
                    Client.DeleteEntity(existing);
                    Client.DeleteEntities(childlist);
                }
                else
                {
                    Client.DeleteEntity(existing);
                }
            }
            Client.Close();
        }
        #endregion

        #region Value Domains

        public void SaveValueDomains(List<ValueDomainModelItem> model)
        {
            ModelCache.Remove(ValueDomainsCacheKey);

            Client.Open();
            foreach (var vd in model)
            {
                if (!vd.IsValid)
                    throw new InvalidOperationException("Missing required information on value domain, can't save.");

                if (vd.Id < 1)
                {
                    Client.InsertEntity(new ValueDomainItem() { DomainName = vd.DomainName, Value = vd.Value, Code = vd.Code });
                }
                else
                {
                    var existing = Client.GetEntities<ValueDomainItem>().FirstOrDefault(p => p.Id == vd.Id);
                    if (existing != null)
                    {
                        existing.Code = vd.Code;
                        existing.Value = vd.Value;
                        existing.DomainName = vd.DomainName;
                        Client.UpdateEntity(existing);
                    }

                }

            }
            Client.Close();
        }

        public List<ValueDomainModelItem> GetValueDomains()
        {
            List<ValueDomainModelItem> res;
            if (ModelCache.TryGetValue(ValueDomainsCacheKey, out res))
            {
                return res;
            }

            Client.Open();
            var t = Client.GetEntities<ValueDomainItem>().Select(p => new ValueDomainModelItem(p)).ToList();
            Client.Close();
            LocalizeTitles(t.ToList<ILocalizableTitle>());
           
            return t;
        }

        public void DeleteValueDomain(int id)
        {
            ModelCache.Remove(ValueDomainsCacheKey);
            var existing = Client.GetEntities<ValueDomainItem>().FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                Client.Open();
                Client.DeleteEntity(existing);
                Client.Close();
            }
        }





        #endregion

        #region translations
      

        public List<TranslationModelItem> GetTranslations()
        {
            List<TranslationModelItem> res;
            if (ModelCache.TryGetValue(TranslationsCacheKey, out res))
            {
                return res;
            }
            Client.Open();
            var t = Client.GetEntities<TranslationItem>().Select(p => new TranslationModelItem(p)).ToList();
            Client.Close();
            return t;
        }

        public void SaveTranslations(List<TranslationModelItem> model)
        {
            ModelCache.Remove(TranslationsCacheKey);

            Client.Open();
            foreach (var trans in model)
            {

                if (trans.Id < 1)
                {
                    Client.InsertEntity(new TranslationItem() { Culture = trans.Culture, TransKey = trans.Key, Text = trans.Text });
                }
                else
                {
                    var existing = Client.GetEntities<TranslationItem>().FirstOrDefault(p => p.Id == trans.Id);
                    if (existing != null)
                    {
                        existing.Culture = trans.Culture;
                        existing.TransKey = trans.Key;
                        existing.Text = trans.Text;
                        Client.UpdateEntity(existing);
                    }
                }

            }
            Client.Close();

        }

        public void DeleteTranslation(int id)
        {
            ModelCache.Remove(TranslationsCacheKey);
            var existing = Client.GetEntities<TranslationItem>().FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                Client.Open();
                Client.DeleteEntity(existing);
                Client.Close();
            }
        }
        #endregion


        private void LocalizeTitles(List<ILocalizableTitle> list)
        {
            
            //Localization
            var translations = Client.GetEntities<TranslationItem>();

            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.TitleLocalizationKey))
                {
                    var trans = translations.Find(p => p.Culture == CurrentCulture && p.TransKey == item.TitleLocalizationKey);
                    if (trans != null)
                        item.Title = trans.Text;
                    else
                        item.Title = item.TitleLocalizationKey;

                    if (CurrentCulture == "sv-SE" && item.Title == "Kunder")
                    {
                        var x = "";
                    }
                }
            }
        }

        private void LocalizeTitle(ILocalizableTitle item)
        {
            if (item == null)
                return;

            //Localization
            var translations = Client.GetEntities<TranslationItem>();


            if (!string.IsNullOrEmpty(item.TitleLocalizationKey))
            {
                var trans = translations.Find(p => p.Culture == CurrentCulture && p.TransKey == item.TitleLocalizationKey);
                if (trans != null)
                    item.Title = trans.Text;
                else
                    item.Title = item.TitleLocalizationKey;
            }
            
        }

        #region misc


        public void CreateIntwentyDatabase()
        {

            if (!Settings.ReCreateDatabaseOnStartup)
                return;

            var client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);

            client.Open();
            client.CreateTable<ApplicationItem>();
            client.CreateTable<DatabaseItem>();
            client.CreateTable<DataViewItem>();
            client.CreateTable<EventLog>();
            client.CreateTable<InformationStatus>();
            client.CreateTable<MenuItem>();
            client.CreateTable<SystemID>();
            client.CreateTable<UserInterfaceItem>();
            client.CreateTable<ValueDomainItem>();
            client.CreateTable<DefaultValue>();
            client.CreateTable<TranslationItem>();
            client.CreateTable<EndpointItem>();

            client.CreateTable<IntwentyUser>(); //security_User
            client.CreateTable<IntwentyRole>(); //security_Role
            client.CreateTable<IntwentyUserRole>(); //security_UserRoles
            client.CreateTable<IntwentyGroup>(); //security_Group
            client.CreateTable<IntwentyUserGroup>(); //security_UserGroup

            client.CreateTable<IntwentyUserClaim>(); //security_UserClaims
            client.CreateTable<IntwentyUserLogin>(); //security_UserLogins
            //client.CreateTable<IntwentyRoleClaim>(true, true); //security_RoleClaims
            //client.CreateTable<IntwentyUserToken>(true, true); //security_UserTokens

            client.Close();
            
        }

       
      

        public List<OperationResult> ConfigureDatabase()
        {
            var databasemodel = GetDatabaseModels();
            var res = new List<OperationResult>();
            var l = GetAppModels();
            foreach (var model in l)
            {
                res.Add(ConfigureDatabase(model, databasemodel));
            }

            return res;
        }

        public OperationResult ConfigureDatabase(ApplicationModelItem model, List<DatabaseModelItem> databasemodel = null)
        {
            var res = new OperationResult(true, MessageCode.RESULT, string.Format("Database configured for application {0}", model.Title));

            try
            {
              
                if (databasemodel == null) 
                {
                    databasemodel = GetDatabaseModels();
                }

                Client.Open();
                var maintable_default_cols = databasemodel.Where(p => p.IsMetaTypeDataColumn && p.IsRoot && p.IsFrameworkItem && p.AppMetaCode == model.MetaCode).ToList();
                if (maintable_default_cols == null)
                    throw new InvalidOperationException("Found application without main table default columns " + model.DbName);
                if (maintable_default_cols.Count == 0)
                    throw new InvalidOperationException("Found application without main table default columns " + model.DbName);


                CreateMainTable(model, maintable_default_cols, res);
                CreateApplicationVersioningTable(model, res);

                foreach (var t in databasemodel)
                {
                    if (t.AppMetaCode != model.MetaCode)
                        continue;

                    if (t.IsMetaTypeDataColumn && t.IsRoot && !t.IsFrameworkItem)
                    {
                        CreateDBColumn(t, model.DbName, res);
                    }

                    if (t.IsMetaTypeDataTable)
                    {
                        var subtable_default_cols = databasemodel.Where(p => p.IsMetaTypeDataColumn && !p.IsRoot && p.IsFrameworkItem && t.AppMetaCode == model.MetaCode && p.ParentMetaCode == t.MetaCode).ToList();
                        if (subtable_default_cols == null)
                            throw new InvalidOperationException("Found application subtable without default columns");
                        if (subtable_default_cols.Count == 0)
                            throw new InvalidOperationException("Found application subtable without default columns");

                        CreateDBTable(model, t, subtable_default_cols, res);
                        foreach (var col in databasemodel)
                        {
                            if (col.IsFrameworkItem || col.AppMetaCode != model.MetaCode || col.IsRoot || col.ParentMetaCode != t.MetaCode)
                                continue;

                            CreateDBColumn(col, t.DbName, res);
                        }

                        CreateSubtableIndexes(t, res);


                    }
                }

                CreateMainTableIndexes(model, res);

            }
            catch (Exception ex)
            {
                res = new OperationResult(false, MessageCode.SYSTEMERROR, ex.Message);
            }
            finally
            {
                Client.Close();
            }

            return res;
        }


        public OperationResult ValidateModel()
        {

            ModelCache.Remove(AppModelCacheKey);

            var apps = GetApplicationModels();
            var viewinfo = GetDataViewModels();
            var endpointinfo = GetEndpointModels();
            var res = new OperationResult();

            if (apps.Count == 0)
            {
                res.IsSuccess = false;
                res.AddMessage(MessageCode.SYSTEMERROR, "The model doesn't seem to exist");
            }

            foreach (var a in apps)
            {

                if (string.IsNullOrEmpty(a.Application.Title))
                {
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The application with Id: {0} has no [Title].", a.Application.Id));
                    return res;
                }

                if (string.IsNullOrEmpty(a.Application.MetaCode))
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The application: {0} has no [MetaCode].", a.Application.Title));

                if (string.IsNullOrEmpty(a.Application.DbName))
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The application: {0} has no [DbName].", a.Application.Title));

                if (!string.IsNullOrEmpty(a.Application.MetaCode) && (a.Application.MetaCode.ToUpper() != a.Application.MetaCode))
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The application: {0} has a non uppercase [MetaCode].", a.Application.Title));

                if (a.DataStructure.Count == 0)
                    res.AddMessage(MessageCode.WARNING, string.Format("The application {0} has no Database objects (DATVALUE, DATATABLE, etc.). Or MetaDataItems has wrong [AppMetaCode]", a.Application.Title));

                if (a.UIStructure.Count == 0)
                    res.AddMessage(MessageCode.WARNING, string.Format("The application {0} has no UI objects.", a.Application.Title));


                foreach (var ui in a.UIStructure)
                {

                    if (string.IsNullOrEmpty(ui.MetaCode))
                    {
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The UI object {0} in application: {1} has no [MetaCode].", ui.Title, a.Application.Title));
                        return res;
                    }

                    if (string.IsNullOrEmpty(ui.ParentMetaCode))
                    {
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The UI object {0} in application: {1} has no [ParentMetaCode].", ui.Title, a.Application.Title));
                        return res;
                    }

                    if (!ui.HasValidMetaType)
                    {
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The UI object {0} in application: {1} has not a valid [MetaType].", ui.Title, a.Application.Title));
                        return res;
                    }

                    if (string.IsNullOrEmpty(ui.Title) && !ui.IsMetaTypePanel && !ui.IsMetaTypeSection)
                    {
                        res.AddMessage(MessageCode.WARNING, string.Format("The UI object {0} in application {1} has no [Title].", ui.MetaType, a.Application.Title));
                    }


                    if (ui.IsMetaTypeEditListView && !a.UIStructure.Exists(p => p.ParentMetaCode == ui.MetaCode && p.IsMetaTypeEditListViewColumn))
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The UI object {0} of type EDITLISTVIEW in application {1} has no children with [MetaType]=EDITLISTVIEWFIELD.", ui.Title, a.Application.Title));


                    if (ui.IsMetaTypeLookUp && !ui.IsDataViewColumnConnected)
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The UI object {0} of type LOOKUP in application {1} has no connection to a DATAVIEWKEYCOLUMN", ui.Title, a.Application.Title));

                    if (ui.IsMetaTypeLookUp && ui.IsDataViewColumnConnected && ui.DataViewColumnInfo.IsMetaTypeDataViewColumn)
                    {
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The UI object {0} of type LOOKUP in application {1} has a connection (ViewMetaCode) to a DATAVIEWCOLUMN, it should be a DATAVIEWKEYCOLUMN", ui.Title, a.Application.Title));
                    }

                    if (ui.IsMetaTypeLookUp && !ui.IsDataViewConnected)
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The UI object {0} of type LOOKUP in application {1} is not connected to a dataview, check domainname.", ui.Title, a.Application.Title));

                    if (!ui.IsMetaTypeEditGrid)
                    {
                        if (!ui.IsDataColumnConnected && !string.IsNullOrEmpty(ui.DataMetaCode))
                            res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The UI object: {0} in application {1} has a missconfigured connection to a database column [DataMetaCode].", new object[] { ui.Title, a.Application.Title }));
                    }
                    else
                    {
                        if (!ui.IsDataTableConnected)
                            res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The UI object: {0} in application {1} has a missconfigured connection to a database table [DataMetaCode].", new object[] { ui.Title, a.Application.Title }));
                    }

                    if (ui.IsMetaTypeEditGridLookUp && !ui.IsDataViewColumnConnected)
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The UI object {0} of type EDITGRID_LOOKUP in application {1} has no connection to a DATAVIEWKEYCOLUMN", ui.Title, a.Application.Title));

                    if (!ui.HasValidProperties)
                    {
                        res.AddMessage(MessageCode.WARNING, string.Format("One or more properties on the UI object: {0} of type {1} in application {2} is not valid and may not be implemented.", new object[] { ui.Title, ui.MetaType, a.Application.Title }));
                    }

                }

                if (a.UIStructure.Count(p => p.IsMetaTypeEditListView) > 1)
                {
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The application: {0} has multiple EDITLISTVIEW ui objects, which is not allowd", a.Application.Title));
                }

                foreach (var db in a.DataStructure)
                {
                    if (string.IsNullOrEmpty(db.MetaCode))
                    {
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The data object with Id: {0} in application: {1} has no [MetaCode].", db.Id, a.Application.Title));
                        return res;
                    }

                    if (string.IsNullOrEmpty(db.ParentMetaCode))
                    {
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The data object: {0} in application: {1} has no [ParentMetaCode]. (ROOT ?)", db.MetaCode, a.Application.Title));
                        return res;
                    }

                    if (string.IsNullOrEmpty(db.MetaType))
                    {
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The data object: {0} in application: {1} has no [MetaType].", db.MetaCode, a.Application.Title));
                        return res;
                    }

                    if (string.IsNullOrEmpty(db.DbName))
                    {
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The data object: {0} in application {1} has no [DbName].", db.MetaCode, a.Application.Title));
                    }

                    if (!string.IsNullOrEmpty(db.DbName) && db.DbName.ToUpper() == db.DbName && db.IsMetaTypeDataColumn)
                    {
                        res.AddMessage(MessageCode.WARNING, string.Format("The data column: {0} in application {1} has an uppercase [DbName], ok but intwenty thinks it's uggly.", db.DbName, a.Application.Title));
                    }

                    if (!string.IsNullOrEmpty(db.DbName) && db.DbName.ToUpper() == db.DbName && db.IsMetaTypeDataTable)
                    {
                        res.AddMessage(MessageCode.WARNING, string.Format("The data table: {0} in application {1} has an uppercase [DbName], ok but intwenty thinks it's uggly.", db.DbName, a.Application.Title));
                    }

                  

                }

            }


            foreach (var v in viewinfo)
            {
                if (string.IsNullOrEmpty(v.Title))
                {
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The view with Id: {0} has no [Title].", v.Id));
                    return res;
                }

                if (!v.HasValidMetaType)
                {
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The view object: {0} has no [MetaType] or the MetaType is invalid.", v.Title));
                    return res;
                }

                if (!string.IsNullOrEmpty(v.SQLQueryFieldName) && v.IsMetaTypeDataView)
                {
                    res.AddMessage(MessageCode.WARNING, string.Format("The view object: {0} has a SqlQueryFieldName. It makes no sense on a DATAVIEW model item", v.Title));
                }

                if (!string.IsNullOrEmpty(v.SQLQuery) && (v.IsMetaTypeDataViewColumn || v.IsMetaTypeDataViewKeyColumn))
                {
                    res.AddMessage(MessageCode.WARNING, string.Format("The view object: {0} has a SqlQuery. It makes no sense on a DATAVIEWCOLUMN or DATAVIEWKEYCOLUMN model item)", v.Title));
                }

                if (v.IsMetaTypeDataView && !viewinfo.Exists(p => p.ParentMetaCode == v.MetaCode && p.IsMetaTypeDataViewColumn))
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The view object: {0} has no children with [MetaType]=DATAVIEWCOLUMN.", v.Title));

                if (v.IsMetaTypeDataView && !viewinfo.Exists(p => p.ParentMetaCode == v.MetaCode && p.IsMetaTypeDataViewKeyColumn))
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The view object: {0} has no children with [MetaType]=DATAVIEWKEYCOLUMN.", v.Title));

                if (v.IsMetaTypeDataViewColumn || v.IsMetaTypeDataViewKeyColumn)
                {
                    var view = viewinfo.Find(p => p.IsMetaTypeDataView && p.MetaCode == v.ParentMetaCode);
                    if (view != null)
                    {
                        if (!view.SQLQuery.Contains(v.SQLQueryFieldName))
                            res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The  DATAVIEWCOLUMN or DATAVIEWKEYCOLUMN {0} has no SQLQueryFieldName that is included in the SQL Query of the parent view.", v.Title));
                    }
                    else
                    {
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The DATAVIEWCOLUMN or DATAVIEWKEYCOLUMN  {0} has no parent with [MetaType]=DATAVIEW.", v.Title));
                    }

                    if (string.IsNullOrEmpty(v.SQLQueryFieldDataType))
                    {

                        res.AddMessage(MessageCode.WARNING, string.Format("The DATAVIEWCOLUMN or DATAVIEWKEYCOLUMN : {0} has no SQLQueryFieldDataType. STRING will be used as default.)", v.Title));

                    }
                }

                if (v.IsMetaTypeDataView)
                {
                    if (v.HasNonSelectSql)
                    {
                        res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The sql query defined for dataview {0} contains invalid commands.", v.Title));
                    }
                    else
                    {
                        var client = new Connection(Settings.DefaultConnectionDBMS, Settings.DefaultConnection);
                        try
                        {
                            client.Open();
                            var t =client.GetScalarValue(v.SQLQuery);
                            client.Close();

                        }
                        catch
                        {
                            client.Close();
                            res.AddMessage(MessageCode.WARNING, string.Format("The sql query defined for dataview {0} returned an error.", v.Title));
                        }
                    }
                }


            }

            foreach (var ep in endpointinfo)
            {
                if (string.IsNullOrEmpty(ep.MetaCode))
                {
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("There is an endpoint object without [MetaCode]"));
                    return res;
                }

                if (string.IsNullOrEmpty(ep.ParentMetaCode))
                {
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("There is an endpoint object without [ParentMetaCode]"));
                    return res;
                }

                if (!ep.HasValidMetaType)
                {
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The endpoint object with MetaCode: {0}  has no [MetaType] or the MetaType is invalid.", ep.MetaCode));
                    return res;
                }

                if (string.IsNullOrEmpty(ep.Path))
                {
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The endpoint object with MetaCode: {0} has no [Path]", ep.MetaCode));
                }

                if (string.IsNullOrEmpty(ep.Action))
                {
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The endpoint object with MetaCode: {0} has no [Action]", ep.MetaCode));
                }

                if (!ep.IsDataTableConnected && !ep.IsDataViewConnected)
                {
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The endpoint object {0} has no connection to database table or an intwenty data view", (ep.Path+ep.Action)));
                }

                if (ep.IsDataTableConnected && string.IsNullOrEmpty(ep.AppMetaCode))
                {
                    res.AddMessage(MessageCode.SYSTEMERROR, string.Format("The endpoint object {0} is connected to a table but has no [AppMetaCode]", (ep.Path + ep.Action)));
                }

            }


            if (res.Messages.Exists(p => p.Code == MessageCode.SYSTEMERROR))
            {
                res.IsSuccess = false;
            }
            else
            {
                res.IsSuccess = true;
                res.AddMessage(MessageCode.RESULT, "Model validated successfully");
            }

            return res;
        }

       

        public List<IntwentyDataColumn> GetDefaultVersioningTableColumns()
        {
            List<IntwentyDataColumn> res = null;
            if (ModelCache.TryGetValue(DefaultVersioningTableColumnsCacheKey, out res))
            {
                return res;
            }

          
            var DefaultVersioningTableColumns = new List<IntwentyDataColumn>();
            DefaultVersioningTableColumns.Add(new IntwentyDataColumn() { DataType = DatabaseModelItem.DataTypeInt, Name = "Id" });
            DefaultVersioningTableColumns.Add(new IntwentyDataColumn() { DataType = DatabaseModelItem.DataTypeInt, Name = "Version" });
            DefaultVersioningTableColumns.Add(new IntwentyDataColumn() { DataType = DatabaseModelItem.DataTypeInt, Name = "ApplicationId" });
            DefaultVersioningTableColumns.Add(new IntwentyDataColumn() { DataType = DatabaseModelItem.DataTypeString, Name = "MetaCode" });
            DefaultVersioningTableColumns.Add(new IntwentyDataColumn() { DataType = DatabaseModelItem.DataTypeString, Name = "MetaType" });
            DefaultVersioningTableColumns.Add(new IntwentyDataColumn() { DataType = DatabaseModelItem.DataTypeDateTime, Name = "ChangedDate" });
            DefaultVersioningTableColumns.Add(new IntwentyDataColumn() { DataType = DatabaseModelItem.DataTypeInt, Name = "ParentId" });
            

            ModelCache.Set(DefaultVersioningTableColumnsCacheKey, DefaultVersioningTableColumns);

            return DefaultVersioningTableColumns;

        }



        #endregion

        #region ConfigureDB

        private void CreateMainTable(ApplicationModelItem model, List<DatabaseModelItem> columns, OperationResult result)
        {

            var table_exist = false;
            table_exist = Client.TableExists(model.DbName);
            if (table_exist)
            {
                result.AddMessage(MessageCode.INFO , "Main table " + model.DbName + " for application: " + model.Title + " is already present");
            }
            else
            {

                string create_sql = GetCreateTableStmt(columns, model.DbName);
                Client.RunCommand(create_sql);
                result.AddMessage(MessageCode.INFO, "Main table: " + model.DbName + " for application: " + model.Title + "  was created successfully");

            }
        }

        private void CreateDBTable(ApplicationModelItem model, DatabaseModelItem table, List<DatabaseModelItem> columns, OperationResult result)
        {

            if (!table.IsMetaTypeDataTable)
            {
                result.AddMessage(MessageCode.SYSTEMERROR, "Invalid MetaType when configuring table");
                return;
            }


            var table_exist = false;
            table_exist = Client.TableExists(table.DbName);
            if (table_exist)
            {
                result.AddMessage(MessageCode.INFO, "Table: " + table.DbName + " in application: " + model.Title + " is already present.");
            }
            else
            {

                string create_sql = GetCreateTableStmt(columns, table.DbName);
                Client.RunCommand(create_sql);
                result.AddMessage(MessageCode.INFO, "Subtable: " + table.DbName + " in application: " + model.Title + "  was created successfully");

            }

        }

        private void CreateApplicationVersioningTable(ApplicationModelItem model, OperationResult result)
        {
            var table_exist = false;
            table_exist = Client.TableExists(model.VersioningTableName);
            if (!table_exist)
            {

                string create_sql = GetCreateVersioningTableStmt(GetDefaultVersioningTableColumns(), model.VersioningTableName);
                Client.RunCommand(create_sql);
            }
        }

        private void CreateDBColumn(DatabaseModelItem column, string tablename, OperationResult result)
        {
            
            if (!column.IsMetaTypeDataColumn)
            {
                result.AddMessage(MessageCode.SYSTEMERROR, "Invalid MetaType when configuring column");
                return;
            }


            var colexist = false;
            colexist = Client.ColumnExists(tablename, column.DbName);

            if (colexist)
            {
                result.AddMessage(MessageCode.INFO, "Column: " + column.DbName + " in table: " + tablename + " is already present.");
            }
            else
            {
                var coldt = DataTypes.Find(p => p.IntwentyType == column.DataType && p.DbEngine == Client.Database);
                string create_sql = "ALTER TABLE " + tablename + " ADD " + column.DbName + " " + coldt.DBMSDataType;
                Client.RunCommand(create_sql);
                result.AddMessage(MessageCode.INFO, "Column: " + column.DbName + " (" + coldt.DBMSDataType + ") was created successfully in table: " + tablename);

            }

        }

      

        private void CreateMainTableIndexes(ApplicationModelItem model, OperationResult result)
        {
            string sql = string.Empty;


            try
            {
                //Ctreate index on main application table
                sql = string.Format("CREATE UNIQUE INDEX {0}_Idx1 ON {0} (Id, Version)", model.DbName);
                Client.RunCommand(sql);
            }
            catch { }

            try
            {
                //Create index on versioning table
                sql = string.Format("CREATE UNIQUE INDEX {0}_Idx1 ON {0} (Id, Version, MetaCode, MetaType)", model.VersioningTableName);
                Client.RunCommand(sql);
            }
            catch { }

            result.AddMessage(MessageCode.INFO, "Database Indexes was created successfully for " + model.DbName);



        }

        private void CreateSubtableIndexes(DatabaseModelItem model, OperationResult result)
        {
            string sql = string.Empty;


            if (!model.IsMetaTypeDataTable)
                return;
         
             
            try
            {
                sql = string.Format("CREATE UNIQUE INDEX {0}_Idx1 ON {0} (Id, Version)", model.DbName);
                Client.RunCommand(sql);
            }
            catch { }

            try
            {
                sql = string.Format("CREATE INDEX {0}_Idx3 ON {0} (ParentId)", model.DbName);
                Client.RunCommand(sql);
            }
            catch { }



            result.AddMessage(MessageCode.INFO, "Database Indexes was created successfully for " + model.DbName);



        }

        private string GetCreateTableStmt(List<DatabaseModelItem> columns, string tablename)
        {
            var res = string.Format("CREATE TABLE {0}", tablename) + " (";
            var sep = "";
            foreach (var c in columns)
            {
                TypeMapItem dt;
                if (c.DataType == DatabaseModelItem.DataTypeString)
                    dt = DataTypes.Find(p => p.IntwentyType == c.DataType && p.DbEngine == Client.Database && p.Length == StringLength.Short);
                else if (c.DataType == DatabaseModelItem.DataTypeText)
                    dt = DataTypes.Find(p => p.IntwentyType == c.DataType && p.DbEngine == Client.Database && p.Length == StringLength.Long);
                else
                    dt = DataTypes.Find(p => p.IntwentyType == c.DataType && p.DbEngine == Client.Database);

                res += sep + string.Format("{0} {1} not null", c.Name, dt.DBMSDataType);
                sep = ", ";
            }

            res += ")";

            return res;

        }

        private string GetCreateVersioningTableStmt(List<IntwentyDataColumn> columns, string tablename)
        {
            var res = string.Format("CREATE TABLE {0}", tablename) + " (";
            var sep = "";
            foreach (var c in columns)
            {
               
                TypeMapItem dt;
                if (c.DataType == DatabaseModelItem.DataTypeString)
                    dt = DataTypes.Find(p => p.IntwentyType == c.DataType && p.DbEngine == Client.Database && p.Length == StringLength.Short);
                else if (c.DataType == DatabaseModelItem.DataTypeText)
                    dt = DataTypes.Find(p => p.IntwentyType == c.DataType && p.DbEngine == Client.Database && p.Length == StringLength.Long);
                else
                    dt = DataTypes.Find(p => p.IntwentyType == c.DataType && p.DbEngine == Client.Database);

                res += sep + string.Format("{0} {1} not null", c.Name, dt.DBMSDataType);
                sep = ", ";
            }

            res += ")";

            return res;

        }

      




        #endregion




    }

}
