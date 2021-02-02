using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Model.UIDesign
{

    public static class InputUIDesignerModelCreator
    {

        public static List<UserInterfaceStructureModelItem> ConvertDesignerModel(UserInterfaceInputDesignVm model, ApplicationModel app, List<DataViewModelItem> views)
        {
            var res = new List<UserInterfaceStructureModelItem>();


            foreach (var section in model.Sections)
            {
                //SECTION CREATED IN UI BUT THEN REMOVED BEFORE SAVE
                if (section.Id == 0 && section.IsRemoved)
                    continue;

                var sect = new UserInterfaceStructureModelItem(UserInterfaceStructureModelItem.MetaTypeSection) { Id = section.Id, AppMetaCode = app.Application.MetaCode, MetaCode = section.MetaCode, ColumnOrder = 1, RowOrder = section.RowOrder, Title = section.Title, ParentMetaCode = "ROOT" };
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

                    var pnl = new UserInterfaceStructureModelItem(UserInterfaceStructureModelItem.MetaTypePanel) { Id = panel.Id, AppMetaCode = app.Application.MetaCode, MetaCode = panel.MetaCode, ColumnOrder = panel.ColumnOrder, RowOrder = panel.RowOrder, Title = panel.Title, ParentMetaCode = sect.MetaCode };
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

                            var dto = new UserInterfaceStructureModelItem(input.MetaType) { Id = input.Id, AppMetaCode = app.Application.MetaCode, MetaCode = input.MetaCode, ColumnOrder = input.ColumnOrder, RowOrder = input.RowOrder, Title = input.Title, ParentMetaCode = pnl.MetaCode, Properties = input.CompilePropertyString() };
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

                         
                        }

                    } 
                }
            }

            

            return res;

        }

      

        public static UserInterfaceInputDesignVm GetDesignerModel(ApplicationModel app, UserInterfaceModelItem userinterface)
        {
            var res = new UserInterfaceInputDesignVm(userinterface.MetaCode);
            res.ApplicationId = app.Application.Id;
            SetCollections(res);
            res.BuildPropertyList();

           foreach (var uic in userinterface.UIStructure.OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
           {
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

               foreach (var uicomp in userinterface.UIStructure.OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
               {
                   if (uicomp.ParentMetaCode == section.MetaCode || section.Id == 0)
                   {

                       if (uicomp.IsMetaTypePanel)
                       {
                           var pnl = new UserInput() { Id = uicomp.Id, ApplicationId = app.Application.Id, ColumnOrder = uicomp.ColumnOrder, RowOrder = 1, MetaCode = uicomp.MetaCode, MetaType = uicomp.MetaType, Title = uicomp.Title, ParentMetaCode = "ROOT", Properties = uicomp.Properties };
                           pnl.BuildPropertyList();
                           section.LayoutPanels.Add(pnl);
                           foreach (var uic in userinterface.UIStructure.OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
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

                             
                           }
                       }
                   }
               }

               section.LayoutPanelCount = userinterface.UIStructure.Count(p => p.IsMetaTypePanel && p.ParentMetaCode == section.MetaCode);
           }
           

            return res;
        }

        private static void SetCollections(UserInterfaceInputDesignVm res)
        {
            res.PropertyCollection = IntwentyRegistry.IntwentyProperties;

            res.UIControls = new List<IntwentyMetaType>();

            var temp = IntwentyRegistry.IntwentyMetaTypes.Where(p => p.ModelCode == "UISTRUCTUREMODEL").ToList();

            res.UIControls.Add(temp.Find(p=> p.Code == UserInterfaceStructureModelItem.MetaTypeCheckBox));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeComboBox));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeDatePicker));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeEmailBox));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeImage));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeImageBox));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeLabel));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeLookUp));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeNumBox));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypePasswordBox));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeStaticHTML));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeTextArea));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeTextBlock));
            res.UIControls.Add(temp.Find(p => p.Code == UserInterfaceStructureModelItem.MetaTypeTextBox));

        }

    


    }


    public class UserInterfaceInputDesignVm : BaseModelVm
    {
        public int ApplicationId { get; set; }
        public string MetaType { get; set; }
        public string MetaCode { get; set; }
        public bool ShowComponents { get; set; }
        public List<Section> Sections { get; set; }
        public List<IntwentyProperty> PropertyCollection { get; set; }
        public List<IntwentyMetaType> UIControls { get; set; }

        public UserInterfaceInputDesignVm(string metacode)
        {
            Sections = new List<Section>();
            MetaType = UserInterfaceModelItem.MetaTypeInputInterface;
            MetaCode = metacode;
            Properties = "";
        }

    }

    public class UIDeleteVm
    {
        public int Id { get; set; }

        public List<UserInterfaceStructureModelItem> Components { get; set; }


        public UIDeleteVm()
        {
            Components = new List<UserInterfaceStructureModelItem>();
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

   
}
