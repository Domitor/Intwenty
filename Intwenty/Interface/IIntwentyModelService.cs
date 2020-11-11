using Intwenty.Model.Dto;
using Intwenty.Entity;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;
using Intwenty.Areas.Identity.Models;

namespace Intwenty.Interface
{
    /// <summary>
    /// Interface for operations on meta data
    /// </summary>
    public interface IIntwentyModelService
    {

        /// <summary>
        /// Returns application models that the current user has permission to use
        /// </summary>
        public List<ApplicationModelItem> GetAuthorizedApplicationsModels(ClaimsPrincipal claimprincipal, IntwentyPermission requested_permission);

        /// <summary>
        /// Returns application models that the current user has permission to use
        /// </summary>
        public List<ApplicationModelItem> GetAuthorizedApplicationModels(ClaimsPrincipal claimprincipal);

        /// <summary>
        /// Returns system models that the current user has permission to use
        /// </summary>
        public List<SystemModelItem> GetAuthorizedSystemsModels(ClaimsPrincipal claimprincipal);

        /// <summary>
        /// Get a complete system model, used for export model
        /// </summary>
        public ExportModel GetExportModel();

        /// <summary>
        /// Insert a complete system model, used for import model
        /// </summary>
        public OperationResult ImportModel(ExportModel model);


        /// <summary>
        /// Get a list of all system models
        /// </summary>
        public List<SystemModelItem> GetSystemModels();

        public void SaveSystemModel(SystemModelItem model);

        public void DeleteSystemModel(SystemModelItem model);


        /// <summary>
        /// Create database objects used for persisting the intwenty model
        /// </summary>
        public void CreateIntwentyDatabase();


        public List<OperationResult> ConfigureDatabase();

        public OperationResult ConfigureDatabase(ApplicationModelItem model, List<DatabaseModelItem> databasemodel = null);


        /// <summary>
        /// Get all application models
        /// </summary>
        public List<ApplicationModel> GetApplicationModels();

        /// <summary>
        /// Get all application models
        /// </summary>
        public List<ApplicationModel> GetLocalizedApplicationModels();


        public List<ApplicationModelItem> GetAppModels();

        public OperationResult SaveAppModel(ApplicationModelItem model);

        public void DeleteAppModel(ApplicationModelItem model);



        public List<DatabaseModelItem> GetDatabaseModels();

        public void SaveDatabaseModels(List<DatabaseModelItem> model, int applicationid);

        public void DeleteDatabaseModel(int id);




        public List<UserInterfaceModelItem> GetUserInterfaceModels();

        public void SaveUserInterfaceModels(List<UserInterfaceModelItem> model);



        public List<DataViewModelItem> GetLocalizedDataViewModels();
        public List<DataViewModelItem> GetDataViewModels();

        public void SaveDataViewModels(List<DataViewModelItem> model);

        public void DeleteDataViewModel(int id);



        public List<ValueDomainModelItem> GetValueDomains();
        public void SaveValueDomains(List<ValueDomainModelItem> model);

        public void DeleteValueDomain(int id);


        public List<TranslationModelItem> GetTranslations();

        public void SaveTranslations(List<TranslationModelItem> model);

        public void DeleteTranslation(int id);


        public List<EndpointModelItem> GetEndpointModels();

        public void SaveEndpointModels(List<EndpointModelItem> model);

        public void DeleteEndpointModel(int id);



        public List<MenuModelItem> GetLocalizedMenuModels();
        public List<MenuModelItem> GetMenuModels();



        public OperationResult ValidateModel();

        public List<IntwentyDataColumn> GetDefaultVersioningTableColumns();

        public void ClearCache(string key="ALL");


    }
}
