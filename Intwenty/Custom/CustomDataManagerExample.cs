using System.Collections.Generic;
using Intwenty.Engine;
using Intwenty.Model;
using Intwenty.Data.Dto;

namespace Intwenty.Custom
{


    public class AppDataManagerExample : DataManager
    {

        public AppDataManagerExample(ApplicationModel application) : base(application)
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

