using Intwenty.Model.Dto;
using Intwenty.Entity;
using Intwenty.DataClient;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Interface
{
    public interface IIntwentyDataService
    {
        /// <summary>
        /// Creates a new application JSON Document including defaultvalues.
        /// </summary>
        /// <returns>An OperationResult including a json object</returns>
        OperationResult CreateNew(ClientStateInfo state);


        /// <summary>
        /// Saves application data
        /// </summary>
        /// <returns>An OperationResult including Id and Version for the saved application</returns>
        OperationResult Save(ClientStateInfo state);

        /// <summary>
        /// Deletes all application data (maintable and subtables) by id.
        /// If the application uses versioning, all versions are deleted.
        /// </summary>
        /// <returns>An OperationResult including Id and Version for the deleted application</returns>
        OperationResult DeleteById(ClientStateInfo state);

        /// <summary>
        /// Deletes data by Id
        /// Parameter Id can be an Id of an application subtable row, or an application maintable Id
        /// Parameter dbname can be an application  subtable name or main tablename
        /// If the dbname represents a main application table, all application data (maintable and subtables) is deleted.
        /// If the dbname represents an application subtable, only the subtable row that matches the id parameter is deleted.
        /// If the application uses versioning, all versions are deleted.
        /// </summary>
        /// <returns>An OperationResult including Id and Version for the deleted application</returns>
        OperationResult DeleteById(int applicationid, int id, string dbname);


        /// <summary>
        /// Get the latest version data for and application based on Id
        /// </summary>
        /// <returns>An OperationResult including a json object</returns>
        OperationResult GetLatestVersionById(ClientStateInfo state);

        /// <summary>
        /// Gets the latest version of an application based on OwnerUserId and ApplicationId
        /// </summary>
        /// <returns>An OperationResult including a json object</returns>
        OperationResult GetLatestVersionByOwnerUser(ClientStateInfo state);


        /// <summary>
        /// Get a list of (latest version) application data that matches the filter specified in args. 
        /// This function supports paging. It returns the number of records specified in args.BatchSize
        /// 
        /// If args.OwnerUSerId is set only applications owned by that OwnerUserId will be returned
        /// </summary>
        /// <returns>An OperationResult including a json array and the current paging rownum</returns>
        OperationResult GetPagedList(ListRetrivalArgs args);


        /// <summary>
        /// Get a list of (latest version) application data. 
        /// All columns from the application's main table is returned.
        /// </summary>
        /// <returns>An OperationResult including a json array</returns>
        OperationResult GetList(int applicationid);

        /// <summary>
        /// Get a list of (latest version) application data based on OwnedBy. 
        /// All columns from the application's main table is returned.
        /// </summary>
        /// <returns>An OperationResult including a json array</returns>
        OperationResult GetListByOwnerUser(int applicationid, string owneruserid);

        /// <summary>
        /// Get a list of all versions for an application based on Id
        /// </summary>
        /// <returns>An OperationResult including a json array</returns>
        OperationResult GetVersionListById(ClientStateInfo state);

        /// <summary>
        /// Get the data for an application based on Id and Version
        /// </summary>
        /// <returns>An OperationResult including a json object</returns>
        OperationResult GetVersion(ClientStateInfo state);

        /// <summary>
        /// Get value domains used by UI in the application specified by application id
        /// </summary>
        OperationResult GetValueDomains(int ApplicationId);

        /// <summary>
        /// Get all value domains.
        /// </summary>
        OperationResult GetValueDomains();

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
        OperationResult GetDataView(ListRetrivalArgs args);

        /// <summary>
        /// Gets  the first record of data based on the DataView defined by args.DataViewMetaCode and that matches the filter specified in args.
        /// </summary>
        /// <returns>An OperationResult including a json object</returns>
        OperationResult GetDataViewRecord(ListRetrivalArgs args);

        /// <summary>
        /// Validates an application according to validation rules in the model
        /// </summary>
        /// <returns>OperationResult as the result of the validation</returns>
        OperationResult Validate(ClientStateInfo state);

        void LogError(string message, int applicationid = 0, string appmetacode = "NONE", string username = "");

        void LogWarning(string message, int applicationid = 0, string appmetacode = "NONE", string username = "");

        void LogInfo(string message, int applicationid = 0, string appmetacode = "NONE", string username = "");

        IDataClient GetDataClient();

    }
}
