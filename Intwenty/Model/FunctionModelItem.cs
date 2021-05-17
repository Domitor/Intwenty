using Intwenty.Areas.Identity.Models;
using Intwenty.Entity;
using Intwenty.Helpers;
using Intwenty.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Model
{


    public class FunctionModelItem : BaseModelItem, ILocalizableTitle
    {
        //META TYPES
        public static readonly string MetaTypeCreate = "CREATE";
        public static readonly string MetaTypeEdit = "EDIT";
        public static readonly string MetaTypeNavigate = "NAVIGATE";
        public static readonly string MetaTypeSave = "SAVE";
        public static readonly string MetaTypeDelete = "DELETE";
        public static readonly string MetaTypeExport = "EXPORT";
        public static readonly string MetaTypeFilter = "FILTER";
        public static readonly string MetaTypePaging = "PAGING";


        public FunctionModelItem()
        {
            SetEmptyStrings();
        }

        public FunctionModelItem(FunctionItem entity)
        {
            Id = entity.Id;
            MetaType = entity.MetaType;
            Title = entity.Title;
            LocalizedTitle = entity.Title;
            TitleLocalizationKey = entity.TitleLocalizationKey;
            SystemMetaCode = entity.SystemMetaCode;
            AppMetaCode = entity.AppMetaCode;
            MetaCode = entity.MetaCode;
            ParentMetaCode = "ROOT";
            Properties = entity.Properties;
            SystemMetaCode = entity.SystemMetaCode;
            ActionPath = entity.ActionPath;         
            ActionMetaCode = entity.ActionMetaCode;
            ActionMetaType = entity.ActionMetaType;
            OwnerMetaType = entity.OwnerMetaType;
            OwnerMetaCode = entity.OwnerMetaCode;
            IsModalAction = entity.IsModalAction;
            SetEmptyStrings();
            ConfigurePath();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(ParentMetaCode)) ParentMetaCode = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(LocalizedTitle)) LocalizedTitle = string.Empty;
            if (string.IsNullOrEmpty(TitleLocalizationKey)) TitleLocalizationKey = string.Empty;
            if (string.IsNullOrEmpty(SystemMetaCode)) SystemMetaCode = string.Empty;
            if (string.IsNullOrEmpty(ActionPath)) ActionPath = string.Empty;
            if (string.IsNullOrEmpty(ActionMetaCode)) ActionMetaCode = string.Empty;
            if (string.IsNullOrEmpty(ActionMetaType)) ActionMetaType = string.Empty;
            if (string.IsNullOrEmpty(OwnerMetaCode)) OwnerMetaCode = string.Empty;
            if (string.IsNullOrEmpty(OwnerMetaType)) OwnerMetaType = string.Empty;

        }

        private void ConfigurePath()
        {
            if (!string.IsNullOrEmpty(ActionPath))
            {
                if (!ActionPath.StartsWith("/"))
                    ActionPath = "/" + ActionPath;
            }
        }

        public ApplicationModelItem ApplicationInfo { get; set; }
        public SystemModelItem SystemInfo { get; set; }
        public string SystemMetaCode { get; set; }
        public string TitleLocalizationKey { get; set; }
        public string AppMetaCode { get; set; }
        public string OwnerMetaType { get; set; }
        public string OwnerMetaCode { get; set; }

       
        public string ActionMetaCode { get; set; }
        public string ActionMetaType { get; set; }
        public string ActionPath { get; set; }
        public int ActionViewId { get; set; }
        public bool IsModalAction { get; set; }

        public override string ModelCode
        {
            get { return "FUNCTIONMODEL"; }
        }


        public override bool HasValidMetaType
        {
            get
            {
                if (string.IsNullOrEmpty(MetaType))
                    return false;


                if (!IntwentyRegistry.IntwentyMetaTypes.Exists(p => p.Code == MetaType && p.ModelCode == ModelCode))
                    return false;

                return true;

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

        public bool IsMetaTypePaging
        {
            get { return MetaType == MetaTypePaging; }
        }
        public bool IsMetaTypeFilter
        {
            get { return MetaType == MetaTypeFilter; }
        }
        public bool IsMetaTypeCreate
        {
            get { return MetaType == MetaTypeCreate; }
        }
        public bool IsMetaTypeEdit
        {
            get { return MetaType == MetaTypeEdit; }
        }

        public bool IsMetaTypeNavigate
        {
            get { return MetaType == MetaTypeNavigate; }
        }


        public bool IsMetaTypeDelete
        {
            get { return MetaType == MetaTypeDelete; }
        }


        public bool IsMetaTypeSave
        {
            get { return MetaType == MetaTypeSave; }
        }

        public bool IsMetaTypeExport
        {
            get { return MetaType == MetaTypeExport; }
        }

       

        public bool HasSystemInfo
        {
            get
            {
                return this.SystemInfo != null;
            }

        }

        public bool HasApplicationInfo
        {
            get
            {
                return this.ApplicationInfo != null;
            }

        }

        public string BuildRuntimeURI(string requestinfo, int id = 0)
        {
            if (IsModalAction)
                return string.Empty;

            string result = string.Empty;
            if (IsMetaTypeSave)
            {
                var aftersaveaction = GetPropertyValue("AFTERSAVEACTION");
                if (aftersaveaction == "GOTOVIEW")
                {
                    result = GetPropertyValue("GOTOVIEWPATH");
                }
            }
            else
            {
                result = ActionPath;
            }


            if (result.Contains("{id}"))
            {
                result = result.Replace("{id}", Convert.ToString(id));
            }

            if (result.Contains("{requestinfo}"))
            {
                var updated_requestinfo = BuildRuntimeRequestInfo(requestinfo);
                result = result.Replace("{requestinfo}", updated_requestinfo);
            }

            return result;


        }

        public string BuildRuntimeRequestInfo(string requestinfo)
        {
            
            string result = string.Empty;

            if (ActionViewId > 0)
            {
                var tmp_requestinfo = string.Empty;
                var hp = new HashTagPropertyObject();
                if (!string.IsNullOrEmpty(requestinfo))
                {
                    tmp_requestinfo = requestinfo.B64UrlDecode();
                    hp.Properties = tmp_requestinfo;
                }
                hp.AddUpdateProperty("VIEWID", Convert.ToString(ActionViewId));
                result = hp.Properties.B64UrlEncode();
            }
            else
            {
                result = requestinfo;
            }

            
            return result;

        }



    }

}
