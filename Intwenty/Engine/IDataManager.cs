using Intwenty.Data.Dto;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Engine
{
    public enum LifecycleStatus
    {
        NONE = 0
       , NEW_NOT_SAVED = 1
       , NEW_SAVED = 2
       , EXISTING_NOT_SAVED = 3
       , EXISTING_SAVED = 4
       , DELETED_NOT_SAVED = 5
       , DELETED_SAVED = 6
    }

    public interface IDataManager
    {
        public ClientStateInfo ClientState { get; set; }

        OperationResult ConfigureDatabase();

        OperationResult GetLatestIdByOwnerUser(ClientStateInfo state);

        OperationResult GetLatestVersion(ClientStateInfo state);

        OperationResult GetVersion();

        OperationResult GetList(ListRetrivalArgs args);

        OperationResult Save(ClientStateInfo data);

        OperationResult GetValueDomains();

        OperationResult GetDataView(List<DataViewModelItem> viewinfo, ListRetrivalArgs args);

        OperationResult GetDataViewValue(List<DataViewModelItem> viewinfo, ListRetrivalArgs args);

        OperationResult GenerateTestData(int gencount);

    }

    

}
