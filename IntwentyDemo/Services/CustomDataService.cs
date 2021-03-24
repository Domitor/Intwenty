﻿using Azure.Storage.Blobs.Models;
using Intwenty;
using Intwenty.Model.Dto;
using Intwenty.Interface;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Intwenty.DataClient;
using Intwenty.DataClient.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace IntwentyDemo.Services
{

    /*
     * Example of overriding the intwenty default dataservice to customize handling of untyped applications (Intwenty Applications)
     * Don't forget to configure dependency injection for it in the startup class
     */

    public class CustomDataService : IntwentyDataService, IIntwentyDataService
    {
        public CustomDataService(IOptions<IntwentySettings> settings, IIntwentyModelService modelservice, IIntwentyDbLoggerService dblogger, IMemoryCache cache) : base(settings, modelservice, dblogger, cache)
        {
            
        }

        /// <summary>
        /// Implement APPDOMAINS
        /// </summary>
        public override List<ValueDomainModelItem> GetApplicationDomain(string domainname, ClientStateInfo state)
        {
            var client = this.GetDataClient();

            if (domainname == "VENDOR")
            {
                client.Open();
                var data = client.GetEntities<ValueDomainModelItem>("select Id, VendorId Code, VendorName Value from wms_Vendor");
                client.Close();
                foreach (var t in data)
                    t.LocalizedTitle = t.Code + " - " + t.Value;
                return data;
            }

            if (domainname == "CUSTOMER")
            {
                client.Open();
                var data = client.GetEntities<ValueDomainModelItem>("select Id, CustomerId Code, CustomerName Value from wms_Customer");
                client.Close();
                foreach (var t in data)
                    t.LocalizedTitle = t.Code + " - " + t.Value;
                return data;
            }


            if (domainname == "ITEM")
            {
                client.Open();
                var data = client.GetEntities<ValueDomainModelItem>("select Id, ItemId Code, ItemName Value from wms_Item");
                client.Close();
                foreach (var t in data)
                    t.LocalizedTitle = t.Code + " - " + t.Value;
                return data;
            }

            return base.GetApplicationDomain(domainname, state);
        }

        /// <summary>
        /// EXAMPLE
        /// Set CustomerStatus when a new customer isadded
        /// </summary>
        protected override void BeforeSaveNew(ApplicationModel model, ClientStateInfo state, IDataClient client)
        {
            if (model.Application.MetaCode == "CUSTOMER")
            {
                var dbmodel = ModelRepository.GetDatabaseColumnModel(model, "CustomerStatus");
                state.Data.SetValue(dbmodel, "NEW");
            }
            else
                base.BeforeSave(model, state, client);
        }

        /// <summary>
        /// EXAMPLE
        /// Change CustomerStatus when order is saved
        /// </summary>
        protected override void AfterSave(ApplicationModel model, ClientStateInfo state, IDataClient client)
        {
            try
            {
                if (model.Application.MetaCode == "SALESORDER")
                {
                    var customerid = state.Data.GetAsString("CustomerId");
                    if (string.IsNullOrEmpty(customerid))
                        return;

                    client.RunCommand(string.Format("update wms_customer set customerstatus='ACTIVE' where customerid='{0}'", customerid));

                    var customer = client.GetResultSet(string.Format("select * from customer where customerid='{0}'", customerid));

                    RemoveFromApplicationCache(10, customer.FirstRowGetAsInt("Id").Value);

                }
                else
                    base.BeforeSave(model, state, client);
            }
            catch { }
        }

        /// <summary>
        /// EXAMPLE
        /// Set ItemName on order row when a row is saved if ItemName is missing
        /// </summary>
        protected override void BeforeSaveSubTableRow(ApplicationModel model, ApplicationTableRow row, IDataClient client)
        {
            if (model.Application.MetaCode != "SALESORDER")
                return;

            if (!row.Table.HasModel)
                return;

            if (row.Table.Model.MetaCode != "DTORDLINE")
                return;

            var itemid = row.GetAsString("ItemNo");
            if (string.IsNullOrEmpty(itemid))
                return;

            var itemname = row.GetAsString("ItemName");
            if (!string.IsNullOrEmpty(itemname))
                return;

            var item = client.GetResultSet("select * from wms_item where ItemId=@P1", parameters: new IIntwentySqlParameter[] { new IntwentySqlParameter("@P1", itemid) });
            if (!item.HasRows)
                return;

            row.SetValue("ItemName", item.FirstRowGetAsString("ItemName"));


        }



        /// <summary>
        /// Example
        /// Serverside validation
        /// </summary>
        protected override ModifyResult Validate(ApplicationModel model, ClientStateInfo state)
        {
            if (state.HasProperty("ISIMPORT"))
                return new ModifyResult(true, MessageCode.RESULT, "Successfully validated", state.Id, state.Version);

            if (model.Application.MetaCode == "CUSTOMER")
            {
                var dv = state.Data.Values.FirstOrDefault(p => p.DbName == "CustomerEmail");
                if ((dv != null && !dv.HasValue) || dv == null)
                {
                    return new ModifyResult(false, MessageCode.USERERROR, string.Format("The field {0} is mandatory", "Email"), state.Id, state.Version);
                }
                else
                {
                    return new ModifyResult(true, MessageCode.RESULT, "Successfully validated", state.Id, state.Version);
                }
            }
            else
                return base.Validate(model, state);
        }


    }
}
