using Intwenty.Entity;
using Intwenty.Interface;
using System.Collections.Generic;


namespace Intwenty.Model
{


    public class EndpointModelItem : BaseModelItem
    {
        //META TYPES
        public static readonly string MetaTypeTableGetById = "TABLEGETBYID";
        public static readonly string MetaTypeTableGetAll = "TABLEGETALL";
        public static readonly string MetaTypeTableSave = "TABLESAVE";
        public static readonly string MetaTypeDataViewGetData = "DATAVIEWGETDATA";
        
        public EndpointModelItem()
        {
            SetEmptyStrings();
        }

        public EndpointModelItem(string metatype)
        {
            MetaType = metatype;
            SetEmptyStrings();
        }

        public EndpointModelItem(EndpointItem entity)
        {
            Id = entity.Id;
            MetaType = entity.MetaType;
            Description = entity.Description;
            AppMetaCode = entity.AppMetaCode;
            MetaCode = entity.MetaCode;
            ParentMetaCode = entity.ParentMetaCode;
            Properties = entity.Properties;
            Path = entity.Path;
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
            if (string.IsNullOrEmpty(DataMetaCode)) DataMetaCode = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
        }

        public string AppMetaCode { get; set; }

        public string Path { get; set; }

        public string Description { get; set; }

        public string DataMetaCode { get; set; }

        public int OrderNo { get; set; }

        public DatabaseModelItem DataTableInfo { get; set; }

        public DataViewModelItem DataViewInfo { get; set; }

        public string Action 
        {
            get {

                if (IsMetaTypeDataViewGetData)
                    return "GetData";
                if (IsMetaTypeTableGetAll)
                    return "GetAll";
                if (IsMetaTypeTableGetById)
                    return "GetById";
                if (IsMetaTypeTableSave)
                    return "Save";

                return string.Empty;

            }
        }

        public override string ModelCode
        {
            get { return "ENDPOINTMODEL"; }
        }


        public static List<IntwentyMetaType> GetAvailableMetaTypes()
        {
            return new List<IntwentyMetaType>()
            {
                new IntwentyMetaType(){ Code=MetaTypeTableGetById, Title="Table - GetById" },
                new IntwentyMetaType(){ Code=MetaTypeTableGetAll,  Title="Table - GetAll" },
                new IntwentyMetaType(){ Code=MetaTypeTableSave,    Title="Table - Save" },
                new IntwentyMetaType(){ Code=MetaTypeDataViewGetData, Title="Intwenty Data View - GetData" },

            };
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

        public bool IsDataTableConnected
        {
            get { return (DataTableInfo != null && !string.IsNullOrEmpty(DataMetaCode) && DataTableInfo.IsMetaTypeDataTable); }
        }

     

        public bool IsDataViewConnected
        {
            get { return (DataViewInfo != null && !string.IsNullOrEmpty(DataMetaCode) && DataViewInfo.IsMetaTypeDataView); }
        }


        public bool IsMetaTypeTableGetById
        {
            get { return MetaType == MetaTypeTableGetById; }
        }

        public bool IsMetaTypeTableGetAll
        {
            get { return MetaType == MetaTypeTableGetAll; }
        }

        public bool IsMetaTypeTableSave
        {
            get { return MetaType == MetaTypeTableSave; }
        }

        public bool IsMetaTypeDataViewGetData
        {
            get { return MetaType == MetaTypeDataViewGetData; }
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
