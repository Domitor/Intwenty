using Intwenty.Model.Dto;
using Intwenty.DataClient;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Intwenty.Entity;
using System.Threading.Tasks;

namespace Intwenty.Interface
{
    public interface IIntwentyDataService
    {
        /// <summary>
        /// Creates a new application including defaultvalues.
        /// </summary>
        /// <returns>An Result including a json object</returns>
        DataResult New(ClientStateInfo state);
        /// <summary>
        /// Creates a new application including defaultvalues.
        /// </summary>
        /// <returns>An Result including a json object</returns>
        DataResult New(ApplicationModel model);

        /// <summary>
        /// Saves application data
        /// </summary>
        /// <returns>A result describing the state of the saved application</returns>
        //ModifyResult Save<TData>(ClientStateInfo<TData> state) where TData : InformationHeader, new();
        //ModifyResult SaveSubTable<TData>(ClientStateInfo<TData> state) where TData : InformationHeader, new();

        /// <summary>
        /// Saves application data
        /// </summary>
        /// <returns>A result describing the state of the saved application</returns>
        ModifyResult Save(ClientStateInfo state);

        /// <summary>
        /// Saves application data
        /// </summary>
        /// <returns>A result describing the state of the saved application</returns>
        ModifyResult Save(ClientStateInfo state, ApplicationModel model);

        /// <summary>
        /// Deletes all application data (maintable and subtables) by id.
        /// If the application uses versioning, all versions are deleted.
        /// </summary>
        /// <returns>A result describing the deleted  application</returns>
        ModifyResult Delete(ClientStateInfo state);

        /// <summary>
        /// Deletes all application data (maintable and subtables) by id.
        /// If the application uses versioning, all versions are deleted.
        /// </summary>
        /// <returns>A result describing the deleted  application</returns>
        ModifyResult Delete(ClientStateInfo state, ApplicationModel model);

        /// <summary>
        /// Deletes data by row Id
        /// If the application uses versioning, all versions are deleted.
        /// </summary>
        /// <returns>A result describing the deleted  application</returns>
        ModifyResult DeleteRow(ClientStateInfo state, int id, string tablename);


        /// <summary>
        /// Get the latest version data for and application based on Id
        /// </summary>
        /// <returns>A result including the application json data</returns>
        DataResult Get(ClientStateInfo state);


        /// <summary>
        /// Get the latest version data for and application based on Id
        /// </summary>
        /// <returns>A result including the application json data</returns>
        DataResult Get(ClientStateInfo state, ApplicationModel model);

        /// <summary>
        /// Get the latest version data for and application based on Id
        /// </summary>
        /// <returns>A result including the application data of type T</returns>
        DataResult<T> Get<T>(ClientStateInfo state, ApplicationModel model) where T : InformationHeader, new();

        /// <summary>
        /// Gets the latest version of the latest id for an application filtered on OwnerUserId and ApplicationId
        /// </summary>
        /// <returns>A result including the application json data</returns>
        //DataResult GetLatestByOwnerUser(ClientStateInfo state);





        /// <summary>
        /// Get a list of (latest version) application data that matches the filter specified in args. 
        /// This function supports paging. It returns the number of records specified in args.BatchSize
        /// 
        /// If args.OwnerUserId is set only applications owned by that OwnerUserId will be returned
        /// </summary>
        /// <returns>A DataListResult including a string json array</returns>
        DataListResult GetJsonArray(ListFilter args);

        /// <summary>
        /// Get a list of (latest version) application data that matches the filter specified in args. 
        /// This function supports paging. It returns the number of records specified in args.BatchSize
        /// 
        /// If args.OwnerUserId is set only applications owned by that OwnerUserId will be returned
        /// </summary>
        /// <returns>A DataListResult including a string json array</returns>
        DataListResult GetJsonArray(ListFilter args, ApplicationModel model);


        /// <summary>
        /// Get a list of (latest version) application data that matches the filter specified in args. 
        /// This function supports paging. It returns the number of records specified in args.BatchSize
        /// 
        /// If args.OwnerUserId is set only applications owned by that OwnerUserId will be returned
        /// </summary>
        /// <returns>A result object that inhertits DataListResult including a string json array</returns>
        TDataListResult GetJsonArray<TDataListResult>(ListFilter args, ApplicationModel model) where TDataListResult : DataListResult, new();



        /// <summary>
        /// Get a list of (latest version) application data that matches the filter specified in args. 
        /// This function supports paging. It returns the number of records specified in args.BatchSize
        /// 
        /// If args.OwnerUserId is set only applications owned by that OwnerUserId will be returned
        /// </summary>
        /// <returns>A DataListResult including a list of T and the current paging rownum</returns>
        DataListResult<T> GetEntityList<T>(ListFilter args, ApplicationModel model) where T : InformationHeader, new();



        /// <summary>
        /// Get a list of (latest version) application data. 
        /// All columns and rows from the application's main table is returned.
        /// </summary>
        /// <returns>An DataListResult including a list of T</returns>
        //DataListResult<T> GetList<T>(int applicationid) where T : InformationHeader, new();



        /// <summary>
        /// Get a list of (latest version) application data. 
        /// All columns and rows from the application's main table is returned.
        /// </summary>
        /// <returns>An OperationResult including a json array</returns>
        //DataListResult GetJsonArray(int applicationid);


        /// <summary>
        /// Get value domains for an application specified by application id
        /// </summary>
        DataListResult GetValueDomains(int ApplicationId);

        /// <summary>
        /// Get all value domains.
        /// </summary>
        DataListResult GetValueDomains();

        /// <summary>
        /// Get all value domain items.
        /// </summary>
        /// <returns>A list of ValueDomainModelItem</returns>
        List<ValueDomainModelItem> GetValueDomainItems();

        /// <summary>
        /// Get all value domain items for one domain.
        /// </summary>
        /// <returns>A list of ValueDomainModelItem</returns>
        List<ValueDomainModelItem> GetValueDomainItems(string domainname);

        /// <summary>
        /// Gets a list of data based on the DataView defined by args.DataViewMetaCode and that matches the filter specified in args.
        /// </summary>
        /// <returns>An OperationResult including a json array</returns>
        DataListResult GetDataView(ListFilter args);

        /// <summary>
        /// Gets  the first record of data based on the DataView defined by args.DataViewMetaCode and that matches the filter specified in args.
        /// </summary>
        /// <returns>An OperationResult including a json object</returns>
        DataListResult GetDataViewRecord(ListFilter args);

        /// <summary>
        /// Validates an application according to validation rules in the model
        /// </summary>
        /// <returns>OperationResult as the result of the validation</returns>
        ModifyResult Validate(ClientStateInfo state);

        void LogError(string message, int applicationid = 0, string appmetacode = "NONE", string username = "");

        void LogWarning(string message, int applicationid = 0, string appmetacode = "NONE", string username = "");

        void LogInfo(string message, int applicationid = 0, string appmetacode = "NONE", string username = "");

        /// <summary>
        /// Get eventlog items
        /// </summary>
        /// <returns>A list of eventlog items</returns>
        Task<List<EventLog>> GetEventLog(string verbosity);

        IDataClient GetDataClient();

        IDataClient GetIAMDataClient();

    }
}
