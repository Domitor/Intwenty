using Intwenty.MetaDataService.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Models.MetaDesigner
{

    public static class UIModelCreator
    {

        public static List<UserInterfaceModelItem> GetUIModel(UIVm model, ApplicationModel app, List<DataViewModelItem> views)
        {
            var res = new List<UserInterfaceModelItem>();

            foreach (var section in model.Sections)
            {
                //SECTION CREATED IN UI BUT THEN REMOVED BEFORE SAVE
                if (section.Id == 0 && section.IsRemoved)
                    continue;

                var sect = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypeSection) { Id = section.Id, AppMetaCode = app.Application.MetaCode, MetaCode = section.MetaCode, ColumnOrder = 1, RowOrder = section.RowOrder, Title = section.Title, ParentMetaCode = BaseModelItem.MetaTypeRoot };
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

                            var dto = new UserInterfaceModelItem(input.MetaType) { Id = input.Id, AppMetaCode = app.Application.MetaCode, MetaCode = input.MetaCode, ColumnOrder = input.ColumnOrder, RowOrder = input.RowOrder, Title = input.Title, ParentMetaCode = pnl.MetaCode };
                            res.Add(dto);

                            if (dto.Id==0)
                                dto.MetaCode = BaseModelItem.GenerateNewMetaCode(dto);

                            if (input.IsRemoved)
                                dto.AddUpdateProperty("REMOVED", "TRUE");

                            if (dto.IsMetaTypeCheckBox || dto.IsMetaTypeComboBox || dto.IsMetaTypeDatePicker || dto.IsMetaTypeNumBox || dto.IsMetaTypeTextArea || dto.IsMetaTypeTextBox)
                            {
                                if (!string.IsNullOrEmpty(input.ColumnName))
                                {
                                    var dmc = app.DataStructure.Find(p => p.DbName == input.ColumnName && p.IsRoot);
                                    if (dmc != null)
                                        dto.DataMetaCode = dmc.MetaCode;
                                }
                                if (dto.IsMetaTypeComboBox)
                                {
                                    if (!string.IsNullOrEmpty(input.Domain))
                                    {
                                        dto.Domain = "VALUEDOMAIN." + input.Domain;
                                    }
                                }
                            }

                            if (dto.IsMetaTypeLookUp)
                            {
                                if (!string.IsNullOrEmpty(input.Domain))
                                {
                                    dto.Domain = "DATAVIEW." + input.Domain;
                                }

                                var keyfield = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypeLookUpKeyField) { AppMetaCode = app.Application.MetaCode, ColumnOrder = 1, RowOrder = 1, Title = "", ParentMetaCode = dto.MetaCode };
                                if (!string.IsNullOrEmpty(input.LookUpKeyFieldDbName))
                                {
                                    var dmc = app.DataStructure.Find(p => p.DbName == input.LookUpKeyFieldDbName && p.IsRoot);
                                    if (dmc != null)
                                        keyfield.DataMetaCode = dmc.MetaCode;
                                }
                                if (!string.IsNullOrEmpty(input.LookUpKeyFieldViewDbName))
                                {
                                    var dmc = views.Find(p => p.SQLQueryFieldName == input.LookUpKeyFieldViewDbName && p.ParentMetaCode == input.Domain);
                                    if (dmc != null)
                                        keyfield.Domain = "DATAVIEW." + input.Domain + "." + dmc.MetaCode;
                                }
                                if (dto.Id > 0)
                                {
                                    var currentkeyfield = app.UIStructure.Find(p => p.IsMetaTypeLookUpKeyField && p.ParentMetaCode == dto.MetaCode);
                                    if (currentkeyfield != null)
                                    {
                                        keyfield.Id = currentkeyfield.Id;
                                        keyfield.MetaCode = currentkeyfield.MetaCode;
                                    }
                                }
                                else
                                {
                                    if (keyfield.Id == 0)
                                        keyfield.MetaCode = BaseModelItem.GenerateNewMetaCode(keyfield);
                                }
                               
                                res.Add(keyfield);

                                var lookupfield = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypeLookUpField) { AppMetaCode = app.Application.MetaCode, ColumnOrder = 1, RowOrder = 1, Title = "", ParentMetaCode = dto.MetaCode };
                                if (!string.IsNullOrEmpty(input.LookUpFieldDbName))
                                {
                                    var dmc = app.DataStructure.Find(p => p.DbName == input.LookUpFieldDbName);
                                    if (dmc != null)
                                        lookupfield.DataMetaCode = dmc.MetaCode;
                                }
                                if (!string.IsNullOrEmpty(input.LookUpFieldViewDbName))
                                {
                                    var dmc = views.Find(p => p.SQLQueryFieldName == input.LookUpFieldViewDbName && p.ParentMetaCode == input.Domain);
                                    if (dmc != null)
                                        lookupfield.Domain = "DATAVIEW." + input.Domain + "." + dmc.MetaCode;
                                }
                                if (dto.Id > 0)
                                {
                                    var currentlookupfield = app.UIStructure.Find(p => p.IsMetaTypeLookUpField && p.ParentMetaCode == dto.MetaCode);
                                    if (currentlookupfield != null)
                                    {
                                        lookupfield.Id = currentlookupfield.Id;
                                        lookupfield.MetaCode = currentlookupfield.MetaCode;
                                    }
                                }
                                else
                                {
                                    if (lookupfield.Id == 0)
                                        lookupfield.MetaCode = BaseModelItem.GenerateNewMetaCode(lookupfield);
                                }
                                res.Add(lookupfield);


                            }

                            if (dto.IsMetaTypeEditGrid)
                            {
                                DatabaseModelItem dt = null;
                                if (!string.IsNullOrEmpty(input.TableName))
                                {
                                    dt = app.DataStructure.Find(p => p.DbName == input.TableName && p.IsMetaTypeDataTable);
                                    if (dt != null)
                                    {
                                        dto.DataMetaCode = dt.MetaCode;
                                    }
                                }

                                foreach (var tcol in input.Children)
                                {
                                    if (tcol.Id == 0 && tcol.IsRemoved)
                                        continue;

                                    var column = new UserInterfaceModelItem(tcol.MetaType) { Id = tcol.Id, AppMetaCode = app.Application.MetaCode, MetaCode = tcol.MetaCode, ColumnOrder = tcol.ColumnOrder, RowOrder = tcol.RowOrder, Title = tcol.Title, ParentMetaCode = dto.MetaCode };
                                    res.Add(column);

                                    if (column.Id == 0)
                                        column.MetaCode = BaseModelItem.GenerateNewMetaCode(column);

                                    if (tcol.IsRemoved || input.IsRemoved)
                                        column.AddUpdateProperty("REMOVED", "TRUE");

                                    if (column.IsMetaTypeEditGridCheckBox || column.IsMetaTypeEditGridComboBox || column.IsMetaTypeEditGridDatePicker || column.IsMetaTypeEditGridNumBox || column.IsMetaTypeEditGridTextBox || column.IsMetaTypeEditGridLookUp)
                                    {
                                        if (dt != null)
                                        {
                                            if (!string.IsNullOrEmpty(tcol.ColumnName))
                                            {
                                                var dmc = app.DataStructure.Find(p => p.DbName == tcol.ColumnName  && p.ParentMetaCode == dt.MetaCode);
                                                if (dmc != null)
                                                    column.DataMetaCode = dmc.MetaCode;
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
                                            if (!string.IsNullOrEmpty(tcol.Domain))
                                            {
                                                column.Domain = "DATAVIEW." + tcol.Domain;
                                            }

                                            var colkeyfield = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypeEditGridLookUpKeyField) { AppMetaCode = app.Application.MetaCode, ColumnOrder = 1, RowOrder = 1, Title = "", ParentMetaCode = column.MetaCode };
                                            if (!string.IsNullOrEmpty(tcol.LookUpKeyFieldDbName))
                                            {
                                                var dmc = app.DataStructure.Find(p => p.DbName == tcol.LookUpKeyFieldDbName && !p.IsRoot);
                                                if (dmc != null)
                                                    colkeyfield.DataMetaCode = dmc.MetaCode;
                                            }
                                            if (!string.IsNullOrEmpty(tcol.LookUpKeyFieldViewDbName))
                                            {
                                                var dmc = views.Find(p => p.SQLQueryFieldName == tcol.LookUpKeyFieldViewDbName && p.ParentMetaCode == tcol.Domain);
                                                if (dmc != null)
                                                    colkeyfield.Domain = "DATAVIEW." + tcol.Domain + "." + dmc.MetaCode;
                                            }
                                            if (column.Id > 0)
                                            {
                                                var currentkeyfield = app.UIStructure.Find(p => p.IsMetaTypeEditGridLookUpKeyField && p.ParentMetaCode == column.MetaCode);
                                                if (currentkeyfield != null)
                                                {
                                                    colkeyfield.Id = currentkeyfield.Id;
                                                    colkeyfield.MetaCode = currentkeyfield.MetaCode;
                                                }
                                            }
                                            else
                                            {
                                                if (colkeyfield.Id == 0)
                                                    colkeyfield.MetaCode = BaseModelItem.GenerateNewMetaCode(colkeyfield);
                                            }

                                            res.Add(colkeyfield);
                                        }
                                    }
                                }

                            }

                        }

                    } 
                }
            }

            return res;

        }

        public static UIVm GetUIVm(ApplicationModel app)
        {
            var res = new UIVm();
            res.Id = app.Application.Id;
            res.Title = app.Application.Title;

         

       
            foreach (var uic in app.UIStructure.OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
            {
                if (uic.IsMetaTypeSection)
                {
                    var sect = new Section() { Id = uic.Id, Title = uic.Title, MetaCode = uic.MetaCode, ParentMetaCode = "ROOT", RowOrder = uic.RowOrder, ColumnOrder = 1 };
                    sect.Collapsible = uic.HasPropertyWithValue("COLLAPSIBLE","TRUE");
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
                            var pnl = new UserInput() { Id = uicomp.Id, ApplicationId = app.Application.Id, ColumnOrder = uicomp.ColumnOrder, RowOrder = 1, MetaCode = uicomp.MetaCode, MetaType = uicomp.MetaType, Title = uicomp.Title, ParentMetaCode = "ROOT" };
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
                                if (uic.IsMetaTypeCheckBox || uic.IsMetaTypeComboBox || uic.IsMetaTypeDatePicker || uic.IsMetaTypeNumBox || uic.IsMetaTypeTextArea || uic.IsMetaTypeTextBox)
                                {
                                    var input = new UserInput() { Id = uic.Id, ApplicationId = app.Application.Id, ColumnOrder = uic.ColumnOrder, RowOrder = uic.RowOrder, MetaCode = uic.MetaCode, MetaType = uic.MetaType, Title = uic.Title, ParentMetaCode = uic.ParentMetaCode, Domain = uic.DomainName };
                                    if (uic.IsDataConnected)
                                    {
                                        input.ColumnName = uic.DataInfo.ColumnName;
                                        input.TableName = uic.DataInfo.TableName;
                                    }
                                    lr.UserInputs.Add(input);
                                }

                                //LOOK UP
                                if (uic.IsMetaTypeLookUp)
                                {

                                    var keyfield = app.UIStructure.Find(p => p.IsMetaTypeLookUpKeyField && p.ParentMetaCode == uic.MetaCode);
                                    var lookupfield = app.UIStructure.Find(p => p.IsMetaTypeLookUpField && p.ParentMetaCode == uic.MetaCode);

                                    var input = new UserInput() { Id = uic.Id, ApplicationId = app.Application.Id, ColumnOrder = uic.ColumnOrder, RowOrder = uic.RowOrder, MetaCode = uic.MetaCode, MetaType = uic.MetaType, Title = uic.Title, ParentMetaCode = uic.ParentMetaCode, Domain = uic.ViewName };
                                    if (keyfield != null)
                                    {
                                        if (keyfield.IsDataConnected)
                                        {
                                            input.TableName = keyfield.DataInfo.TableName;
                                            input.LookUpKeyFieldDbName = keyfield.DataInfo.ColumnName;
                                        }
                                        if (keyfield.IsDataViewConnected)
                                        {
                                            input.LookUpKeyFieldViewDbName = keyfield.ViewInfo.SQLQueryFieldName;
                                        }
                                    }
                                    if (lookupfield != null)
                                    {
                                        if (lookupfield.IsDataConnected)
                                        {
                                            input.LookUpFieldDbName = lookupfield.DataInfo.ColumnName;
                                        }
                                        if (lookupfield.IsDataViewConnected)
                                        {
                                            input.LookUpFieldViewDbName = lookupfield.ViewInfo.SQLQueryFieldName;
                                        }
                                    }

                                    lr.UserInputs.Add(input);
                                }

                                if (uic.IsMetaTypeEditGrid)
                                {
                                    var input = new UserInput() { Id = uic.Id, ApplicationId = app.Application.Id, ColumnOrder = uic.ColumnOrder, RowOrder = uic.RowOrder, MetaCode = uic.MetaCode, MetaType = uic.MetaType, Title = uic.Title, ParentMetaCode = uic.ParentMetaCode, Domain = uic.DomainName };
                                    if (uic.IsDataConnected)
                                        input.TableName = uic.DataInfo.TableName;

                                    //COLUMNS
                                    foreach (var gridcol in app.UIStructure.OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
                                    {
                                        if (gridcol.ParentMetaCode != input.MetaCode)
                                            continue;

                                        var child = new UserInput() { Id = gridcol.Id, ApplicationId = app.Application.Id, ColumnOrder = gridcol.ColumnOrder, RowOrder = gridcol.RowOrder, MetaCode = gridcol.MetaCode, MetaType = gridcol.MetaType, Title = gridcol.Title, ParentMetaCode = gridcol.ParentMetaCode, Domain = gridcol.DomainName };
                                        input.Children.Add(child);

                                        if (gridcol.IsDataConnected)
                                        {
                                            child.ColumnName = gridcol.DataInfo.ColumnName;
                                            child.TableName = gridcol.DataInfo.TableName;
                                        }

                                        if (gridcol.IsMetaTypeEditGridLookUp)
                                        {
                                            child.Domain = gridcol.ViewName;

                                            var keyfield = app.UIStructure.Find(p => p.IsMetaTypeEditGridLookUpKeyField && p.ParentMetaCode == gridcol.MetaCode);
                                            if (keyfield != null)
                                            {
                                                if (keyfield.IsDataConnected)
                                                {
                                                    child.TableName = keyfield.DataInfo.TableName;
                                                    child.LookUpKeyFieldDbName = keyfield.DataInfo.ColumnName;
                                                }
                                                if (keyfield.IsDataViewConnected)
                                                {
                                                    child.LookUpKeyFieldViewDbName = keyfield.ViewInfo.SQLQueryFieldName;
                                                }
                                            }
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

           


            return res;
        }

      
    }


    public class UIVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
       
        public List<Section> Sections { get; set; }
  

        public UIVm()
        {
            Title = "";
            Sections = new List<Section>();
         
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

    public class UserInput
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string Title { get; set; }
        public int RowOrder { get; set; }
        public int ColumnOrder { get; set; }
        public string Description { get; set; }
        public string Properties { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string MetaType { get; set; }
        public string MetaCode { get; set; }
        public string ParentMetaCode { get; set; }
        public string LookUpKeyFieldDbName { get; set; }
        public string LookUpKeyFieldViewDbName { get; set; }
        public string LookUpFieldDbName { get; set; }
        public string LookUpFieldViewDbName { get; set; }
        public string Domain { get; set; }
        public bool IsRemoved { get; set; }

        public List<UserInput> Children { get; set; }



        public UserInput()
        {
            Title = "";
            Description = "";
            Properties = "";
            TableName = "";
            ColumnName = "";
            MetaType = "";
            MetaCode = "";
            ParentMetaCode = "";
            LookUpKeyFieldDbName = "";
            LookUpKeyFieldViewDbName = "";
            LookUpFieldDbName = "";
            LookUpFieldViewDbName = "";
            Domain = "";
            Children = new List<UserInput>();

        }

    }

   
  



}
