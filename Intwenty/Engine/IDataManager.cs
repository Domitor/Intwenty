using Intwenty.Data.Dto;


namespace Intwenty.Engine
{
   

    public interface IDataManager
    {
       
        OperationResult ConfigureDatabase();

        OperationResult GetLatestVersionByOwnerUser(ClientStateInfo state);

        OperationResult GetLatestVersionById(ClientStateInfo state);

        OperationResult GetList(ListRetrivalArgs args);

        OperationResult GetList();

        OperationResult GetListByOwnerUser(ListRetrivalArgs args);

        OperationResult GetListByOwnerUser(string owneruserid);

        OperationResult GetVersionListById(ClientStateInfo state);

        OperationResult GetVersion(ClientStateInfo state);

        OperationResult Save(ClientStateInfo data);

        OperationResult GetApplicationValueDomains();

        OperationResult GetValueDomains();

        OperationResult GetDataView(ListRetrivalArgs args);

        OperationResult GetDataViewRecord(ListRetrivalArgs args);

        OperationResult GenerateTestData(int gencount, ClientStateInfo state);

    }

   

    

}
