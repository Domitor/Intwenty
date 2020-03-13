using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Intwenty.Data.Entity
{

   public class ApplicationItem
   {
        public ApplicationItem()
        {

        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Alphanumeric ID
        /// </summary>
        public string MetaCode { get; set; }

        public string DbName { get; set; }

        public bool IsHierarchicalApplication { get; set; }

        /// <summary>
        /// Don't update data, instead create and keep track of new versions.
        /// </summary>
        public bool UseVersioning { get; set; }

        /// <summary>
        /// The number of apps of this type to generate testdata for
        /// </summary>
        public int TestDataAmount { get; set; }

        /// <summary>
        /// This variable is used to tell if versioning should occur in the
        /// application main table. Eg. that a new application row with a new
        /// versionno should be created everytime atleast one single value
        /// in the application or in any subtable is changed.
        /// (This means that a new version of the application is created on every
        /// call to application.save, if anything under the application is saved, eg.changed)
        /// </summary>
        //public bool UseApplicationVersioning { get; set; }

        /// <summary>
        /// This variable is used to tell if versioning should occur in the
        /// DATAVALUE level. Eg. That a new trans row with a new
        /// versionno for the value should be created everytime a value
        /// in an application is changed.
        /// (This means that a new version of every value is created on every
        /// call to application.save, if any of them values in the the application is changed)
        //public bool UseApplicationValueVersioning { get; set; }

        /// <summary>
        /// This variable is used to tell if versioning should occur in the
        /// DATAVALUETABLEROW level. Eg. That a new table row with a new
        /// versionno should be created everytime atleast one single value
        /// in a tablerow is changed
        /// (This means that a new version of the row is created on every
        /// call to row.save, if anything under the row is saved, eg. changed)
        /// </summary>
        //public bool UseRowVersioning { get; set; }

        /// <summary>
        /// This variable is used to tell if versioning should occur in the
        /// DATAVALUE level. Eg. That a new trans row with a new
        /// versionno for the value should be created everytime a value
        /// in a tablerow is changed.
        /// (This means that a new version of every value is created on every
        /// call to row.save, if any of them values in the the row is changed)
        //public bool UseRowValueVersioning { get; set; }


    }

    public class ApplicationItemMap
    {
        public ApplicationItemMap(EntityTypeBuilder<ApplicationItem> entityBuilder)
        {

        }
    }

}
