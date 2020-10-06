﻿using System;
using Intwenty.Model;
using Intwenty.Data.Dto;
using Intwenty.DataClient;

namespace Intwenty.Engine.Custom
{


    public class CustomDataManagerExample : DbDataManager
    {

        public CustomDataManagerExample(ApplicationModel model, IIntwentyModelService modelservice, IntwentySettings settings, IDataClient client) : base(model,modelservice,settings, client)
        {
        }


        public override OperationResult GetValueDomains()
        {
            return base.GetValueDomains();
        }

        public override OperationResult GetLatestVersionById(ClientStateInfo state)
        {
            return base.GetLatestVersionById(state);
        }

        public override OperationResult GetLatestVersionByOwnerUser(ClientStateInfo state)
        {
            return base.GetLatestVersionByOwnerUser(state);
        }

        public override OperationResult GetList(ListRetrivalArgs args)
        {
            return base.GetList(args);
        }

        public override OperationResult GetList()
        {
            return base.GetList();
        }

        public override OperationResult GetVersion()
        {
            return base.GetVersion();
        }

        public override OperationResult Save(ClientStateInfo state)
        {
            return base.Save(state);
        }

        protected override void BeforeSave(ClientStateInfo state)
        {
            base.BeforeSave(state);
        }

        protected override void BeforeSaveNew(ClientStateInfo state)
        {
            base.BeforeSaveNew(state);
        }

        protected override void BeforeSaveUpdate(ClientStateInfo state)
        {
            base.BeforeSaveUpdate(state);
        }


        protected override void AfterSave(ClientStateInfo state)
        {
            base.AfterSave(state);
        }

    }

 }

