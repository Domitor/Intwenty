using Intwenty.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Intwenty.MetaDataService.Model
{

   public class ApplicationModelItem : BaseModelItem
    {
        public static readonly string MetaTypeApplication = "APPLICATION";

        public ApplicationModelItem()
        {
            MainMenuItem = new MenuModelItem(MenuModelItem.MetaTypeMenuItem);
            SetEmptyStrings();
        }

        public ApplicationModelItem(Data.Entity.ApplicationDescription entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            Description = entity.Description;
            MetaCode = entity.MetaCode;
            DbName = entity.DbName;
            UseVersioning = entity.UseVersioning;
            IsHierarchicalApplication = entity.IsHierarchicalApplication;
            TestDataAmount = entity.TestDataAmount;
            MetaType = MetaTypeApplication;
            ParentMetaCode = BaseModelItem.MetaTypeRoot;
            MainMenuItem = new MenuModelItem(MenuModelItem.MetaTypeMenuItem);
            SetEmptyStrings();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(Description)) Description = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(ParentMetaCode)) ParentMetaCode = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string DbName { get; set; }

        public bool IsHierarchicalApplication { get; set; }

        public bool UseVersioning { get; set; }

        [Obsolete]
        public bool UseApplicationVersioning { get; set; }

        [Obsolete]
        public bool UseApplicationValueVersioning { get; set; }

        [Obsolete]
        public bool UseRowVersioning { get; set; }

        [Obsolete]
        public bool UseRowValueVersioning { get; set; }

        public int TestDataAmount { get; set; }

        public MenuModelItem MainMenuItem { get; set; }

        public string MainTableName
        {
            get { return this.DbName;  }
        }

        public string VersioningTableName
        {
            get { return this.DbName + "_Versioning"; }
        }


        public override bool HasValidMetaType
        {
            get { return true; }
        }

        public static List<string> ValidProperties
        {
            get
            {
                var t = new List<string>();
                return t;
            }
        }

        public static List<string> ValidMetaTypes
        {
            get
            {
                var t = new List<string>();
                t.Add(MetaTypeApplication);
                return t;
            }
        }



    }
    
}
