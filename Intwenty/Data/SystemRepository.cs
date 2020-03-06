﻿
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Moley.Data.Entity;
using Moley.Data.Dto;
using System;

namespace Moley.Data
{
    /// <summary>
    /// Interface for operations on meta data
    /// </summary>
    public interface ISystemRepository
    {
        List<ApplicationDescriptionDto> GetApplicationDescriptions();

        List<MetaDataItemDto> GetMetaDataItems();

        List<MetaUIItemDto> GetMetaUIItems();

        List<MetaDataViewDto> GetMetaDataViews();

        List<ApplicationDto> GetApplicationMeta();

        List<MetaMenuItemDto> GetMetaMenuItems();

        List<ValueDomainDto> GetValueDomains();

        List<NoSerieDto> GetNewNoSeriesValues(int applicationid );

        List<NoSerieDto> GetNoSeries();

        ApplicationDescriptionDto SaveApplication(ApplicationDescriptionDto model);

        void SaveApplicationUI(List<MetaUIItemDto> model);

        void SaveApplicationDB(List<MetaDataItemDto> model, int applicationid);

        void SaveDataView(List<MetaDataViewDto> model);

        void SaveValueDomains(List<ValueDomainDto> model);

        void DeleteApplicationUI(int id);

        void DeleteApplicationDB(int id);

        void DeleteDataView(int id);

    }

    public class SystemRepository : ISystemRepository
    {

        private ApplicationDbContext context;
        private DbSet<ApplicationDescription> AppDescription;
        private DbSet<MetaDataItem> MetaDBItem;
        private DbSet<MetaUIItem> MetaUIItem;
        private DbSet<MetaDataView> MetaDataViews;
        private DbSet<MetaMenuItem> MetaMenuItems;
        private DbSet<NoSerie> NoSeries;
        private DbSet<ValueDomain> ValueDomains;


        public SystemRepository(ApplicationDbContext context)
        {
            this.context = context;
            AppDescription = context.Set<ApplicationDescription>();
            MetaDBItem = context.Set<MetaDataItem>();
            MetaUIItem = context.Set<MetaUIItem>();
            MetaDataViews = context.Set<MetaDataView>();
            MetaMenuItems = context.Set<MetaMenuItem>();
            NoSeries = context.Set<NoSerie>();
            ValueDomains = context.Set<ValueDomain>();
        }

        public List<ApplicationDescriptionDto> GetApplicationDescriptions()
        {
            var t = AppDescription.Select(p=> new ApplicationDescriptionDto(p)).ToList();
            var menu = GetMetaMenuItems();
            foreach (var app in t)
            {
                var mi = menu.Find(p => p.IsMetaTypeMenuItem && p.Application.Id == app.Id);
                if (mi != null)
                    app.MainMenuItem = mi;
            }

            return t;
        }



        public List<MetaDataItemDto> GetMetaDataItems()
        {
            return MetaDBItem.Select(p => new MetaDataItemDto(p)).ToList();
        }

        public List<MetaUIItemDto> GetMetaUIItems()
        {
            var dataitems = MetaDBItem.Select(p => new MetaDataItemDto(p)).ToList();
            var res = MetaUIItem.Select(p => new MetaUIItemDto(p)).ToList();

            foreach (var item in res)
            {
               
                if (!string.IsNullOrEmpty(item.DataMetaCode))
                {
                    var dinf = dataitems.Find(p => p.MetaCode == item.DataMetaCode && p.AppMetaCode == item.AppMetaCode);
                    if (dinf != null)
                        item.DataInfo = dinf;
                }
                
            }

            return res;
        }

