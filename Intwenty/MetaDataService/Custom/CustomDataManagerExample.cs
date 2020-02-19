using System.Collections.Generic;
using Moley.Data.Dto;
using Moley.Models;
using Moley.MetaDataService.Engine;
using Moley.MetaDataService.Engine.Common;

namespace Moley.MetaDataService.Custom
{


    public class AppDataManagerExample : DataManager
    {

        public AppDataManagerExample(ApplicationDto application) : base(application)
        {
        }


        public override OperationResult GetDomains(List<MetaDataViewDto> viewinfo)
        {
            return base.GetDomains(viewinfo);
        }

        public override OperationResult GetLatestVersion()
        {
            return base.GetLatestVersion();
        }

        public override OperationResult GetList(ListRetrivalArgs args)
        {
            return base.GetList(args);
        }

        public override OperationResult GetVersion()
        {
            return base.GetVersion();
        }

        public override OperationResult Save(Dictionary<string, object> data)
        {
            return base.Save(data);
        }

        protected override void BeforeSave(Dictionary<string, object> data)
        {
            base.BeforeSave(data);
        }

        protected override void BeforeSaveNew(DataAccessService da, Dictionary<string, object> data)
        {
            base.BeforeSaveNew(da, data);
        }

        protected override void BeforeSaveUpdate(DataAccessService da, Dictionary<string, object> data)
        {
            base.BeforeSaveUpdate(da, data);
        }


        protected override void AfterSave(Dictionary<string, object> data)
        {
            base.AfterSave(data);
        }

    }

 }

