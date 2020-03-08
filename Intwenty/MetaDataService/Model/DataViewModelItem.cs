
using Intwenty.Data.Entity;
using System.Collections.Generic;

namespace Intwenty.MetaDataService.Model
{
    public class DataViewModelItem : BaseModelItem
    {

        public int Id { get; set; }

        public string Title { get; set; }

        public string SQLQuery { get; set; }

        public string SQLQueryFieldName { get; set; }

        public DataViewModelItem(Data.Entity.MetaDataView entity)
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

        public DataViewModelItem(string metatype)
        {
            MetaType = metatype;
        }

        public DataViewModelItem()
        {
        }

        public string MetaTypeDataView
        {
            get { return "DATAVIEW"; }
        }

        public bool IsMetaTypeDataView
        {
            get { return MetaType == MetaTypeDataView; }
        }

        public string MetaTypeDataViewField
        {
            get { return "DATAVIEWFIELD"; }
        }

        public bool IsMetaTypeDataViewField
        {
            get { return MetaType == MetaTypeDataViewField; }
        }

        public string MetaTypeDataViewKeyField
        {
            get { return "DATAVIEWKEYFIELD"; }
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

        protected override List<string> ValidProperties
        {
            get
            {
                var t = new List<string>();
                return t;
            }
        }

        public List<string> ValidMetaTypes
        {
            get
            {
                var t = new List<string>();
                t.Add("DATAVIEW");
                t.Add("DATAVIEWKEYFIELD");
                t.Add("DATAVIEWFIELD");
                return t;
            }
        }

      
    }

  
}
