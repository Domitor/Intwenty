using Intwenty.Model.Dto;
using Intwenty.Entity;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;
using Intwenty.Areas.Identity.Models;
using System.Threading.Tasks;

namespace Intwenty.Interface
{
    /// <summary>
    /// Interface for operations on meta data
    /// </summary>
    public interface IIntwentyModelService
    {
        public IntwentySettings Settings { get; }

        /// <summary>
        /// Returns localized application models that the current user has permission to use
        /// </summary>
        public Task<List<ApplicationModelItem>> GetLocalizedAuthorizedApplicationModelsAsync(ClaimsPrincipal claimprincipal);


        /// <summary>
        /// Returns application models that the current user has permission to use
        /// </summary>
        public Task<List<ApplicationModelItem>> GetAuthorizedApplicationModelsAsync(ClaimsPrincipal claimprincipal);
        public Task<List<SystemModelItem>> GetAuthorizedSystemModelsAsync(ClaimsPrincipal claimprincipal);
        public ExportModel GetExportModel();
        public OperationResult ImportModel(ExportModel model);
        public List<OperationResult> ConfigureDatabase();
        public OperationResult ConfigureDatabase(ApplicationModelItem model, List<DatabaseModelItem> databasemodel = null);


        /// <summary>
        /// Get a list of all system models
        /// </summary>
        public List<SystemModelItem> GetSystemModels();

        public void SaveSystemModel(SystemModelItem model);

        public void DeleteSystemModel(SystemModelItem model);



       



        //APPLICATION
        public List<ApplicationModel> GetApplicationModels();
        public ApplicationModel GetApplicationModel(int applicationid);
        public List<ApplicationModel> GetLocalizedApplicationModels();
        public ApplicationModel GetLocalizedApplicationModel(int applicationid);
        public List<ApplicationModelItem> GetAppModels();

        public ModifyResult SaveAppModel(ApplicationModelItem model);

        public void SetAppModelLocalizationKey(int id, string key);

        public void DeleteAppModel(ApplicationModelItem model);


        //DATABASE
        public List<DatabaseModelItem> GetDatabaseModels();

        public void SaveDatabaseModels(List<DatabaseModelItem> model, int applicationid);

        public void DeleteDatabaseModel(int id);



        //UI
        public List<ViewModel> GetViewModels();
        public ViewModel GetLocalizedViewModelByPath(string path);
        public void SetUserInterfaceModelLocalizationKey(int id, string key);


        //DATAVIEWS
        public List<DataViewModelItem> GetLocalizedDataViewModels();
        public List<DataViewModelItem> GetDataViewModels();
        public void SaveDataViewModels(List<DataViewModelItem> model);
        public void DeleteDataViewModel(int id);


        //VALUE DOMAINS
        public List<ValueDomainModelItem> GetValueDomains();
        public void SaveValueDomains(List<ValueDomainModelItem> model);

        public void DeleteValueDomain(int id);


        //TRANSLATIONS
        public List<TranslationModelItem> GetTranslations();

        public void SaveTranslations(List<TranslationModelItem> model);

        public void DeleteTranslation(int id);


        //ENDPOINTS
        public List<EndpointModelItem> GetEndpointModels();

        public void SaveEndpointModels(List<EndpointModelItem> model);

        public void DeleteEndpointModel(int id);



        public OperationResult ValidateModel();

        public List<IntwentyDataColumn> GetDefaultVersioningTableColumns();

        public void ClearCache(string key="ALL");

        public List<CachedObjectDescription> GetCachedObjectDescriptions();


    }
}
