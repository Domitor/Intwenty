using Moley.Data.Dto;
using System;
using System.Collections.Generic;


namespace Moley.Models
{
    public class UIVm
    {

        public int Id = 0;
        public int ApplicationId = 0;
        public string Title = "";
        public string Rowid = "";
        public string Colid = "";
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


        public List<UIFieldVm> Fields = new List<UIFieldVm>();

        public List<MetaUIItemDto> GetDtoList(ApplicationDto app, List<MetaDataViewDto> views)
        {
            var res = new List<MetaUIItemDto>();

            var panelcode = GetPanelAtCol(app, Convert.ToInt32(this.Colid));
            var uic = new MetaUIItemDto(this.MetaType);
            if (string.IsNullOrEmpty(panelcode))
            {
                panelcode = uic.UITypePanel + MetaModelDto.GetRandomUniqueString();
                var pnl = new MetaUIItemDto(uic.UITypePanel);
                pnl.AppMetaCode = app.Application.MetaCode;
                pnl.ColumnOrder = Convert.ToInt32(this.Colid);
                pnl.RowOrder = 1;
                pnl.MetaCode = panelcode;
                pnl.ParentMetaCode = "ROOT";
                res.Add(pnl);
            }
            uic.Id = this.Id;
            uic.AppMetaCode = app.Application.MetaCode;
            uic.ColumnOrder = Convert.ToInt32(this.Colid);
            uic.RowOrder = Convert.ToInt32(this.Rowid);
            uic.DataMetaCode = GetDataMetaCode(app, this.TableName, this.ColumnName);
            uic.Description = "";
            uic.Domain = "";
            if (uic.IsUITypeComboBox && !string.IsNullOrEmpty(this.Domain))
                uic.Domain = "VALUEDOMAIN." + this.Domain;
            if (uic.IsUITypeLookUp && !string.IsNullOrEmpty(this.Domain))
                uic.Domain = "DATAVIEW." + this.Domain;
            if (this.Id < 1 || string.IsNullOrEmpty(this.MetaCode))
                uic.MetaCode = uic.MetaType + MetaModelDto.GetRandomUniqueString();

            uic.ParentMetaCode = panelcode;
            uic.Properties = "";
            uic.Title = this.Title;
            res.Add(uic);

            if (uic.IsUITypeLookUp)
            {
                var kf = new MetaUIItemDto(uic.UITypeLookUpKeyField);
                kf.Id = this.Id;
                kf.AppMetaCode = app.Application.MetaCode;
                kf.ColumnOrder = 1;
                kf.RowOrder = 1;
                kf.DataMetaCode = GetDataMetaCode(app, this.TableName, this.LookUpKeyFieldDbName);
                kf.Description = "";
                kf.Domain = uic.Domain + "." + GetLookUpFieldViewMetaCode(views, this.Domain, this.LookUpKeyFieldViewDbName, true);
                kf.MetaCode = kf.MetaType + MetaModelDto.GetRandomUniqueString();
                kf.ParentMetaCode = uic.MetaCode;
                kf.Properties = "";
                kf.Title = this.LookUpKeyFieldTitle;
                res.Add(kf);

                var lf = new MetaUIItemDto(this.MetaType);
                uic.Id = this.Id;
                lf.AppMetaCode = app.Application.MetaCode;
                lf.ColumnOrder = 2;
                lf.RowOrder = 1;
                if (!string.IsNullOrEmpty(this.ColumnName))
                    lf.DataMetaCode = GetDataMetaCode(app, this.TableName, this.LookUpFieldDbName);
                lf.Description = "";
                lf.Domain = uic.Domain + "." + GetLookUpFieldViewMetaCode(views, this.Domain, this.LookUpKeyFieldViewDbName, false);
                lf.MetaCode = lf.MetaType + MetaModelDto.GetRandomUniqueString();
                lf.ParentMetaCode = uic.MetaCode;
                lf.Properties = "";
                lf.Title = this.Title;
                res.Add(lf);

            }


            return res;
        }

        private string GetPanelAtCol(ApplicationDto app, int col)
        {
            foreach (var t in app.UIStructure)
            {
                if (t.IsUITypePanel && t.ColumnOrder == col && t.IsRoot)
                    return t.MetaCode;

            }

            return string.Empty;
        }

        private string GetDataMetaCode(ApplicationDto app, string tablename, string columnname)
        {
            foreach (var t in app.DataStructure)
            {
                if (t.TableName == tablename && t.ColumnName == columnname)
                    return t.MetaCode;

            }

            return string.Empty;
        }

