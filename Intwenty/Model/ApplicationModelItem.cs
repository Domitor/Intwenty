using Intwenty.Data.Entity;
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
            MainMenuItem = new MenuModelItem(MenuModelItem.MetaTypeMenuItem);
            SetEmptyStrings();
        }

        public ApplicationModelItem(Data.Entity.ApplicationItem entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            TitleLocalizationKey = entity.TitleLocalizationKey;
            Description = entity.Description;
            MetaCode = entity.MetaCode;
            DbName = entity.DbName;
            UseVersioning = entity.UseVersioning;
            IsHierarchicalApplication = entity.IsHierarchicalApplication;
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
            if (string.IsNullOrEmpty(TitleLocalizationKey)) TitleLocalizationKey = string.Empty;
        }


        public string TitleLocalizationKey { get; set; }

        public string Description { get; set; }

        public string DbName { get; set; }

        public bool IsHierarchicalApplication { get; set; }

        public bool UseVersioning { get; set; }


        public MenuModelItem MainMenuItem { get; set; }

        public string VersioningTableName
        {
            get { return this.DbName + "_Versioning"; }
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
                    if (!GetAvaliableProperties().Exists(p => p.CodeName == prop))
                        return false;
                }
                return true;
            }
        }

        public static List<IntwentyProperty> GetAvaliableProperties()
        {
            return new List<IntwentyProperty>();
        }

        public static List<IntwentyMetaType> GetAvailableMetaTypes()
        {
            return new List<IntwentyMetaType>()
            {
                new IntwentyMetaType(){ Code=MetaTypeApplication, Title="Application" }

            };
        }



    }
    
}
