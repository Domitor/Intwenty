using Intwenty.Data.Dto;
using Intwenty.Data.Entity;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Interface
{
    /// <summary>
    /// Interface for operations on meta data
    /// </summary>
    public interface IIntwentyModelService
    {
        /// <summary>
        /// Get a complete system model, used for export model
        /// </summary>
        /// <returns></returns>
        public SystemModel GetSystemModel();

        /// <summary>
        /// Insert a complete system model, used for import model
        /// </summary>
        public OperationResult InsertSystemModel(SystemModel model);


        /// <summary>
        /// Create database objects used for persisting the intwenty model
        /// </summary>
        public void CreateIntwentyDatabase();


        public List<OperationResult> ConfigureDatabase();

        public OperationResult ConfigureDatabase(ApplicationModel model);


        /// <summary>
        /// Get all application models
        /// </summary>
        public List<ApplicationModel> GetApplicationModels();



        public List<ApplicationModelItem> GetAppModels();

        public ApplicationModelItem SaveAppModel(ApplicationModelItem model);

        public void DeleteAppModel(ApplicationModelItem model);



        public List<DatabaseModelItem> GetDatabaseModels();

        public void SaveDatabaseModels(List<DatabaseModelItem> model, int applicationid);

        public void DeleteDatabaseModel(int id);




        public List<UserInterfaceModelItem> GetUserInterfaceModels();

        public void SaveUserInterfaceModels(List<UserInterfaceModelItem> model);




        public List<DataViewModelItem> GetDataViewModels();

        public void SaveDataViewModels(List<DataViewModelItem> model);

        public void DeleteDataViewModel(int id);



        public List<ValueDomainModelItem> GetValueDomains();
        public void SaveValueDomains(List<ValueDomainModelItem> model);

        public void DeleteValueDomain(int id);


        public List<TranslationModelItem> GetTranslations();

        public void SaveTranslations(List<TranslationModelItem> model);

        public void DeleteTranslation(int id);




        public List<MenuModelItem> GetMenuModels();


        public OperationResult ValidateModel();

        //public List<IntwentyDataColumn> GetDefaultMainTableColumns();


        //public List<IntwentyDataColumn> GetDefaultSubTableColumns();


        public List<IntwentyDataColumn> GetDefaultVersioningTableColumns();


    }
}
