﻿using System;
using Intwenty.Data.DBAccess.Helpers;
using Intwenty.Data.Entity;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Intwenty.Model;
using Intwenty.Data.Dto;

namespace Intwenty.Data.DBAccess
{

    public interface IIntwentyDb
    {
        public string ConnectionString { get; }

        public DBMS DbEngine { get; }

        public bool IsNoSql { get; }

    }

    public interface IIntwentyDbORM
    {
        void CreateTable<T>(bool checkexisting = false);
        T GetOne<T>(int id) where T : new();
        T GetOne<T>(string id) where T : new();
        List<T> GetAll<T>() where T : new();
        int Insert<T>(T model);
        int Update<T>(T model);
        int Delete<T>(T model);
        int DeleteRange<T>(IEnumerable<T> model);

    }

    public interface IIntwentyDbNoSql : IIntwentyDb, IIntwentyDbORM
    {

        public void DropCollection(string collectionname);

        public bool DeleteIntwentyJsonObject(string collectionname, int id, int version);

        public int InsertIntwentyJsonObject(string json, string collectionname, int id, int version);

        public int UpdateIntwentyJsonObject(string json, string collectionname, int id, int version);

        public bool DeleteJsonDocument(string collectionname, int id);

        public bool DeleteJsonDocument(string collectionname, string id);

        public bool UpdateJsonDocument(string collectionname, int id);

        public bool UpdateJsonDocument(string collectionname, string id);

        public bool InsertJsonDocument(string collectionname, int id);

        public bool InsertJsonDocument(string collectionname, string id);



        public int GetAutoIncrementalId(int applicationid=0, string metatype="INTERNAL", string metacode="UNSPECIFIED", string properties="");

        /// <summary>
        /// Counts the documents in the collection
        /// </summary>
        public int GetJsonDocumentCount(string collectionname);


        StringBuilder GetJsonArray(string collectionname, List<IIntwentyDataColum> returnfields=null, int minrow = 0, int maxrow = 0);

        StringBuilder GetJsonArray(string collectionname, string filterexpression, List<IIntwentyDataColum> returnfields = null, int minrow = 0, int maxrow = 0);

        StringBuilder GetIntwentyJsonObject(string collectionname, int id, int version, List<IIntwentyDataColum> returnfields = null);

        StringBuilder GetJsonObject(string collectionname, string id);

        StringBuilder GetJsonObject(string collectionname, int id);

        StringBuilder GetJsonObject(string collectionname, string filterexpression, List<IIntwentyDataColum> returnfields = null);

        ApplicationTable GetDataSet(string collectionname, string filterexpression = "");
    }


    public interface IIntwentyDbSql : IIntwentyDb, IIntwentyDbORM
    {
        void Open();
        void Close();
        void CreateCommand(string sql);
        void CreateSPCommand(string procedurename);
        void AddParameter(string name, object value);
        void AddParameter(IntwentySqlParameter p);
        object ExecuteScalarQuery();
        bool TableExist(string tablename);
        bool ColumnExist(string tablename, string columnname);
        StringBuilder GetJsonArray(List<IIntwentyDataColum> returnfields, int minrow = 0, int maxrow = 0);
        StringBuilder GetJsonArray(int minrow = 0, int maxrow = 0);
        StringBuilder GetJsonObject();
        StringBuilder GetJsonObject(List<IIntwentyDataColum> returnfields);

        ApplicationTable GetDataSet();

        NonQueryResult ExecuteNonQuery();
        void CreateTable<T>(bool checkexisting = false, bool use_current_connection = false);
        T GetOne<T>(int id, bool use_current_connection = false) where T : new();
        List<T> GetAll<T>(bool use_current_connection = false) where T : new();
        int Insert<T>(T model, bool use_current_connection = false);
        int Update<T>(T model, bool use_current_connection = false);
        int Delete<T>(T model, bool use_current_connection = false);
        int DeleteRange<T>(IEnumerable<T> model, bool use_current_connection = false);
    }



}
