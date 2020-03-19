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
                var sect = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypeSection) { Id = section.Id, AppMetaCode = app.Application.MetaCode, MetaCode = section.MetaCode, ColumnOrder = 1, RowOrder = section.RowOrder, Title = section.Title, ParentMetaCode = BaseModelItem.MetaTypeRoot };
                res.Add(sect);
                if (sect.Id == 0)
                    sect.MetaCode = BaseModelItem.GenerateNewMetaCode(sect);

                foreach (var panel in section.LayoutPanels)
                {
                    var pnl = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypePanel) { Id = panel.Id, AppMetaCode = app.Application.MetaCode, MetaCode = panel.MetaCode, ColumnOrder = panel.ColOrder, RowOrder = panel.RowOrder, Title = panel.Title, ParentMetaCode = sect.MetaCode };
                    res.Add(pnl);

                    foreach (var lr in section.LayoutRows)
                    {
                        foreach (var input in lr.UserInputs)
                        {
                            if (input.ParentMetaCode != pnl.MetaCode)
                                continue;

                            var dto = new UserInterfaceModelItem(input.MetaType) { Id = input.Id, AppMetaCode = app.Application.MetaCode, MetaCode = input.MetaCode, ColumnOrder = input.ColOrder, RowOrder = input.RowOrder, Title = input.Title, ParentMetaCode = input.ParentMetaCode };

                            if (dto.Id==0)
                                dto.MetaCode = BaseModelItem.GenerateNewMetaCode(dto);

                            if (dto.IsMetaTypeCheckBox || dto.IsMetaTypeComboBox || dto.IsMetaTypeDatePicker || dto.IsMetaTypeNumBox || dto.IsMetaTypeTextArea || dto.IsMetaTypeTextBox)
                            {
                                if (!string.IsNullOrEmpty(input.ColumnName))
                                {
                                    var dmc = app.DataStructure.Find(p => p.DbName == input.ColumnName);
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

                                var keyfield = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypeLookUpKeyField) { AppMetaCode = app.Application.MetaCode, ColumnOrder = 1, RowOrder = 1, Title = input.LookUpKeyFieldTitle, ParentMetaCode = dto.MetaCode };
                                if (!string.IsNullOrEmpty(input.LookUpKeyFieldDbName))
                                {
                                    var dmc = app.DataStructure.Find(p => p.DbName == input.LookUpKeyFieldDbName);
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
                                res.Add(keyfield);

                                var lookupfield = new UserInterfaceModelItem(UserInterfaceModelItem.MetaTypeLookUpField) { AppMetaCode = app.Application.MetaCode, ColumnOrder = 1, RowOrder = 1, Title = input.LookUpKeyFieldTitle, ParentMetaCode = dto.MetaCode };
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
                                res.Add(lookupfield);


                            }

                            res.Add(dto);


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

         

            if (!app.UIStructure.Exists(p => p.IsMetaTypeSection))
            {
                res.Sections.Add(new Section() { Id = 0, Title = "-- Your Title --", MetaCode = "", ParentMetaCode = "ROOT", RowOrder = 1, ColOrder = 1 });

            }
            else
            {
                foreach (var uic in app.UIStructure.OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
                {
                    if (uic.IsMetaTypeSection)
                    {
                        res.Sections.Add(new Section() { Id = uic.Id, Title = uic.Title, MetaCode = uic.MetaCode, ParentMetaCode = "ROOT" });
                    }
                }
            }

            foreach (var section in res.Sections)
            {

                foreach (var uicomp in app.UIStructure.OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
                {
                    if (uicomp.ParentMetaCode == section.MetaCode || section.MetaCode == "DEFAULTSECTION")
                    {
                       
                        if (uicomp.IsMetaTypePanel)
                        {
                            var pnl = new UserInput() { Id = uicomp.Id, ApplicationId = app.Application.Id, Colid = uicomp.ColumnOrder, Rowid = 1, MetaCode = uicomp.MetaCode, MetaType = uicomp.MetaType, Title = uicomp.Title, ParentMetaCode = "ROOT" };
                            section.LayoutPanels.Add(pnl);
                            foreach (var uic in app.UIStructure.OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
                            {

                                if (uic.ParentMetaCode != pnl.MetaCode)
                                    continue;

                                LayoutRow lr = section.LayoutRows.Find(p => p.Id == uic.RowOrder);
                                if (lr == null)
                                {
                                    lr = new LayoutRow() { Id = uic.RowOrder };
                                    section.LayoutRows.Add(lr);

                                }


                                //SIMPLE INPUTS
                                if (uic.IsMetaTypeCheckBox || uic.IsMetaTypeComboBox || uic.IsMetaTypeDatePicker || uic.IsMetaTypeNumBox || uic.IsMetaTypeTextArea || uic.IsMetaTypeTextBox)
                                {


                                    var input = new UserInput() { Id = uic.Id, ApplicationId = app.Application.Id, Colid = uic.ColumnOrder, Rowid = uic.RowOrder, MetaCode = uic.MetaCode, MetaType = uic.MetaType, Title = uic.Title, ParentMetaCode = uic.ParentMetaCode, Domain = uic.DomainName };
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

                                    var input = new UserInput() { Id = uic.Id, ApplicationId = app.Application.Id, Colid = uic.ColumnOrder, Rowid = uic.RowOrder, MetaCode = uic.MetaCode, MetaType = uic.MetaType, Title = uic.Title, ParentMetaCode = uic.ParentMetaCode, Domain = uic.ViewName };
                                    if (keyfield != null)
                                    {
                                        if (keyfield.IsDataConnected)
                                        {
                                            input.TableName = keyfield.DataInfo.TableName;
                                            input.LookUpKeyFieldDbName = keyfield.DataInfo.ColumnName;
                                            input.LookUpKeyFieldTitle = keyfield.Title;
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
                                            input.LookUpFieldTitle = lookupfield.Title;
                                        }
                                        if (lookupfield.IsDataViewConnected)
                                        {
                                            input.LookUpFieldViewDbName = lookupfield.ViewInfo.SQLQueryFieldName;
                                        }
                                    }
                                    lr.UserInputs.Add(input);
                                }
                            }
                        }
                    }
                }

                section.LayoutPanelCount = app.UIStructure.Count(p => p.IsMetaTypePanel).ToString();
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

    public class Section
    {
        public int Id { get; set; }

        public int RowOrder { get; set; }

        public int ColOrder { get; set; }

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public string Title { get; set; }

        public string LayoutPanelCount { get; set; }
       
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
        public int Id { get; set; }

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
        public int ColOrder { get; set; }
        public string Description { get; set; }
        public string Properties { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string MetaType { get; set; }
        public string MetaCode { get; set; }
        public string ParentMetaCode { get; set; }
        public string LookUpKeyFieldDbName { get; set; }
        public string LookUpKeyFieldViewDbName { get; set; }
        public string LookUpKeyFieldTitle { get; set; }
        public string LookUpFieldDbName { get; set; }
        public string LookUpFieldViewDbName { get; set; }
        public string LookUpFieldTitle { get; set; }
        public string Domain { get; set; }
        public bool ShowSettings { get; set; }

      

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
            LookUpKeyFieldTitle = "";
            LookUpFieldDbName = "";
            LookUpFieldViewDbName = "";
            LookUpFieldTitle = "";
            Domain = "";


        }

    }

   
  



}
