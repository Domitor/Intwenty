using Intwenty.Data.Entity;
using Intwenty.Interface;
using System.Collections.Generic;


namespace Intwenty.Model
{


    public class EndpointModelItem : BaseModelItem
    {
        //META TYPES
        public static readonly string MetaTypeAPI = "API";
        public static readonly string MetaTypeTableOperation = "TABLEOPERATION";
        public static readonly string MetaTypeDataViewOperation = "DATAVIEWOPERATION";
        
        public EndpointModelItem()
        {
            SetEmptyStrings();
        }

        public EndpointModelItem(string metatype)
        {
            MetaType = metatype;
            SetEmptyStrings();
        }

        public EndpointModelItem(Data.Entity.EndpointItem entity)
        {
            Id = entity.Id;
            MetaType = entity.MetaType;
            Title = entity.Title;
            Description = entity.Description;
            AppMetaCode = entity.AppMetaCode;
            MetaCode = entity.MetaCode;
            ParentMetaCode = entity.ParentMetaCode;
            Properties = entity.Properties;
            Path = entity.Path;
            Action = entity.Action;
            DataMetaCode = entity.DataMetaCode;
            OrderNo = entity.OrderNo;
            Title = entity.Title;
            

            SetEmptyStrings();
        }

        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(Description)) Description = string.Empty;
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(ParentMetaCode)) ParentMetaCode = string.Empty;
            if (string.IsNullOrEmpty(Path)) Path = string.Empty;
            if (string.IsNullOrEmpty(Action)) Action = string.Empty;
            if (string.IsNullOrEmpty(DataMetaCode)) DataMetaCode = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
        }

        public string AppMetaCode { get; set; }

        public string Path { get; set; }

        public string Action { get; set; }

        public string Description { get; set; }

        public string DataMetaCode { get; set; }

        public int OrderNo { get; set; }

        public DatabaseModelItem DataTableInfo { get; set; }

        public DataViewModelItem DataViewInfo { get; set; }

     

        public List<string> ValidProperties
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
                t.Add(MetaTypeAPI);
                t.Add(MetaTypeDataViewOperation);
                t.Add(MetaTypeTableOperation);
              

                return t;
            }
        }

        public bool IsDataTableConnected
        {
            get { return IsMetaTypeTableOperation && (DataTableInfo != null && !string.IsNullOrEmpty(DataMetaCode) && DataTableInfo.IsMetaTypeDataTable); }
        }

     

        public bool IsDataViewConnected
        {
            get { return IsMetaTypeDataViewOperation && (DataViewInfo != null && !string.IsNullOrEmpty(DataMetaCode) && DataViewInfo.IsMetaTypeDataView); }
        }


        public bool IsMetaTypeApi
        {
            get { return MetaType == MetaTypeAPI; }
        }


        public bool IsMetaTypeTableOperation
        {
            get { return MetaType == MetaTypeTableOperation; }
        }

        public bool IsMetaTypeDataViewOperation
        {
            get { return MetaType == MetaTypeDataViewOperation; }
        }


        public override bool HasValidMetaType
        {
            get
            {
               if (string.IsNullOrEmpty(MetaType))
                   return false;

               if (ValidMetaTypes.Contains(MetaType))
                   return true;

                return false;

            }
        }

        public override bool HasValidProperties
        {
            get
            {
                foreach (var s in GetProperties())
                {
                    if (!ValidProperties.Exists(p => p == s))
                        return false;
                }

                return true;
            }
        }

        public string UIId
        {
            get { return MetaCode; }
        }


        public string DataTableDbName
        {
            get 
            { 
                if (IsDataTableConnected)
                    return DataTableInfo.DbName;

                return string.Empty;
            }
        }

    }

}
