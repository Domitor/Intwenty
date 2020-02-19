
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Moley.Data.Entity;
using Moley.Data.Dto;
using System;

namespace Moley.Data
{

    public interface ISystemRepository
    {
        List<ApplicationDescriptionDto> GetApplicationDescriptions();

        List<MetaDataItemDto> GetMetaDataItems();

        List<MetaUIItemDto> GetMetaUIItems();

        List<MetaDataViewDto> GetMetaDataViews();

        List<ApplicationDto> GetApplicationMeta();

        List<MetaMenuItemDto> GetMetaMenuItems();

        List<NoSerieDto> GetNewNoSeriesValues(int applicationid );


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


        public SystemRepository(ApplicationDbContext context)
        {
            this.context = context;
            AppDescription = context.Set<ApplicationDescription>();
            MetaDBItem = context.Set<MetaDataItem>();
            MetaUIItem = context.Set<MetaUIItem>();
            MetaDataViews = context.Set<MetaDataView>();
            MetaMenuItems = context.Set<MetaMenuItem>();
            NoSeries = context.Set<NoSerie>();
        }

        public List<ApplicationDescriptionDto> GetApplicationDescriptions()
        {
            var t = AppDescription.Select(p=> new ApplicationDescriptionDto(p)).ToList();
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
            var apps = AppDescription.Select(p => new ApplicationDescriptionDto(p)).ToList();
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
                t.ViewStructure = new List<MetaDataViewDto>();
                t.NoSeries = new List<NoSerieDto>();

                foreach (var item in ditems)
                {
                    if (item.AppMetaCode == app.MetaCode)
                    {
                        t.DataStructure.Add(item);
                    }
                }

                foreach (var item in noseries)
                {
                    if (item.AppMetaCode == app.MetaCode)
                    {
                        t.NoSeries.Add(item);
                    }
                }

                foreach (var item in uitems)
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
            foreach (var m in MetaMenuItems)
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
    }
}
