using Intwenty.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.UIRendering
{

    public class UIView : HashTagPropertyObject, ILocalizableTitle
    {
        public int ApplicationId { get; set; }
        public string MetaType { get; set; }
        public string TitleLocalizationKey { get; set; }
        public string Title { get; set; }
        public string LocalizedTitle { get; set; }
        public List<UISection>  Sections { get; set; }
        public List<IEditListViewColumn> Columns { get; set; }
        public List<IUIControl> Modals { get; set; }
        public int PageSize { get; set; }

        public bool IsMetaTypeCreateView
        {
            get { return MetaType == UserInterfaceModelItem.MetaTypeCreateView; }
        }

        public bool IsMetaTypeEditView
        {
            get { return MetaType == UserInterfaceModelItem.MetaTypeEditView; }
        }

        public bool IsMetaTypeDetailView
        {
            get { return MetaType == UserInterfaceModelItem.MetaTypeDetailView; }
        }

        public bool IsMetaTypeListView
        {
            get { return MetaType == UserInterfaceModelItem.MetaTypeListView; }
        }

        public bool IsMetaTypeEditListView
        {
            get { return MetaType == UserInterfaceModelItem.MetaTypeEditListView; }
        }
        public UIView()
        {
            Sections = new List<UISection>();
            Columns = new List<IEditListViewColumn>();
            Modals = new List<IUIControl>();
        }
    }

   

    public class UISection : HashTagPropertyObject, ILocalizableTitle
    {
        public string TitleLocalizationKey { get; set; }
        public string Title { get; set; }
        public string LocalizedTitle { get; set; }
        public List<UIPanel> Panels { get; set; }

        public UISection()
        {
            Panels = new List<UIPanel>();
        }
    }

    public class UIPanel : HashTagPropertyObject, ILocalizableTitle
    {
        public string TitleLocalizationKey { get; set; }
        public string Title { get; set; }
        public string LocalizedTitle { get; set; }

        public bool UseFieldSet { get; set; }

        public List<IUIControl> Controls { get; set; }

        public UIPanel()
        {
            Controls = new List<IUIControl>();
        }
    }

   

}
