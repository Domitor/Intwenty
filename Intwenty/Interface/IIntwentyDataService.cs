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
        DataResult New(ClientOperation state);
        /// <summary>
        /// Creates a new application including defaultvalues.
        /// </summary>
        /// <returns>An Result including a json object</returns>
        DataResult New(ApplicationModel model);

        /// <summary>
        /// Saves application data
        /// </summary>
        /// <returns>A result describing the state of the saved application</returns>
        ModifyResult Save(ClientOperation state);

        /// <summary>
        /// Saves application data
        /// </summary>
        /// <returns>A result describing the state of the saved application</returns>
        ModifyResult Save(ClientOperation state, ApplicationModel model);

        /// <summary>
        /// Saves an application sub table row
        /// </summary>
        /// <returns>A result describing the result of the save operation</returns>
        ModifyResult SaveSubTableLine(ClientOperation state, ApplicationModel model, ApplicationTableRow row);

        /// <summary>
        /// Deletes all application data (maintable and subtables) by id.
        /// If the application uses versioning, all versions are deleted.
        /// </summary>
        /// <returns>A result describing the deleted  application</returns>
        ModifyResult Delete(ClientOperation state);

        /// <summary>
        /// Deletes all application data (maintable and subtables) by id.
        /// If the application uses versioning, all versions are deleted.
        /// </summary>
        /// <returns>A result describing the deleted  application</returns>
        ModifyResult Delete(ClientOperation state, ApplicationModel model);

        /// <summary>
        /// Deletes data by row Id
        /// If the application uses versioning, all versions are deleted.
        /// </summary>
        /// <returns>A result describing the deleted  application</returns>
        ModifyResult DeleteSubTableLine(ClientOperation state, ApplicationModel model, ApplicationTableRow row);


        /// <summary>
        /// Get the latest version data for and application based on Id
        /// </summary>
        /// <returns>A result including the application json data</returns>
        DataResult Get(ClientOperation state);


        /// <summary>
        /// Get the latest version data for and application based on Id
        /// </summary>
        /// <returns>A result including the application json data</returns>
        DataResult Get(ClientOperation state, ApplicationModel model);

        /// <summary>
        /// Get the latest version data for and application based on Id
        /// </summary>
        /// <returns>A result including the application data of type T</returns>
        DataResult<T> Get<T>(ClientOperation state, ApplicationModel model) where T : InformationHeader, new();

   
        /// <summary>
        /// Get a list of (latest version) application data that matches the filter specified in args. 
        /// This function supports paging. It returns the number of records specified in args.BatchSize
        /// 
        /// If args.OwnerUserId is set only applications owned by that OwnerUserId will be returned
        /// </summary>
        /// <returns>A DataListResult including a string json array</returns>
        DataListResult GetJsonArray(ClientOperation args);

        /// <summary>
        /// Get a list of (latest version) application data that matches the filter specified in args. 
        /// This function supports paging. It returns the number of records specified in args.BatchSize
        /// 
        /// If args.OwnerUserId is set only applications owned by that OwnerUserId will be returned
        /// </summary>
        /// <returns>A DataListResult including a string json array</returns>
        DataListResult GetJsonArray(ClientOperation args, ApplicationModel model);


        /// <summary>
        /// Get a list of (latest version) application data that matches the filter specified in args. 
        /// This function supports paging. It returns the number of records specified in args.BatchSize
        /// 
        /// If args.OwnerUserId is set only applications owned by that OwnerUserId will be returned
        /// </summary>
        /// <returns>A result object that inhertits DataListResult including a string json array</returns>
        TDataListResult GetJsonArray<TDataListResult>(ClientOperation args, ApplicationModel model) where TDataListResult : DataListResult, new();



        /// <summary>
        /// Get a list of (latest version) application data that matches the filter specified in args. 
        /// This function supports paging. It returns the number of records specified in args.BatchSize
        /// 
        /// If args.OwnerUserId is set only applications owned by that OwnerUserId will be returned
        /// </summary>
        /// <returns>A DataListResult including a list of T and the current paging rownum</returns>
        DataListResult<T> GetEntityList<T>(ClientOperation args, ApplicationModel model) where T : InformationHeader, new();


        /// <summary>
        /// Get all value domain items.
        /// </summary>
        /// <returns>A list of ValueDomainModelItem</returns>
        List<ValueDomainModelItem> GetValueDomains();

        /// <summary>
        /// Get all value domain items for one domain.
        /// </summary>
        /// <returns>A list of ValueDomainModelItem</returns>
        List<ValueDomainModelItem> GetValueDomain(string domainname);
        List<ValueDomainModelItem> GetValueDomain(string domainname, ClientOperation state);
        /// <summary>
        /// Generate a value domain based on a customized query.
        /// </summary>
        /// <returns>A list of ValueDomainModelItem</returns>
        List<ValueDomainModelItem> GetApplicationDomain(string domainname, ClientOperation state);


        /// <summary>
        /// Validates an application according to validation rules in the model
        /// </summary>
        /// <returns>OperationResult as the result of the validation</returns>
        ModifyResult Validate(ClientOperation state);

        IDataClient GetDataClient();


    }
}
