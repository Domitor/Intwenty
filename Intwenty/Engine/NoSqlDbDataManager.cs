using Intwenty.Data.Dto;
using Intwenty.Model;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Engine
{
    public class NoSqlDbDataManager : IDataManager
    {
        protected IIntwentyModelService ModelRepository { get; set; }

        protected ApplicationModel Model { get; set; }

        protected SystemSettings Settings { get; set; }

        public ClientStateInfo ClientState { get; set; }

        protected LifecycleStatus Status { get; set; }

        protected bool CanRollbackVersion { get; set; }

        protected DateTime ApplicationSaveTimeStamp { get; set; }


        protected NoSqlDbDataManager(ApplicationModel model, IIntwentyModelService modelservice, SystemSettings settings)
        {
            Settings = settings;
            Model = model;
            ModelRepository = modelservice;
            ApplicationSaveTimeStamp = DateTime.Now;
        }

        public static NoSqlDbDataManager GetDataManager(ApplicationModel model, IIntwentyModelService modelservice, SystemSettings settings)
        {


            if (model.Application.MetaCode == "XXXXX")
            {
                return null;
            }
            else
            {
                var t = new NoSqlDbDataManager(model, modelservice, settings);
                return t;
            }
        }

        public OperationResult ConfigureDatabase()
        {
            throw new NotImplementedException();
        }

        public OperationResult GenerateTestData(int gencount)
        {
            throw new NotImplementedException();
        }

        public OperationResult GetDataView(List<DataViewModelItem> viewinfo, ListRetrivalArgs args)
        {
            throw new NotImplementedException();
        }

        public OperationResult GetDataViewValue(List<DataViewModelItem> viewinfo, ListRetrivalArgs args)
        {
            throw new NotImplementedException();
        }

        public OperationResult GetLatestIdByOwnerUser(ClientStateInfo data)
        {
            throw new NotImplementedException();
        }

        public OperationResult GetLatestVersion(ClientStateInfo data)
        {
            throw new NotImplementedException();
        }

        public OperationResult GetListView(ListRetrivalArgs args)
        {
            throw new NotImplementedException();
        }

        public OperationResult GetValueDomains()
        {
            throw new NotImplementedException();
        }

        public OperationResult GetVersion()
        {
            throw new NotImplementedException();
        }

        public OperationResult Save(ClientStateInfo data)
        {
            throw new NotImplementedException();
        }
    }
}
