using Intwenty.Model.Dto;
using Intwenty.Entity;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;
using Intwenty.Areas.Identity.Models;
using System.Threading.Tasks;
using Intwenty.Areas.Identity.Entity;

namespace Intwenty.Interface
{
    /// <summary>
    /// Interface for operations on meta data
    /// </summary>
    public interface IIntwentyModelService
    {
        public IntwentySettings Settings { get; }
        public Task<List<ViewModel>> GetApplicationMenuAsync(ClaimsPrincipal claimprincipal);
        public Task<List<ViewModel>> GetAuthorizedViewModelsAsync(ClaimsPrincipal claimprincipal);
        public Task<List<ApplicationModelItem>> GetAuthorizedApplicationModelsAsync(ClaimsPrincipal claimprincipal);
        public Task<List<SystemModelItem>> GetAuthorizedSystemModelsAsync(ClaimsPrincipal claimprincipal);



        //APPLICATION
        public List<SystemModelItem> GetSystemModels();
        public List<ApplicationModel> GetApplicationModels();
        public ApplicationModel GetApplicationModel(int applicationid);
        public List<ApplicationModelItem> GetAppModels();



        //DATABASE
        public List<DatabaseModelItem> GetDatabaseModels();

<<<<<<< HEAD


        //UI
        public List<ViewModel> GetViewModels();
        public ViewModel GetLocalizedViewModelById(int id);
        public ViewModel GetLocalizedViewModelByMetaCode(string metacode);
        public ViewModel GetLocalizedViewModelByPath(string path);

=======


        //UI
        public List<ViewModel> GetViewModels();
        public ViewModel GetLocalizedViewModelById(int id);
        public ViewModel GetLocalizedViewModelByMetaCode(string metacode);
        public ViewModel GetLocalizedViewModelByPath(string path);



        //DATAVIEWS
        public List<DataViewModelItem> GetLocalizedDataViewModels();
        public List<DataViewModelItem> GetDataViewModels();

>>>>>>> master


        //VALUE DOMAINS
        public List<ValueDomainModelItem> GetValueDomains();
        public void SaveValueDomains(List<ValueDomainModelItem> model);
        public void DeleteValueDomain(int id);


        //TRANSLATIONS
        public List<TranslationModelItem> GetTranslations();



        //ENDPOINTS
        public List<EndpointModelItem> GetEndpointModels();


        //MISC
        public Task<List<OperationResult>> CreateTenantIsolatedTables(IntwentyUser user);
        public OperationResult ValidateModel();
        public List<IntwentyDataColumn> GetDefaultVersioningTableColumns();
        public void ClearCache(string key="ALL");
        public List<CachedObjectDescription> GetCachedObjectDescriptions();
        public ExportModel GetExportModel();
        public OperationResult ImportModel(ExportModel model);
        public Task<List<OperationResult>> ConfigureDatabase(string tableprefix = "");
        public Task<OperationResult> ConfigureDatabase(ApplicationModelItem model, List<DatabaseModelItem> databasemodel = null, string tableprefix = "");


    }
}