        private string GetLookUpFieldViewMetaCode(List<MetaDataViewDto> views, string domainname, string columnname, bool iskey)
        {
            foreach (var t in views)
            {
                if (iskey && t.IsMetaTypeDataViewKeyField && t.ParentMetaCode == domainname && t.SQLQueryFieldName == columnname)
                    return t.MetaCode;
                if (!iskey && t.IsMetaTypeDataViewField && t.ParentMetaCode == domainname && t.SQLQueryFieldName == columnname)
                    return t.MetaCode;

            }

            return string.Empty;
        }


        public static List<UIVm> GetInput(ApplicationDto app)
        {
            var res = new List<UIVm>();

            foreach (var t in app.UIStructure)
            {
                if (t.IsUITypeListView || t.IsUITypeListViewField)
                    continue;

                if (t.IsUITypeLookUp)
                {
                    var lookup = new UIVm() { Description = t.Description, Title = t.Title, Properties = t.Properties, Domain = t.Domain, MetaType = t.MetaType, Id=t.Id, ApplicationId=app.Application.Id };
                    foreach (var lf in app.UIStructure)
                    {
                        if (lf.IsUITypeLookUpKeyField && lf.ParentMetaCode == t.MetaCode)
                        {
                            lookup.LookUpKeyFieldTitle = lf.Title;
                            if (lf.IsDataConnected)
                            {
                                lookup.TableName = lf.DataInfo.TableName;
                                lookup.LookUpKeyFieldDbName = lf.DataInfo.DbName;
                            }
                            if (lf.IsDataViewConnected)
                            {
                                lookup.LookUpKeyFieldViewDbName = lf.ViewInfo.SQLQueryFieldName;
                            }
                        }

                        if (lf.IsUITypeLookUpField && lf.ParentMetaCode == t.MetaCode)
                        {
                            lookup.LookUpFieldTitle = lf.Title;
                            if (lf.IsDataConnected)
                            {
                                lookup.TableName = lf.DataInfo.TableName;
                                lookup.LookUpFieldDbName = lf.DataInfo.DbName;
                            }
                            if (lf.IsDataViewConnected)
                            {
                                lookup.LookUpFieldViewDbName = lf.ViewInfo.SQLQueryFieldName;
                            }
                        }
                    }

                    res.Add(lookup);

                }
                else
                {
                    if (t.IsUITypeLookUpField || t.IsUITypeLookUpKeyField)
                        continue;

                    if (t.IsDataConnected)
                        res.Add(new UIVm() { ColumnName = t.DataInfo.ColumnName, TableName = t.DataInfo.TableName, Description = t.Description, Title = t.Title, Properties = t.Properties, MetaType = t.MetaType,Id = t.Id, ApplicationId = app.Application.Id });
                    else
                        res.Add(new UIVm() { ColumnName = "", TableName = "", Description = t.Description, Title = t.Title, Properties = t.Properties, MetaType = t.MetaType, Id = t.Id, ApplicationId = app.Application.Id });
                }
                 

            }


            return res;
        }

        public static UIVm GetListView(ApplicationDto app)
        {
            var res = new UIVm();
            return res;
        }


    }

    public class UIFieldVm
    {

    }

    public class UIDbTable
    {
        public int Id = 0;
        public string DbName = "";
        public string MetaCode = "";
        public List<UIDbTableField> Columns = new List<UIDbTableField>();

        public static List<UIDbTable> GetTables(ApplicationDto app)
        {
            var res = new List<UIDbTable>();

            res.Add(new UIDbTable() { Id = 0, DbName = app.Application.MainTableName, MetaCode = "VIRTUAL" });

            foreach (var t in app.DataStructure)
            {
                if (t.IsMetaTypeDataValue && t.IsRoot)
                {
                    res[0].Columns.Add(new UIDbTableField() { DbName = t.DbName, Id = t.Id, MetaCode = t.MetaCode, ParentMetaCode = t.ParentMetaCode });
                }

                if (t.IsMetaTypeDataValueTable)
                {
                    var tbl = new UIDbTable() { Id = 0, DbName = app.Application.MainTableName, MetaCode = t.MetaCode };
                    foreach (var col in app.DataStructure)
                    {
                        if (col.IsMetaTypeDataValue && col.ParentMetaCode == t.MetaCode)
                        {
                            tbl.Columns.Add(new UIDbTableField() { DbName = col.DbName, Id = t.Id, MetaCode = col.MetaCode, ParentMetaCode = col.ParentMetaCode });
                        }
                    }
                    res.Add(tbl);
                }

            }

            return res;
        }
    }

    public class UIDbTableField
    {
        public int Id = 0;
        public string DbName = "";
        public string MetaCode = "";
        public string ParentMetaCode = "";
    }



}
