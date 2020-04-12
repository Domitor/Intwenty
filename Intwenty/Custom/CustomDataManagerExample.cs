using System.Collections.Generic;
using Intwenty.Engine;
using Intwenty.Model;
using Intwenty.Data.Dto;
using Intwenty.Data.DBAccess;
using Shared;

namespace Intwenty.Custom
{


    public class CustomSqlDbDataManagerExample : SqlDbDataManager
    {

        public CustomSqlDbDataManagerExample(ApplicationModel model, IIntwentyModelService modelservice, SystemSettings settings, IntwentyDBClient sqlclient) : base(model,modelservice,settings, sqlclient)
        {
        }


        public override OperationResult GetValueDomains()
        {
            return base.GetValueDomains();
        }

        public override OperationResult GetLatestVersion(ClientStateInfo data)
        {
            return base.GetLatestVersion(data);
        }

        public override OperationResult GetListView(ListRetrivalArgs args)
        {
            return base.GetListView(args);
        }

        public override OperationResult GetVersion()
        {
            return base.GetVersion();
        }

        public override OperationResult Save(ClientStateInfo data)
        {
            return base.Save(data);
        }

        protected override void BeforeSave(ClientStateInfo data)
        {
            base.BeforeSave(data);
        }

        protected override void BeforeSaveNew(ClientStateInfo data)
        {
            base.BeforeSaveNew(data);
        }

        protected override void BeforeSaveUpdate(ClientStateInfo data)
        {
            base.BeforeSaveUpdate(data);
        }


        protected override void AfterSave(ClientStateInfo data)
        {
            base.AfterSave(data);
        }

    }

 }

