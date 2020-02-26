using Moley.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Moley.Data.Dto
{

   public class ApplicationDescriptionDto : MetaModelDto
    {
        public ApplicationDescriptionDto()
        {
           
        }

        public ApplicationDescriptionDto(ApplicationDescription entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            Description = entity.Description;
            MetaCode = entity.MetaCode;
            DbName = entity.DbName;
            UseVersioning = entity.UseVersioning;
            IsHierarchicalApplication = entity.IsHierarchicalApplication;
            TestDataAmount = entity.TestDataAmount;
            MetaType = "APPLICATION";
            ParentMetaCode = "ROOT";
            Properties = "";
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

        protected override List<string> ValidProperties
        {
            get
            {
                var t = new List<string>();
                return t;
            }
        }

        public override List<string> ValidMetaTypes
        {
            get
            {
                var t = new List<string>();
                t.Add("APPLICATION");
                return t;
            }
        }



    }
    
}