        public List<ApplicationDto> GetApplicationMeta()
        {
            var res = new List<ApplicationDto>();
            var apps =  GetApplicationDescriptions();
            var ditems = MetaDBItem.Select(p => new MetaDataItemDto(p)).ToList();
            var uitems = MetaUIItem.Select(p => new MetaUIItemDto(p)).ToList();
            var views = MetaDataViews.Select(p => new MetaDataViewDto(p)).ToList();
            var noseries = NoSeries.Select(p => new NoSerieDto(p)).ToList();


            foreach (var app in apps)
            {
                var t = new ApplicationDto();
                t.Application = app;
                t.DataStructure = new List<MetaDataItemDto>();
                t.UIStructure = new List<MetaUIItemDto>();
                t.NoSeries = new List<NoSerieDto>();

                foreach (var item in ditems)
                {
                    if (item.AppMetaCode == app.MetaCode)
                    {
                        t.DataStructure.Add(item);
                        if (item.IsMetaTypeDataValue)
                        {
                            item.ColumnName = item.DbName;
                            if (item.IsRoot)
                                item.TableName = app.DbName;
                            else
                            {
                                var table = ditems.Find(p => p.MetaCode == item.ParentMetaCode && p.IsMetaTypeDataValueTable);
                                if (table != null)
                                    item.TableName = table.DbName;
                            }
                                

                        }
                    }
                }

                foreach (var item in noseries)
                {
                    if (item.AppMetaCode == app.MetaCode)
                    {
                        t.NoSeries.Add(item);
                    }
                }

                foreach (var item in uitems.OrderBy(p=> p.RowOrder).ThenBy(p=> p.ColumnOrder))
                {
                    if (item.AppMetaCode == app.MetaCode)
                    {
                        t.UIStructure.Add(item);
                        if (!string.IsNullOrEmpty(item.DataMetaCode))
                        {
                            var dinf = ditems.Find(p => p.MetaCode == item.DataMetaCode);
                            if (dinf != null)
                                item.DataInfo = dinf;
                        }

                        if (item.HasDataViewDomain && item.IsUITypeLookUp)
                        {
                            var vinf = views.Find(p => p.MetaCode == item.ViewName && p.IsRoot);
                            if (vinf != null)
                                item.ViewInfo = vinf;
                        }

                        if (item.HasDataViewDomain && item.IsUITypeLookUpKeyField)
                        {
                            var vinf = views.Find(p => p.MetaCode == item.ViewField && p.ParentMetaCode == item.ViewName && !p.IsRoot && !item.IsRoot);
                            if (vinf != null)
                                item.ViewInfo = vinf;
                        }

                        if (item.HasDataViewDomain && item.IsUITypeLookUpField)
                        {
                            var vinf = views.Find(p => p.MetaCode == item.ViewField && p.ParentMetaCode == item.ViewName && !p.IsRoot && !item.IsRoot);
                            if (vinf != null)
                                item.ViewInfo = vinf;
                        }
                    }
                }

                res.Add(t);
            }

            return res;

        }

        public List<MetaDataViewDto> GetMetaDataViews()
        {
            return MetaDataViews.Select(p => new MetaDataViewDto(p)).ToList();
        }

