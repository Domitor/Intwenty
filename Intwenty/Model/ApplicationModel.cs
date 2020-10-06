using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Intwenty.Model
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



    }

}
