﻿using Moley.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moley.Models
{

    public static class UIVmCreator
    {
        public static UIVm GetUIVm(ApplicationDto app)
        {
            var res = new UIVm();
            res.Id = app.Application.Id;
            res.Title = app.Application.Title;

            var layoutpnls = 0;
            foreach (var uic in app.UIStructure.OrderBy(p=> p.RowOrder).ThenBy(p=> p.ColumnOrder))
            {
                if (uic.IsUITypePanel)
                {
                    if (res.LayoutRows.Count == 0)
                        res.LayoutRows.Add(new LayoutRow() { Id = 1 });

                    var pnl = new UserInput() { Id=uic.Id, ApplicationId = app.Application.Id, Colid = uic.ColumnOrder, Rowid = uic.RowOrder, MetaCode = uic.MetaCode, MetaType = uic.MetaType, Title = uic.Title, ParentMetaCode = "ROOT" };
                    res.UserInputs.Add(pnl);

                    if (uic.ColumnOrder > layoutpnls)
                    {
                        layoutpnls = uic.ColumnOrder;
                        res.LayoutPanels = Convert.ToString(layoutpnls);
                    }
                }
            }

            foreach (var uic in app.UIStructure.OrderBy(p => p.RowOrder).ThenBy(p => p.ColumnOrder))
            {
               
                    //SIMPLE INPUTS
                    if (uic.IsUITypeCheckBox || uic.IsUITypeComboBox || uic.IsUITypeDatePicker || uic.IsUITypeNumBox || uic.IsUITypeTextArea || uic.IsUITypeTextBox)
                    {
                        if (res.LayoutRows.Count < uic.RowOrder)
                        {
                            res.LayoutRows.Add(new LayoutRow() { Id = uic.RowOrder });
                        }

                        var input = new UserInput() { Id = uic.Id, ApplicationId = app.Application.Id, Colid = uic.ColumnOrder, Rowid = uic.RowOrder, MetaCode = uic.MetaCode, MetaType = uic.MetaType, Title = uic.Title, ParentMetaCode = uic.ParentMetaCode, Domain = uic.DomainName };
                        if (uic.IsDataConnected)
                        {
                            input.ColumnName = uic.DataInfo.ColumnName;
                            input.TableName = uic.DataInfo.TableName;
                        }
                        res.UserInputs.Add(input);
                    }

                //LOOK UP
                if (uic.IsUITypeLookUp)
                {
                    if (res.LayoutRows.Count < uic.RowOrder)
                    {
                        res.LayoutRows.Add(new LayoutRow() { Id = uic.RowOrder });
                    }

                    var keyfield = app.UIStructure.Find(p => p.IsUITypeLookUpKeyField && p.ParentMetaCode == uic.MetaCode);
                    var lookupfield = app.UIStructure.Find(p => p.IsUITypeLookUpField && p.ParentMetaCode == uic.MetaCode);

                    var input = new UserInput() { Id = uic.Id, ApplicationId = app.Application.Id, Colid = uic.ColumnOrder, Rowid = uic.RowOrder, MetaCode = uic.MetaCode, MetaType = uic.MetaType, Title = uic.Title, ParentMetaCode = uic.ParentMetaCode, Domain = uic.ViewName};
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
                    res.UserInputs.Add(input);
                }

            }


            return res;
        }
    }

    public static class MetaUIItemCreator
    {

        public static List<MetaUIItemDto> GetMetaUI(UIVm model, ApplicationDto app, List<MetaDataViewDto> views)
        {
            var res = new List<MetaUIItemDto>();
            foreach (var input in model.UserInputs)
            {
                var dto = new MetaUIItemDto(input.MetaType) { Id=input.Id, AppMetaCode = app.Application.MetaCode, ColumnOrder = input.Colid, RowOrder = input.Rowid, Title = input.Title, ParentMetaCode = input.ParentMetaCode };

                if (string.IsNullOrEmpty(dto.MetaCode))
                    dto.MetaCode = dto.MetaType + MetaModelDto.GetRandomUniqueString();

                if (dto.IsUITypeCheckBox || dto.IsUITypeComboBox || dto.IsUITypeDatePicker || dto.IsUITypeNumBox || dto.IsUITypeTextArea || dto.IsUITypeTextBox)
                {
                    if (!string.IsNullOrEmpty(input.ColumnName))
                    {
                        var dmc = app.DataStructure.Find(p => p.DbName == input.ColumnName);
                        if (dmc != null)
                            dto.DataMetaCode = dmc.MetaCode;
                    }
                    if (dto.IsUITypeComboBox)
                    {
                        if (!string.IsNullOrEmpty(input.Domain))
                        {
                            dto.Domain = "VALUEDOMAIN."+input.Domain;
                        }
                    }
                }

                if (dto.IsUITypeLookUp)
                {
                    if (!string.IsNullOrEmpty(input.Domain))
                    {
                        dto.Domain = "DATAVIEW." + input.Domain;
                    }

                    var keyfield = new MetaUIItemDto(dto.UITypeLookUpKeyField) { AppMetaCode = app.Application.MetaCode, ColumnOrder = 1, RowOrder = 1, Title = input.LookUpKeyFieldTitle, ParentMetaCode = dto.MetaCode };
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
                    res.Add(keyfield);

                    var lookupfield = new MetaUIItemDto(dto.UITypeLookUpField) { AppMetaCode = app.Application.MetaCode, ColumnOrder = 1, RowOrder = 1, Title = input.LookUpKeyFieldTitle, ParentMetaCode = dto.MetaCode };
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
                    res.Add(lookupfield);


                }

                res.Add(dto);
               

            }


            return res;

        }

    }

    public class UIVm
    {
        public int Id = 0;
        public string Title = "";
        public string LayoutPanels = "2";
        public List<LayoutRow> LayoutRows = new List<LayoutRow>();
        public List<UserInput> UserInputs = new List<UserInput>();

    }

    public class LayoutRow
    {
        public int Id = 0;

    }

    public class UserInput
    {
        public int Id = 0;
        public int ApplicationId = 0;
        public string Title = "";
        public int Rowid = 0;
        public int Colid = 0;
        public string Description = "";
        public string Properties = "";
        public string TableName = "";
        public string ColumnName = "";
        public string MetaType = "";
        public string MetaCode = "";
        public string ParentMetaCode = "";
        public string LookUpKeyFieldDbName = "";
        public string LookUpKeyFieldViewDbName = "";
        public string LookUpKeyFieldTitle = "";
        public string LookUpFieldDbName = "";
        public string LookUpFieldViewDbName = "";
        public string LookUpFieldTitle = "";
        public string Domain = "";
        public bool ShowSettings = false;

    }

   
  



}
