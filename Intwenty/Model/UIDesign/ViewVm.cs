using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Model.UIDesign
{

    public static class UIModelCreator
    {

        public static List<UserInterfaceModelItem> GetUIModel(ViewVm model, ApplicationModel app, List<DataViewModelItem> views)
        {
            var res = new List<UserInterfaceModelItem>();


            var viewitem = app.UIStructure.Find(p => p.MetaType == model.MetaType);
            if (viewitem == null)
            {
                viewitem = new UserInterfaceModelItem(model.MetaType) { Id = -1, AppMetaCode = app.Application.MetaCode, MetaCode = "", ColumnOrder = 1, RowOrder = 1, Title = "", ParentMetaCode = BaseModelItem.MetaTypeRoot };
                viewitem.MetaCode = BaseModelItem.GenerateNewMetaCode(viewitem);
            }

            if (viewitem == null)
                return res;

            viewitem.Properties = model.CompilePropertyString();
            viewitem.Title = model.Title;

            res.Add(viewitem);

            foreach (var section in model.Sections)
            {
                //SECTION CREATED IN UI BUT THEN REMOVED BEFORE SAVE
                if (section.Id == 0 && section.IsRemoved)
                    continue;

                var sect = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypeSection) { Id = section.Id, AppMetaCode = app.Application.MetaCode, MetaCode = section.MetaCode, ColumnOrder = 1, RowOrder = section.RowOrder, Title = section.Title, ParentMetaCode = viewitem.MetaCode };
                res.Add(sect);
                if (sect.Id == 0)
                    sect.MetaCode = BaseModelItem.GenerateNewMetaCode(sect);

                if (section.Collapsible)
                    sect.AddUpdateProperty("COLLAPSIBLE", "TRUE");
                if (section.StartExpanded)
                    sect.AddUpdateProperty("STARTEXPANDED", "TRUE");
                if(section.IsRemoved)
                    sect.AddUpdateProperty("REMOVED", "TRUE");

                foreach (var panel in section.LayoutPanels)
                {
                    //PANEL CREATED IN UI BUT THEN REMOVED BEFORE SAVE
                    if (panel.Id == 0 && panel.IsRemoved)
                        continue;

                    var pnl = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypePanel) { Id = panel.Id, AppMetaCode = app.Application.MetaCode, MetaCode = panel.MetaCode, ColumnOrder = panel.ColumnOrder, RowOrder = panel.RowOrder, Title = panel.Title, ParentMetaCode = sect.MetaCode };
                    res.Add(pnl);
                    if (pnl.Id == 0)
                        pnl.MetaCode = BaseModelItem.GenerateNewMetaCode(pnl);

                    if (panel.IsRemoved)
                        pnl.AddUpdateProperty("REMOVED", "TRUE");

                    foreach (var lr in section.LayoutRows)
                    {
                        foreach (var input in lr.UserInputs)
                        {
                            if (input.ColumnOrder != pnl.ColumnOrder || string.IsNullOrEmpty(input.MetaType) || (input.Id == 0 && input.IsRemoved))
                                continue;

                            var dto = new UserInterfaceModelItem(input.MetaType) { Id = input.Id, AppMetaCode = app.Application.MetaCode, MetaCode = input.MetaCode, ColumnOrder = input.ColumnOrder, RowOrder = input.RowOrder, Title = input.Title, ParentMetaCode = pnl.MetaCode, Properties = input.CompilePropertyString() };
                            res.Add(dto);

                            if (dto.Id == 0)
                                dto.MetaCode = BaseModelItem.GenerateNewMetaCode(dto);

                            if (input.IsRemoved)
                                dto.AddUpdateProperty("REMOVED", "TRUE");

                            if (!string.IsNullOrEmpty(input.TableName))
                            {
                                var dmc = app.DataStructure.Find(p => p.DbName == input.TableName && p.IsMetaTypeDataTable);
                                if (dmc != null)
                                    dto.DataTableMetaCode = dmc.MetaCode;
                            }

                            if (dto.IsUIBindingType)
                            {
                                if (!string.IsNullOrEmpty(input.ColumnName))
                                {
                                    var dmc = app.DataStructure.Find(p => p.DbName == input.ColumnName && p.IsRoot);
                                    if (dmc != null)
                                        dto.DataColumn1MetaCode = dmc.MetaCode;
                                }
                                if (dto.IsMetaTypeComboBox)
                                {
                                    if (!string.IsNullOrEmpty(input.Domain))
                                    {
                                        dto.Domain = "VALUEDOMAIN." + input.Domain;
                                    }
                                }
                            }

                            if (dto.IsUIComplexBindingType)
                            {
                                if (!string.IsNullOrEmpty(input.DataViewMetaCode))
                                {
                                    dto.DataViewMetaCode = input.DataViewMetaCode;
                                }

                                if (!string.IsNullOrEmpty(input.ColumnName))
                                {
                                    var dmc = app.DataStructure.Find(p => p.DbName == input.ColumnName && p.IsRoot);
                                    if (dmc != null)
                                        dto.DataColumn1MetaCode = dmc.MetaCode;
                                }

                                if (!string.IsNullOrEmpty(input.ColumnName2))
                                {
                                    var dmc = app.DataStructure.Find(p => p.DbName == input.ColumnName2 && p.IsRoot);
                                    if (dmc != null)
                                        dto.DataColumn2MetaCode = dmc.MetaCode;
                                }

                                if (!string.IsNullOrEmpty(input.DataViewColumnName))
                                {
                                    var dmc = views.Find(p => p.SQLQueryFieldName == input.DataViewColumnName && p.ParentMetaCode == input.DataViewMetaCode);
                                    if (dmc != null)
                                        dto.DataViewColumn1MetaCode = dmc.MetaCode;
                                }

                               
                                if (!string.IsNullOrEmpty(input.DataViewColumnName2))
                                {
                                    var dmc = views.Find(p => p.SQLQueryFieldName == input.DataViewColumnName2 && p.ParentMetaCode == input.DataViewMetaCode);
                                    if (dmc != null)
                                        dto.DataViewColumn2MetaCode = dmc.MetaCode;
                                }
                            }

                            if (dto.IsMetaTypeStaticHTML)
                            {
                                dto.RawHTML = input.RawHTML;

                            }

                            if (dto.IsMetaTypeEditGrid)
                            {
                                DatabaseModelItem dt = null;
                                if (!string.IsNullOrEmpty(input.TableName))
                                {
                                    dt = app.DataStructure.Find(p => p.DbName == input.TableName && p.IsMetaTypeDataTable);
                                    if (dt != null)
                                    {
                                        dto.DataTableMetaCode = dt.MetaCode;
                                    }
                                }

                                foreach (var tcol in input.Children)
                                {
                                    if (tcol.Id == 0 && tcol.IsRemoved)
                                        continue;

                                    var column = new UserInterfaceModelItem(tcol.MetaType) { Id = tcol.Id, AppMetaCode = app.Application.MetaCode, MetaCode = tcol.MetaCode, ColumnOrder = tcol.ColumnOrder, RowOrder = tcol.RowOrder, Title = tcol.Title, ParentMetaCode = dto.MetaCode, Properties = tcol.CompilePropertyString() };
                                    res.Add(column);

                                    if (column.Id == 0)
                                        column.MetaCode = BaseModelItem.GenerateNewMetaCode(column);

                                    if (tcol.IsRemoved || input.IsRemoved)
                                        column.AddUpdateProperty("REMOVED", "TRUE");

                                    if (column.IsEditGridUIBindingType || column.IsEditGridUIComplexBindingType)
                                    {
                                        if (dt != null)
                                        {
                                            if (!string.IsNullOrEmpty(tcol.ColumnName))
                                            {
                                                var dmc = app.DataStructure.Find(p => p.DbName == tcol.ColumnName && p.ParentMetaCode == dt.MetaCode && p.IsMetaTypeDataColumn);
                                                if (dmc != null)
                                                    column.DataColumn1MetaCode = dmc.MetaCode;
                                            }
                                        }

                                        if (column.IsMetaTypeEditGridComboBox)
                                        {
                                            if (!string.IsNullOrEmpty(tcol.Domain))
                                            {
                                                column.Domain = "VALUEDOMAIN." + tcol.Domain;
                                            }
                                        }


                                        if (column.IsMetaTypeEditGridLookUp)
                                        {
                                            if (!string.IsNullOrEmpty(tcol.ColumnName2) && dt != null)
                                            {
                                                var dmc = app.DataStructure.Find(p => p.DbName == tcol.ColumnName2 && p.ParentMetaCode == dt.MetaCode);
                                                if (dmc != null)
                                                    column.DataColumn2MetaCode = dmc.MetaCode;
                                            }

                                            if (!string.IsNullOrEmpty(tcol.DataViewMetaCode))
                                            {
                                                column.DataViewMetaCode = tcol.DataViewMetaCode;
                                            }

                                            if (!string.IsNullOrEmpty(tcol.DataViewColumnName) && !string.IsNullOrEmpty(tcol.DataViewMetaCode))
                                            {
                                                var dmc = views.Find(p => p.SQLQueryFieldName == tcol.DataViewColumnName && p.ParentMetaCode == tcol.DataViewMetaCode);
                                                if (dmc != null)
                                                    column.DataViewColumn1MetaCode = dmc.MetaCode;
                                            }

                                            if (!string.IsNullOrEmpty(tcol.DataViewColumnName2) && !string.IsNullOrEmpty(tcol.DataViewMetaCode))
                                            {
                                                var dmc = views.Find(p => p.SQLQueryFieldName == tcol.DataViewColumnName2 && p.ParentMetaCode == tcol.DataViewMetaCode);
                                                if (dmc != null)
                                                    column.DataViewColumn2MetaCode = dmc.MetaCode;
                                            }
                                        }
                                    }
                                    else if (column.IsMetaTypeEditGridStaticHTML)
                                    {
                                        column.RawHTML = tcol.RawHTML;
                                    }
                                }


                                
                            }
                        }

                    } 
                }
            }

            return res;

        }

        public static List<UserInterfaceModelItem> GetListViewUIModel(ViewVm model, ApplicationModel app)
        {
            var res = new List<UserInterfaceModelItem>();
            var t = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypeListView) { Title = model.Title, MetaCode = model.MetaCode, ParentMetaCode = "ROOT", Id = model.Id, AppMetaCode = app.Application.MetaCode };
            if (string.IsNullOrEmpty(model.MetaCode))
                t.MetaCode = BaseModelItem.GenerateNewMetaCode(t);

            res.Add(t);


            foreach (var f in model.Fields)
            {
                var lf = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypeEditListViewColumn) { Title = f.Title, MetaCode = "", ParentMetaCode = t.MetaCode, Id = f.Id, AppMetaCode = app.Application.MetaCode };
                if (string.IsNullOrEmpty(lf.MetaCode))
                    lf.MetaCode = BaseModelItem.GenerateNewMetaCode(lf);

                if (!string.IsNullOrEmpty(f.DbName))
                {
                    var dmc = app.DataStructure.Find(p => p.DbName == f.DbName && p.IsRoot && p.IsMetaTypeDataColumn);
                    if (dmc != null)
                    {
                        lf.DataColumn1MetaCode = dmc.MetaCode;
                    }
                   
                }

                res.Add(lf);
            }

            return res;
        }

        public static ViewVm GetUIVm(ApplicationModel app, string viewtype)
        {
            var res = new ViewVm();
            res.ApplicationId = app.Application.Id;
            res.ApplicationTitle = app.Application.Title;

            res.ViewType = viewtype;
            if (viewtype.ToUpper() == "CRVIEW")
            {
                res.DesignerTitle = "Create View";
                res.MetaType = UserInterfaceModelItem.MetaTypeCreateView;
            }
            else if (viewtype.ToUpper() == "EDVIEW")
            {
                res.DesignerTitle = "Edit View";
                res.MetaType = UserInterfaceModelItem.MetaTypeEditView;
            }
            else if (viewtype.ToUpper() == "EDLIVIEW")
            {
                res.DesignerTitle = "Edit List View";
                res.MetaType = UserInterfaceModelItem.MetaTypeEditListView;
            }
            else if (viewtype.ToUpper() == "DEVIEW")
            {
                res.DesignerTitle = "Detail View";
                res.MetaType = UserInterfaceModelItem.MetaTypeDetailView;
            }
            else if (viewtype.ToUpper() == "LIVIEW")
            {
                res.DesignerTitle = "List View";
                res.MetaType = UserInterfaceModelItem.MetaTypeListView;
            }
            else
            {
                throw new InvalidOperationException("invalid view type in UIModelCreater.GetUIVm");
            }

            SetCollections(res);

            var viewitem = app.UIStructure.Find(p => p.MetaType == res.MetaType);
            if (viewitem == null)
                return res;

            res.MetaCode = viewitem.MetaCode;
            res.Properties = viewitem.Properties;
            res.Id = viewitem.Id;
            res.Title = viewitem.Title;
            res.BuildPropertyList();

            if (!viewitem.IsMetaTypeEditListView)
                BuildVm(res, viewitem, app);
            else
                BuildEditListViewVm(res, viewitem, app);

            return res;
        }

        private static void SetCollections(ViewVm res)
        {
            res.PropertyCollection = IntwentyRegistry.IntwentyProperties;
            res.UIControls = new List<IntwentyMetaType>();
            res.GridUIControls = new List<IntwentyMetaType>();

            var temp = IntwentyRegistry.IntwentyMetaTypes.Where(p => p.ModelCode == "UIMODEL").ToList();

            res.UIControls.Add(temp.Find(p=> p.Code == UserInterfaceModelItem.MetaTypeCheckBox));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeComboBox));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeDatePicker));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeEmailBox));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeImage));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeImageBox));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeLabel));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeLookUp));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeNumBox));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypePasswordBox));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeStaticHTML));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeTextArea));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeTextBlock));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeTextBox));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeEditGrid));

            res.GridUIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeEditGridCheckBox));
            res.GridUIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeEditGridComboBox));
            res.GridUIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeEditGridDatePicker));
            res.GridUIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeEditGridEmailBox));
            res.GridUIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeEditGridLookUp));
            res.GridUIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeEditGridNumBox));
            res.GridUIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeEditGridStaticHTML));
            res.GridUIControls.Add(temp.Find(p => p.Code == UserInterfaceModelItem.MetaTypeEditGridTextBox));


        }

        public static void BuildVm(ViewVm res, UserInterfaceModelItem viewitem, ApplicationModel app)
        {

            foreach (var uic in app.UIStructure.OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
            {
                if (uic.ParentMetaCode != viewitem.MetaCode)
                    continue;

                if (uic.IsMetaTypeSection)
                {
                    var sect = new Section() { Id = uic.Id, Title = uic.Title, MetaCode = uic.MetaCode, ParentMetaCode = "ROOT", RowOrder = uic.RowOrder, ColumnOrder = 1 };
                    sect.Collapsible = uic.HasPropertyWithValue("COLLAPSIBLE", "TRUE");
                    sect.StartExpanded = uic.HasPropertyWithValue("STARTEXPANDED", "TRUE");
                    res.Sections.Add(sect);
                }
            }

            foreach (var section in res.Sections)
            {

                foreach (var uicomp in app.UIStructure.OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
                {
                    if (uicomp.ParentMetaCode == section.MetaCode || section.Id == 0)
                    {

                        if (uicomp.IsMetaTypePanel)
                        {
                            var pnl = new UserInput() { Id = uicomp.Id, ApplicationId = app.Application.Id, ColumnOrder = uicomp.ColumnOrder, RowOrder = 1, MetaCode = uicomp.MetaCode, MetaType = uicomp.MetaType, Title = uicomp.Title, ParentMetaCode = "ROOT", Properties = uicomp.Properties };
                            pnl.BuildPropertyList();
                            section.LayoutPanels.Add(pnl);
                            foreach (var uic in app.UIStructure.OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
                            {

                                if (uic.ParentMetaCode != pnl.MetaCode)
                                    continue;

                                LayoutRow lr = section.LayoutRows.Find(p => p.RowOrder == uic.RowOrder);
                                if (lr == null)
                                {
                                    lr = new LayoutRow() { RowOrder = uic.RowOrder };
                                    section.LayoutRows.Add(lr);

                                }


                                //SIMPLE INPUTS
                                if (uic.IsUIBindingType)
                                {
                                    var input = new UserInput() { Id = uic.Id, ApplicationId = app.Application.Id, ColumnOrder = pnl.ColumnOrder, RowOrder = uic.RowOrder, MetaCode = uic.MetaCode, MetaType = uic.MetaType, Title = uic.Title, ParentMetaCode = uic.ParentMetaCode, Domain = uic.DomainName, Properties = uic.Properties };
                                    input.BuildPropertyList();
                                    if (uic.IsDataColumn1Connected)
                                    {
                                        input.ColumnName = uic.DataColumn1Info.ColumnName;
                                    }
                                    if (uic.IsDataTableConnected)
                                    {
                                        input.TableName = uic.DataTableInfo.TableName;
                                    }
                                    lr.UserInputs.Add(input);
                                }

                                //Static
                                if (uic.IsMetaTypeStaticHTML)
                                {
                                    var input = new UserInput() { Id = uic.Id, ApplicationId = app.Application.Id, ColumnOrder = pnl.ColumnOrder, RowOrder = uic.RowOrder, MetaCode = uic.MetaCode, MetaType = uic.MetaType, Title = uic.Title, ParentMetaCode = uic.ParentMetaCode, Domain = uic.DomainName, Properties = uic.Properties };
                                    input.BuildPropertyList();
                                    input.RawHTML = uic.RawHTML;
                                    lr.UserInputs.Add(input);
                                }

                                //LOOK UP
                                if (uic.IsUIComplexBindingType)
                                {

                                    var input = new UserInput() { Id = uic.Id, ApplicationId = app.Application.Id, ColumnOrder = pnl.ColumnOrder, RowOrder = uic.RowOrder, MetaCode = uic.MetaCode, MetaType = uic.MetaType, Title = uic.Title, ParentMetaCode = uic.ParentMetaCode, Domain = uic.DataViewMetaCode, Properties = uic.Properties };
                                    input.BuildPropertyList();

                                    //TABLE ND COLUMN(S)
                                    if (uic.IsDataColumn1Connected)
                                    {
                                        input.ColumnName = uic.DataColumn1Info.ColumnName;
                                        if (uic.IsDataColumn2Connected)
                                            input.ColumnName2 = uic.DataColumn2Info.ColumnName;
                                    }
                                    if (uic.IsDataTableConnected)
                                    {
                                        input.TableName = uic.DataTableInfo.TableName;
                                    }


                                    //VIEW CONNECTION
                                    input.DataViewMetaCode = uic.DataViewMetaCode;

                                    if (uic.IsDataViewColumn1Connected)
                                        input.DataViewColumnName = uic.DataViewColumn1Info.SQLQueryFieldName;
                                    if (uic.IsDataViewColumn2Connected)
                                        input.DataViewColumnName2 = uic.DataViewColumn2Info.SQLQueryFieldName;

                                    lr.UserInputs.Add(input);
                                }

                                if (uic.IsMetaTypeEditGrid)
                                {
                                    var input = new UserInput() { Id = uic.Id, ApplicationId = app.Application.Id, ColumnOrder = pnl.ColumnOrder, RowOrder = uic.RowOrder, MetaCode = uic.MetaCode, MetaType = uic.MetaType, Title = uic.Title, ParentMetaCode = uic.ParentMetaCode, Domain = uic.DomainName, Properties = uic.Properties };
                                    input.BuildPropertyList();
                                    if (uic.IsDataTableConnected)
                                        input.TableName = uic.DataTableInfo.TableName;

                                    //COLUMNS
                                    foreach (var gridcol in app.UIStructure.Where(p=> p.ParentMetaCode == input.MetaCode).OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
                                    {
                                        var child = new UserInput() { Id = gridcol.Id, ApplicationId = app.Application.Id, ColumnOrder = gridcol.ColumnOrder, RowOrder = gridcol.RowOrder, MetaCode = gridcol.MetaCode, MetaType = gridcol.MetaType, Title = gridcol.Title, ParentMetaCode = gridcol.ParentMetaCode, Domain = gridcol.DomainName, Properties = gridcol.Properties };
                                        child.BuildPropertyList();
                                        input.Children.Add(child);

                                        child.TableName = input.TableName;

                                        if (gridcol.IsDataColumn1Connected)
                                            child.ColumnName = gridcol.DataColumn1Info.ColumnName;
                                        if (gridcol.IsDataColumn2Connected)
                                            child.ColumnName2 = gridcol.DataColumn2Info.ColumnName;

                                        if (gridcol.IsMetaTypeEditGridLookUp)
                                        {
                                            //VIEW CONNECTION
                                            child.DataViewMetaCode = gridcol.DataViewMetaCode;
                                            if (gridcol.IsDataViewColumn1Connected)
                                                child.DataViewColumnName = gridcol.DataViewColumn1Info.SQLQueryFieldName;
                                            if (gridcol.IsDataViewColumn2Connected)
                                                child.DataViewColumnName2 = gridcol.DataViewColumn2Info.SQLQueryFieldName;

                                        }

                                        if (gridcol.IsMetaTypeEditGridStaticHTML)
                                        {
                                            child.RawHTML = gridcol.RawHTML;
                                        }
                                    }

                                    lr.UserInputs.Add(input);
                                }
                            }
                        }
                    }
                }

                section.LayoutPanelCount = app.UIStructure.Count(p => p.IsMetaTypePanel && p.ParentMetaCode == section.MetaCode);
            }

        }

        public static void BuildEditListViewVm(ViewVm res, UserInterfaceModelItem viewitem, ApplicationModel app)
        {

            foreach (var f in app.UIStructure)
            {
                if (f.IsMetaTypeEditListViewColumn && f.ParentMetaCode == viewitem.MetaCode)
                {
                    if (f.IsDataColumn1Connected)
                        res.Fields.Add(new ListViewFieldVm() { Id = f.Id, Properties = f.Properties, Title = f.Title, DbName = f.DataColumn1Info.DbName });
                    else
                        res.Fields.Add(new ListViewFieldVm() { Id = f.Id, Properties = f.Properties, Title = f.Title });
                }
            }


        }


    }


    public class ViewVm : BaseModelVm
    {

        public int ApplicationId { get; set; }
        public string ApplicationTitle { get; set; }
        public string ViewType { get; set;  }
        public string DesignerTitle{ get; set; }
        public string Title { get; set; }
        public string MetaType { get; set; }
        public string MetaCode { get; set; }
        public bool ShowComponents { get; set; }
        public List<Section> Sections { get; set; }
        public List<IntwentyProperty> PropertyCollection { get; set; }
        public List<ListViewFieldVm> Fields { get; set; }
        public List<IntwentyMetaType> UIControls { get; set; }
        public List<IntwentyMetaType> GridUIControls { get; set; }

        public ViewVm()
        {
            ApplicationTitle = "";
            Sections = new List<Section>();
            ViewType = "";
            DesignerTitle = "";
            Title = "";
            MetaType = "";
            MetaCode = "";
            Properties = "";
            Fields = new List<ListViewFieldVm>();
        }

    }

    public class UIDeleteVm
    {
        public int Id { get; set; }

        public List<UserInterfaceModelItem> Components { get; set; }


        public UIDeleteVm()
        {
            Components = new List<UserInterfaceModelItem>();
        }

    }


    public class Section
    {
        public int Id { get; set; }

        public int RowOrder { get; set; }

        public int ColumnOrder { get; set; }

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public string Title { get; set; }

        public int LayoutPanelCount { get; set; }

        public bool Collapsible { get; set; }

        public bool StartExpanded { get; set; }

        public bool IsRemoved { get; set; }

        public List<UserInput> LayoutPanels { get; set; }

        public List<LayoutRow> LayoutRows { get; set; }

        public Section()
        {
            Title = "";
            ParentMetaCode = "";
            MetaCode = "";
            LayoutPanels = new List<UserInput>();
            LayoutRows = new List<LayoutRow>();
            
        }
    }

    public class LayoutRow
    {
        public int RowOrder { get; set; }

        public List<UserInput> UserInputs { get; set; }

        public LayoutRow()
        {
            UserInputs = new List<UserInput>();
        }
    }

    public class UserInput : BaseModelVm
    {
        public int ApplicationId { get; set; }
        public string Title { get; set; }
        public int RowOrder { get; set; }
        public int ColumnOrder { get; set; }
        public string Description { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnName2 { get; set; }
        public string DataViewMetaCode { get; set; }
        public string DataViewColumnName { get; set; }
        public string DataViewColumnName2 { get; set; }
        public string MetaType { get; set; }
        public string MetaCode { get; set; }
        public string ParentMetaCode { get; set; }
        public string RawHTML { get; set; }
        public string Domain { get; set; }
        public bool IsRemoved { get; set; }
        public List<UserInput> Children { get; set; }



        public UserInput()
        {
            Title = "";
            Description = "";
            TableName = "";
            MetaType = "";
            MetaCode = "";
            ParentMetaCode = "";
            ColumnName = "";
            ColumnName2 = "";
            DataViewColumnName = "";
            DataViewColumnName2 = "";
            DataViewMetaCode = "";
            Domain = "";
            RawHTML = "";
            Children = new List<UserInput>();

        }

    }

    public class ListViewFieldVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string DbName { get; set; }
        public string Properties { get; set; }

        public ListViewFieldVm()
        {
            Title = "";
            DbName = "";
            Properties = "";
        }
    }




}
