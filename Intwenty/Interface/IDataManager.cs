using Intwenty.Data.Dto;
using Intwenty.Model;

namespace Intwenty.Interface
{
   
    public interface IDataManager
    {

        OperationResult GetLatestVersionByOwnerUser(ApplicationModel model, ClientStateInfo state);

        OperationResult GetLatestVersionById(ApplicationModel model, ClientStateInfo state);

        OperationResult GetList(ApplicationModel model, ListRetrivalArgs args);

        OperationResult GetList(ApplicationModel model);

        OperationResult GetListByOwnerUser(ApplicationModel model, ListRetrivalArgs args);

        OperationResult GetListByOwnerUser(ApplicationModel model, string owneruserid);

        OperationResult GetVersionListById(ApplicationModel model, ClientStateInfo state);

        OperationResult GetVersion(ApplicationModel model, ClientStateInfo state);

        OperationResult Validate(ApplicationModel model, ClientStateInfo data);

        OperationResult Save(ApplicationModel model, ClientStateInfo data);

        OperationResult DeleteById(ApplicationModel model, ClientStateInfo state);

        OperationResult DeleteById(ApplicationModel model, int id, string dbname);

        OperationResult GetApplicationValueDomains(ApplicationModel model);

        OperationResult GetValueDomains();

        OperationResult GetDataView(ListRetrivalArgs args);

        OperationResult GetDataViewRecord(ListRetrivalArgs args);

    }

   

    

}
