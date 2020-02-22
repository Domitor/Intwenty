
using Moley.Data.Entity;
using System.Collections.Generic;

namespace Moley.Data.Dto
{
    public class MetaDataViewDto
    {

        public int Id { get; set; }

        public string MetaType { get; set; }

        public string Title { get; set; }

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public string SQLQuery { get; set; }

        public string SQLQueryFieldName { get; set; }

        public MetaDataViewDto(MetaDataView entity)
        {
            Id = entity.Id;
            SQLQuery = entity.SQLQuery;
            SQLQueryFieldName = entity.SQLQueryFieldName;
            MetaType = entity.MetaType;
            Title = entity.Title;
            MetaCode = entity.MetaCode;
            ParentMetaCode = entity.ParentMetaCode;
        }

        public MetaDataViewDto()
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

        public bool HasValidMetaType
        {
            get
            {
                if (string.IsNullOrEmpty(MetaType))
                    return false;

                if (MetaTypes.Contains(MetaType))
                    return true;

                return false;

            }
        }



        public List<string> MetaTypes
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

        public bool IsRoot
        {
            get { return this.ParentMetaCode == "" || this.ParentMetaCode == "ROOT"; }
        }

    }

  
}
