﻿using System.Data.SQLite;
using System.Data;
using Intwenty.DataClient.Model;
using System;
using System.Collections.Generic;

namespace Intwenty.DataClient.Databases
{
    public sealed class SqlLite : BaseSqlDb, ISqlClient
    {

        private SQLiteConnection connection;
        private SQLiteCommand command;
        private SQLiteTransaction transaction;

        public SqlLite(string connectionstring) : base(connectionstring)
        {

        }

        public override void Dispose()
        {
            transaction = null;
            command = null;
            transaction = null;
            IsInTransaction = false;
             
        }

        private SQLiteConnection GetConnection()
        {

            if (connection != null && connection.State == ConnectionState.Open)
                return connection;

            connection = new SQLiteConnection();
            connection.ConnectionString = this.ConnectionString;
            connection.Open();

            if (IsInTransaction && transaction == null)
                transaction = connection.BeginTransaction();

            return connection;
        }

        protected override IDbCommand GetCommand()
        {
            command = new SQLiteCommand();
            command.Connection = GetConnection();
            if (IsInTransaction && transaction != null)
                command.Transaction = transaction;

            return command;
        }

        protected override IDbTransaction GetTransaction()
        {
            return transaction;
        }

     

        protected override void SetPropertyValues<T>(IDataReader reader, IntwentyDataColumn column, T instance)
        {
            if (column.Property.PropertyType.FullName.ToUpper().Contains("SYSTEM.DATETIMEOFFSET"))
            {
                column.Property.SetValue(instance, new DateTimeOffset(reader.GetDateTime(column.Order)), null);
            }
            else
            {
                base.SetPropertyValues(reader, column, instance);
            }
        }

        protected override object GetObjectValue<T>(IntwentyDataColumn column, T instance)
        {
            var value = column.Property.GetValue(instance);
            if (value == null)
                return value;

            if (column.Property.PropertyType.FullName.ToUpper().Contains("SYSTEM.DATETIMEOFFSET"))
            {
                return ((DateTimeOffset)value).DateTime;
            }
         
           return base.GetObjectValue(column,  instance);
            
        }

        protected override void AddCommandParameters(List<IntwentySqlParameter> parameters)
        {
            foreach (var p in parameters)
            {
                var param = new SQLiteParameter() { ParameterName = p.Name, Value = p.Value, Direction = p.Direction };
                if (param.Direction == ParameterDirection.Output)
                    param.DbType = p.DataType;

                command.Parameters.Add(param);

            }
        }

        protected override void SetInsertQueryAutoIncResult<T>(IntwentyDataTable info, List<IntwentySqlParameter> parameters, T instance)
        {
            var autoinccol = info.Columns.Find(p => p.IsAutoIncremental);
            if (autoinccol == null)
                return;

            var command = GetCommand();
            command.CommandText = "SELECT LAST_INSERT_ID()";
            command.CommandType = CommandType.Text;

            autoinccol.Property.SetValue(instance, Convert.ToInt32(command.ExecuteScalar()), null);

        }

       
    }
}
