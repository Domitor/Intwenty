using Intwenty.Interface;
using Intwenty.Model.UIRendering;
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

        public SystemModelItem System { get; set; }

        /// <summary>
        /// Describes an application
        /// </summary>
        public ApplicationModelItem Application { get; set; }

        /// <summary>
        /// Describes the database for this application
        /// </summary>
        public List<DatabaseModelItem> DataStructure { get; set; }


        public List<ViewModel> Views { get; set; }

        public List<string> GetDomainReferences()
        {
            var res = new List<string>();

            foreach (var v in Views)
            {
                foreach (var ui in v.UserInterface)
                {
                    foreach (var uiitem in ui.UIStructure)
                    {

                        if (uiitem.HasValueDomain)
                            res.Add(uiitem.Domain);
                    }

                }

            }

            return res;
        }

        public UserInterfaceModelItem GetUserInterface(string metacode)
        {
           
            foreach (var v in Views)
            {
                foreach (var ui in v.UserInterface)
                {
                    if (ui.MetaCode == metacode)
                        return ui;

                }

            }

            return null;
        }


    }

}
