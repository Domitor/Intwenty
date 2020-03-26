﻿using System.Collections.Generic;
using Intwenty.Models;
using Intwenty.MetaDataService.Engine;
using Intwenty.MetaDataService.Model;

namespace Intwenty.MetaDataService.Custom
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

        public override OperationResult GetLatestVersion()
        {
            return base.GetLatestVersion();
        }

        public override OperationResult GetListView(ListRetrivalArgs args)
        {
            return base.GetListView(args);
        }

        public override OperationResult GetVersion()
        {
            return base.GetVersion();
        }

        public override OperationResult Save(ApplicationData data)
        {
            return base.Save(data);
        }

        protected override void BeforeSave(ApplicationData data)
        {
            base.BeforeSave(data);
        }

        protected override void BeforeSaveNew(DataAccessClient da, ApplicationData data)
        {
            base.BeforeSaveNew(da, data);
        }

        protected override void BeforeSaveUpdate(DataAccessClient da, ApplicationData data)
        {
            base.BeforeSaveUpdate(da, data);
        }


        protected override void AfterSave(ApplicationData data)
        {
            base.AfterSave(data);
        }

    }

 }

