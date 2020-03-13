
using Intwenty.Data.Entity;
using System.Collections.Generic;

namespace Intwenty.MetaDataService.Model
{
    public class DataViewModelItem : BaseModelItem
    {
        //META TYPES
        public static readonly string MetaTypeDataView = "DATAVIEW";
        public static readonly string MetaTypeDataViewField = "DATAVIEWFIELD";
        public static readonly string MetaTypeDataViewKeyField = "DATAVIEWKEYFIELD";


        public DataViewModelItem()
        {
        }

        public DataViewModelItem(string metatype)
        {
            MetaType = metatype;
        }


        public DataViewModelItem(Data.Entity.DataViewItem entity)
        {
            Id = entity.Id;
            SQLQuery = entity.SQLQuery;
            SQLQueryFieldName = entity.SQLQueryFieldName;
            MetaType = entity.MetaType;
            Title = entity.Title;
            MetaCode = entity.MetaCode;
            ParentMetaCode = entity.ParentMetaCode;
            Properties = "";
        }


        public int Id { get; set; }

        public string Title { get; set; }

        public string SQLQuery { get; set; }

        public string SQLQueryFieldName { get; set; }

       

      

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
                t.Add(MetaTypeDataView);
                t.Add(MetaTypeDataViewField);
                t.Add(MetaTypeDataViewKeyField);
                return t;
            }
        }

        public bool IsMetaTypeDataView
        {
            get { return MetaType == MetaTypeDataView; }
        }

        public bool IsMetaTypeDataViewField
        {
            get { return MetaType == MetaTypeDataViewField; }
        }


        public bool IsMetaTypeDataViewKeyField
        {
            get { return MetaType == MetaTypeDataViewKeyField; }
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

      

      
    }

  
}