        public List<MetaMenuItemDto> GetMetaMenuItems()
        {
            var apps = AppDescription.Select(p => new ApplicationDescriptionDto(p)).ToList();
            var res = new List<MetaMenuItemDto>();
            foreach (var m in MetaMenuItems.OrderBy(p=> p.Order))
            {
                var s = new MetaMenuItemDto(m);

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

        public List<NoSerieDto> GetNewNoSeriesValues(int applicationid)
        {
            var res = new List<NoSerieDto>();

            var app = AppDescription.FirstOrDefault(p => p.Id == applicationid);
            if (app == null)
                return new List<NoSerieDto>();

            var appseries = NoSeries.Select(p => new NoSerieDto(p)).Where(p=> p.AppMetaCode == app.MetaCode).ToList();

            foreach (var item in appseries)
            {
                item.Application = new ApplicationDescriptionDto(app);

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

            context.SaveChanges();



            return res;

        }

        public List<NoSerieDto> GetNoSeries()
        {
            var res = NoSeries.Select(p => new NoSerieDto(p)).ToList();
            var ditems = MetaDBItem.Select(p => new MetaDataItemDto(p)).ToList();

            foreach (var item in res)
            {



                if (!string.IsNullOrEmpty(item.AppMetaCode))
                {
                    var app = AppDescription.FirstOrDefault(p => p.MetaCode == item.AppMetaCode);
                    if (app != null)
                    {
                        item.Application = new ApplicationDescriptionDto(app);
                        if (!string.IsNullOrEmpty(item.DataMetaCode))
                        {
                            var dinf = ditems.Find(p => p.MetaCode == item.DataMetaCode && p.AppMetaCode == app.MetaCode);
                            if (dinf != null)
                                item.DataInfo = dinf;
                        }
                    }
                }

            }

            return res;
        }

        public ApplicationDescriptionDto SaveApplication(ApplicationDescriptionDto model)
        {
            var res = new ApplicationDescriptionDto();

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

                var entity = new ApplicationDescription();
                entity.Id = max;
                entity.MetaCode = model.MetaCode;
                entity.Title = model.Title;
                entity.DbName = model.DbName;
                entity.Description = model.Description;

                AppDescription.Add(entity);

                CreateApplicationMenuItem(model);

                context.SaveChanges();

               

                return new ApplicationDescriptionDto(entity);

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

                var menuitem = MetaMenuItems.FirstOrDefault(p => p.AppMetaCode == entity.MetaCode && p.MetaType == "MENUITEM");
                if (menuitem != null)
                {
                    menuitem.Action = model.MainMenuItem.Action;
                    menuitem.Controller = model.MainMenuItem.Controller;
                    menuitem.Title = model.MainMenuItem.Title;
                }
                else
                {
                    CreateApplicationMenuItem(model);
                }

                context.SaveChanges();

                return new ApplicationDescriptionDto(entity);

            }

        }

        private void CreateApplicationMenuItem(ApplicationDescriptionDto model)
        {

            if (!string.IsNullOrEmpty(model.MainMenuItem.Title))
            {
                var max = 0;
                var menu = GetMetaMenuItems();
                var root = menu.Find(p => p.IsMetaTypeMainMenu);
                if (root == null)
                {
                    var main = new MetaMenuItem() { AppMetaCode = "", MetaType = "MAINMENU", Order = 1, ParentMetaCode = "ROOT", Properties = "", Title = "Applications" };
                    MetaMenuItems.Add(main);
                    max = 1;
                }
                else
                {
                    max = menu.Max(p => p.Order);
                }

                var appmi = new MetaMenuItem() { AppMetaCode = model.MetaCode, MetaType = "MENUITEM", Order = max + 10, ParentMetaCode = root.MetaCode, Properties = "", Title = model.MainMenuItem.Title };
                appmi.Action = model.MainMenuItem.Action;
                appmi.Controller = model.MainMenuItem.Controller;
                appmi.MetaCode = "MI_" + MetaModelDto.GetRandomUniqueString();
                MetaMenuItems.Add(appmi);

            }

        }

        public void SaveValueDomains(List<ValueDomainDto> model)
        {
           
            foreach (var vd in model)
            {
                if (!vd.IsValid)
                    throw new InvalidOperationException("Missing required information on value domain, can't save.");
                
                if (vd.Id < 1)
                {
                    ValueDomains.Add(new ValueDomain() { DomainName= vd.DomainName, Value = vd.Value, Code = vd.Code });
                }
                else
                {
                    var existing = ValueDomains.FirstOrDefault(p => p.Id == vd.Id);
                    if (existing != null)
                    {
                        existing.Code = vd.Code;
                        existing.Value = vd.Value;
                        existing.DomainName = vd.DomainName;
                    }

                }

            }

            context.SaveChanges();
        }

        public List<ValueDomainDto> GetValueDomains()
        {
            var t = ValueDomains.Select(p => new ValueDomainDto(p)).ToList();
            return t;
        }

        public void SaveApplicationUI(List<MetaUIItemDto> model)
        {

            foreach (var uic in model)
            {
                if (uic.Id < 1)
                {
                    MetaUIItem.Add(CreateMetaUIItem(uic));

                }
                else
                {
                    var existing = MetaUIItem.FirstOrDefault(p => p.Id == uic.Id);
                    if (existing != null)
                    {
                        existing.Title = uic.Title;
                        existing.RowOrder = uic.RowOrder;
                        existing.ColumnOrder = uic.ColumnOrder;
                        existing.DataMetaCode = uic.DataMetaCode;
                        existing.Domain = uic.Domain;
                        existing.MetaType = uic.MetaType;
                        existing.MetaCode = uic.MetaCode;
                        existing.Description = uic.Description;
                    }

                }

            }

            context.SaveChanges();
        }

        private MetaUIItem CreateMetaUIItem(MetaUIItemDto dto)
        {
            var res = new MetaUIItem() { AppMetaCode = dto.AppMetaCode, ColumnOrder = dto.ColumnOrder, DataMetaCode = dto.DataMetaCode,
                                         Description = dto.Description, Domain = dto.Domain, MetaCode = dto.MetaCode, MetaType = dto.MetaType,
                                         ParentMetaCode = dto.ParentMetaCode, RowOrder = dto.RowOrder, Title = dto.Title };

            return res;

        }

        public void DeleteApplicationUI(int id)
        {
            var existing = MetaUIItem.FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                var dto = new MetaUIItemDto(existing);
                if (dto.IsUITypeLookUp)
                {
                    var childlist = MetaUIItem.Where(p => (p.MetaType == "LOOKUPKEYFIELD" || p.MetaType == "LOOKUPFIELD") && p.ParentMetaCode == existing.MetaCode).ToList();
                    MetaUIItem.Remove(existing);
                    MetaUIItem.RemoveRange(childlist);
                }
                else
                {
                    MetaUIItem.Remove(existing);

                }
              
                context.SaveChanges();
            }
        }

        public void SaveApplicationDB(List<MetaDataItemDto> model, int applicationid)
        {

            var app = GetApplicationMeta().Find(p => p.Application.Id == applicationid);
            if (app == null)
                throw new InvalidOperationException("Could not find application when saving application db meta.");

            foreach (var dbi in model)
            {
                dbi.AppMetaCode = app.Application.MetaCode;

                //ASSUME ALL IS ROOT, CORRECT LATER
                dbi.ParentMetaCode = "ROOT";

                if (dbi.IsMetaTypeDataValue && string.IsNullOrEmpty(dbi.TableName))
                    throw new InvalidOperationException("Could not identify parent table when saving application db meta.");

                if (string.IsNullOrEmpty(dbi.MetaCode) && dbi.IsMetaTypeDataValueTable)
                    dbi.MetaCode = "TBL_" + MetaModelDto.GetRandomUniqueString();
                else if (string.IsNullOrEmpty(dbi.MetaCode) && dbi.IsMetaTypeDataValue)
                    dbi.MetaCode = "DVL_" + MetaModelDto.GetRandomUniqueString();

                if (dbi.IsMetaTypeDataValue && dbi.TableName == app.Application.DbName)
                {
                    dbi.ParentMetaCode = "ROOT";
                }

                if (!string.IsNullOrEmpty(dbi.DbName))
                    dbi.DbName = dbi.DbName.Replace(" ", "");

                if (!dbi.HasValidMetaType)
                    throw new InvalidOperationException("Invalid meta type when saving application db meta.");

                if (dbi.IsMetaTypeDataValue && !dbi.HasValidDataType)
                    throw new InvalidOperationException("Invalid datatype type when saving application db meta.");


            }

            foreach (var dbi in model)
            {
                if (dbi.Id < 1)
                {
                    var t = CreateMetaDataItem(dbi);


                    //SET PARENT META CODE
                    if (dbi.IsMetaTypeDataValue && dbi.TableName != app.Application.DbName)
                    {
                        var tbl = model.Find(p => p.IsMetaTypeDataValueTable && p.DbName == dbi.TableName);
                        if (tbl != null)
                            t.ParentMetaCode = tbl.MetaCode;
                    }

                    //Don't save main table, it's implicit for application
                    if (dbi.IsMetaTypeDataValueTable && dbi.DbName == app.Application.DbName)
                        continue;

                    MetaDBItem.Add(t);

                }
                else
                {
                    var existing = MetaDBItem.FirstOrDefault(p => p.Id == dbi.Id);
                    if (existing != null)
                    {
                        existing.DataType = dbi.DataType;
                        existing.Domain = dbi.Domain;
                        existing.MetaType = dbi.MetaType;
                        existing.MetaCode = dbi.MetaCode;
                        existing.Description = dbi.Description;
                        existing.ParentMetaCode = dbi.ParentMetaCode;
                        existing.DbName = dbi.DbName;
                        existing.Mandatory = dbi.Mandatory;
                       
                    }

                }

            }

            context.SaveChanges();
        
        }

        private MetaDataItem CreateMetaDataItem(MetaDataItemDto dto)
        {
            var res = new MetaDataItem()
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

        public void SaveDataView(List<MetaDataViewDto> model)
        {
            foreach (var dv in model)
            {

                if (dv.IsMetaTypeDataView)
                    dv.ParentMetaCode = "ROOT";

                if (string.IsNullOrEmpty(dv.MetaCode) && dv.IsMetaTypeDataView)
                    dv.MetaCode = "DV_" + MetaModelDto.GetRandomUniqueString();
                else if (string.IsNullOrEmpty(dv.MetaCode) && dv.IsMetaTypeDataViewKeyField)
                    dv.MetaCode = "DVKF_" + MetaModelDto.GetRandomUniqueString();
                else if (string.IsNullOrEmpty(dv.MetaCode) && dv.IsMetaTypeDataViewField)
                    dv.MetaCode = "DVLF_" + MetaModelDto.GetRandomUniqueString();

            }

            foreach (var dv in model)
            {
                if (dv.Id < 1)
                {
                    var t = CreateMetaDataView(dv);
                    MetaDataViews.Add(t);
                }
                else
                {
                    var existing = MetaDataViews.FirstOrDefault(p => p.Id == dv.Id);
                    if (existing != null)
                    {

                        existing.MetaType = dv.MetaType;
                        existing.MetaCode = dv.MetaCode;
                        existing.ParentMetaCode = dv.ParentMetaCode;
                        existing.SQLQuery = dv.SQLQuery;
                        existing.SQLQueryFieldName = dv.SQLQueryFieldName;
                        existing.Title = dv.Title;
                    }

                }

            }

            context.SaveChanges();
        }


        private MetaDataView CreateMetaDataView(MetaDataViewDto dto)
        {
            var res = new MetaDataView()
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

      

        public void DeleteApplicationDB(int id)
        {
            var existing = MetaDBItem.FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                var dto = new MetaDataItemDto(existing);
                var app = GetApplicationMeta().Find(p => p.Application.MetaCode == dto.AppMetaCode);
                if (app == null)
                    return;

                if (dto.IsMetaTypeDataValueTable && dto.DbName != app.Application.DbName)
                {
                    var childlist = MetaDBItem.Where(p => (p.MetaType == "DATAVALUE") && p.ParentMetaCode == existing.MetaCode).ToList();
                    MetaDBItem.Remove(existing);
                    MetaDBItem.RemoveRange(childlist);
                }
                else
                {
                    MetaDBItem.Remove(existing);
                }
                context.SaveChanges();
            }
        }

        public void DeleteDataView(int id)
        {
            var existing = MetaDataViews.FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                var dto = new MetaDataViewDto(existing);
                if (dto.IsMetaTypeDataView)
                {
                    var childlist = MetaDataViews.Where(p => (p.MetaType == "DATAVIEWFIELD" || p.MetaType == "DATAVIEWKEYFIELD") && p.ParentMetaCode == existing.MetaCode).ToList();
                    MetaDataViews.Remove(existing);
                    MetaDataViews.RemoveRange(childlist);
                }
                else
                {
                    MetaDataViews.Remove(existing);
                }
                context.SaveChanges();
            }
        }

    
    }

}
