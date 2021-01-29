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

       
        public bool HasListView
        {
            get
            {
                return false;
            }

        }

        public bool UseCreateViewAuthorization
        {
            get
            {
                if (Application.CreateViewRequirement != "PUB")
                    return true;

                return false;
            }
        }

        public bool UseEditViewAuthorization
        {
            get
            {
                if (Application.EditViewRequirement != "PUB")
                    return true;

                return false;
            }
        }


        public bool UseEditListViewAuthorization
        {
            get
            {
                if (Application.EditListViewRequirement != "PUB")
                    return true;

                return false;
            }
        }

        public bool UseDetailViewAuthorization
        {
            get
            {
                if (Application.DetailViewRequirement != "PUB")
                    return true;

                return false;
            }
        }

        public bool UseListViewAuthorization
        {
            get
            {
                if (Application.ListViewRequirement != "PUB")
                    return true;

                return false;
            }
        }

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


        /*
        /// <summary>
        /// All UI model items, describing the UI for this application
        /// </summary>
        public List<UserInterfaceModelItem> UIStructure { get; set; }

        /// <summary>
        /// UIView models to use when rendering html
        /// </summary>
        public List<UIView> UIViews 
        {
            get { return GetUIViews(); }
        }

        public bool HasCreateView
        {
            get 
            { 
                return GetUIViews().Exists(p=> p.IsMetaTypeCreateView); 
            }

        }

        public bool HasEditView
        {
            get
            {
                return GetUIViews().Exists(p => p.IsMetaTypeEditView);
            }

        }

        public bool HasEditListView
        {
            get
            {
                return GetUIViews().Exists(p => p.IsMetaTypeEditListView);
            }

        }

        public bool HasDetailView
        {
            get
            {
                return GetUIViews().Exists(p => p.IsMetaTypeDetailView);
            }

        }

        public bool HasListView
        {
            get
            {
                return GetUIViews().Exists(p => p.IsMetaTypeListView);
            }

        }

        

        private List<UIView> GetUIViews()
        {
            if (UIStructure == null)
                return new List<UIView>();
            if (UIStructure.Count == 0)
                return new List<UIView>();

            var res = new List<UIView>();

            foreach (var v in UIStructure.Where(p => p.IsUIViewType))
            {
                var uiview = new UIView();
                uiview.Title = v.Title;
                uiview.LocalizedTitle = v.LocalizedTitle;
                uiview.TitleLocalizationKey = v.TitleLocalizationKey;
                uiview.MetaType = v.MetaType;
                uiview.ApplicationId = Application.Id;
                uiview.Properties = v.Properties;
                

                if (uiview.HasProperty("PAGESIZE"))
                    uiview.PageSize = uiview.GetAsInt("PAGESIZE");
                if (uiview.PageSize == 0)
                    uiview.PageSize = 20; //DEFAULT

                if (v.IsMetaTypeEditListView)
                {
                    foreach (var liedcol in UIStructure.Where(p => p.IsMetaTypeEditListViewColumn && p.ParentMetaCode == v.MetaCode).OrderBy(p => p.ColumnOrder))
                    {
                        if (!liedcol.IsDataColumn1Connected)
                            continue;
                        if (!liedcol.IsDataTableConnected)
                            continue;

                        liedcol.JavaScriptObjectName = "item";
                        uiview.Columns.Add(liedcol);
                    }
                }
                foreach (var sect in UIStructure.Where(p => p.IsMetaTypeSection && p.ParentMetaCode == v.MetaCode).OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
                {
                    var section = new UISection() { Properties = sect.Properties, Title = sect.Title, LocalizedTitle = sect.LocalizedTitle, TitleLocalizationKey = sect.TitleLocalizationKey };
                    foreach (var pnl in UIStructure.Where(p => p.IsMetaTypePanel && p.ParentMetaCode == sect.MetaCode).OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
                    {
                        var panel = new UIPanel() { Properties = pnl.Properties, Title = pnl.Title, LocalizedTitle = pnl.LocalizedTitle, TitleLocalizationKey = pnl.TitleLocalizationKey };
                        if (!string.IsNullOrEmpty(panel.Title) && (uiview.IsMetaTypeCreateView || uiview.IsMetaTypeEditView))
                            panel.UseFieldSet = true;
                      
                        foreach (var ctrl in UIStructure.Where(p => p.ParentMetaCode == pnl.MetaCode).OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
                        {
                            //Should not happen, just in case, remove container controls
                            if (ctrl.IsMetaTypePanel || ctrl.IsMetaTypeSection || ctrl.IsUIViewType)
                                continue;

                            if ((ctrl.IsUIBindingType || ctrl.IsUIComplexBindingType) && (!ctrl.IsDataColumn1Connected || !ctrl.IsDataTableConnected))
                                continue;

                            if (ctrl.IsUIComplexBindingType && (!ctrl.IsDataViewColumn1Connected || !ctrl.IsDataViewConnected))
                                continue;

                            if (ctrl.IsMetaTypeEditGrid && !ctrl.IsDataTableConnected)
                                continue;

                            if (v.IsMetaTypeDetailView || v.IsMetaTypeCreateView || v.IsMetaTypeEditView)
                                ctrl.JavaScriptObjectName = "model";
                            if (v.IsMetaTypeEditListView || v.IsMetaTypeListView)
                                ctrl.JavaScriptObjectName = "item";

                            if (ctrl.IsMetaTypeLookUp)
                                uiview.Modals.Add(ctrl);


                            if (ctrl.IsMetaTypeEditGrid)
                            {
                                ctrl.Children = new List<IUIControl>();
                                foreach (var col in UIStructure.Where(p => p.ParentMetaCode == ctrl.MetaCode && (p.IsEditGridUIBindingType || 
                                                                                                                 p.IsEditGridUIComplexBindingType)).OrderBy(p => p.ColumnOrder))
                                {
                                    if (!col.IsDataTableConnected || !col.IsDataColumn1Connected)
                                        continue;
                                    if (col.IsEditGridUIComplexBindingType && (!col.IsDataViewColumn1Connected || !col.IsDataViewConnected))
                                        continue;

                                    if (col.IsMetaTypeEditGridLookUp)
                                        uiview.Modals.Add(col);

                                    ctrl.Children.Add(col);
                                }

                            }
                            panel.Controls.Add(ctrl);
                        }
                        section.Panels.Add(panel);
                    }
                    uiview.Sections.Add(section);
                }
                res.Add(uiview);
            }

            return res;
        }
        */

    }

}
