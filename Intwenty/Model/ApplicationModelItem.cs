using Intwenty.Entity;
using Intwenty.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Intwenty.Model
{

   public class ApplicationModelItem : BaseModelItem, ILocalizableTitle
   {
        public static readonly string MetaTypeApplication = "APPLICATION";

        public ApplicationModelItem()
        {
            SetEmptyStrings();
        }

        public ApplicationModelItem(ApplicationItem entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            TitleLocalizationKey = entity.TitleLocalizationKey;
            Description = entity.Description;
            MetaCode = entity.MetaCode;
            DbName = entity.DbName;
            UseVersioning = entity.UseVersioning;
            IsHierarchicalApplication = entity.IsHierarchicalApplication;
            SystemMetaCode = entity.SystemMetaCode;
            MetaType = MetaTypeApplication;
            ParentMetaCode = BaseModelItem.MetaTypeRoot;
            CreateViewRequirement = entity.CreateViewRequirement;
            EditViewRequirement = entity.EditViewRequirement;
            EditListViewRequirement = entity.EditListViewRequirement;
            DetailViewRequirement = entity.DetailViewRequirement;
            ListViewRequirement = entity.ListViewRequirement;
            ApplicationPath = entity.ApplicationPath;
            SetEmptyStrings();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(Description)) Description = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(ParentMetaCode)) ParentMetaCode = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(TitleLocalizationKey)) TitleLocalizationKey = string.Empty;
            if (string.IsNullOrEmpty(SystemMetaCode)) TitleLocalizationKey = string.Empty;
            if (string.IsNullOrEmpty(CreateViewRequirement)) CreateViewRequirement = string.Empty;
            if (string.IsNullOrEmpty(EditViewRequirement)) EditViewRequirement = string.Empty;
            if (string.IsNullOrEmpty(EditListViewRequirement)) EditListViewRequirement = string.Empty;
            if (string.IsNullOrEmpty(DetailViewRequirement)) DetailViewRequirement = string.Empty;
            if (string.IsNullOrEmpty(ListViewRequirement)) ListViewRequirement = string.Empty;
            if (string.IsNullOrEmpty(ApplicationPath)) ApplicationPath = string.Empty;
        }

        public SystemModelItem SystemInfo { get; set; }

        public string SystemMetaCode { get; set; }

        public string CreateViewRequirement { get; set; }

        public string EditViewRequirement { get; set; }

        public string EditListViewRequirement { get; set; }

        public string DetailViewRequirement { get; set; }

        public string ListViewRequirement { get; set; }

        public string ApplicationPath { get; set; }

        public string TitleLocalizationKey { get; set; }

        public string Description { get; set; }

        public string DbName { get; set; }

        public bool IsHierarchicalApplication { get; set; }

        public bool UseVersioning { get; set; }

        public string VersioningTableName
        {
            get { return this.DbName + "_Versioning"; }
        }

        public override string ModelCode
        {
            get { return "APPMODEL"; }
        }

        public override bool HasValidMetaType
        {
            get 
            {
                return this.MetaType == MetaTypeApplication; 
            }
        }

        public override bool HasValidProperties
        {
            get
            {
                foreach (var prop in GetProperties())
                {
                    if (!IntwentyRegistry.IntwentyProperties.Exists(p => p.CodeName == prop && p.ValidFor.Contains(MetaType)))
                        return false;
                }
                return true;
            }
        }


        public bool HasSystemInfo
        {
            get
            {
                return this.SystemInfo != null;
            }

        }










    }
    
}
