using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Intwenty.MetaDataService.Model
{

    public class ApplicationModel
    {
        public ApplicationModel()
        {

        }

        /// <summary>
        /// Describes an application
        /// </summary>
        public ApplicationModelItem Application { get; set; }

        /// <summary>
        /// Describes the database for this application
        /// </summary>
        public List<DatabaseModelItem> DataStructure { get; set; }

        /// <summary>
        /// Describes the UI for this application
        /// </summary>
        public List<UserInterfaceModelItem> UIStructure { get; set; }

        /// <summary>
        /// Describes the data views for this application
        /// </summary>
        //public List<MetaDataViewDto> ViewStructure { get; set; }

        /// <summary>
        /// No Series used in this application
        /// </summary>
        public List<NoSerieModelItem> NoSeries { get; set; }

    }

}
