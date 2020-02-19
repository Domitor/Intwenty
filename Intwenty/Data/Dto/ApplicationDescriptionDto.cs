using Moley.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Moley.Data.Dto
{

   public class ApplicationDescriptionDto
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
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string MetaCode { get; set; }

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

       


    }
    
}
