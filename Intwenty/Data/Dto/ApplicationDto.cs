using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Moley.Data.Dto
{

    public class ApplicationDto
    {
        public ApplicationDto()
        {

        }

        /// <summary>
        /// Describes an application
        /// </summary>
        public ApplicationDescriptionDto Application { get; set; }

        /// <summary>
        /// Describes the database for this application
        /// </summary>
        public List<MetaDataItemDto> DataStructure { get; set; }

        /// <summary>
        /// Describes the UI for this application
        /// </summary>
        public List<MetaUIItemDto> UIStructure { get; set; }

        /// <summary>
        /// Describes the data views for this application
        /// </summary>
        //public List<MetaDataViewDto> ViewStructure { get; set; }

        /// <summary>
        /// No Series used in this application
        /// </summary>
        public List<NoSerieDto> NoSeries { get; set; }

    }

}
