using Azure.Storage.Blobs.Models;
using Intwenty;
using Intwenty.Data.Dto;
using Intwenty.Data;
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

namespace IntwentyDemo.Data
{

    /*
     * Example of overriding the intwenty default dataservice to customize handling of untyped applications (Intwenty Applications)
     * Don't forget to configure dependency injection for it in the startup class
     */

    public class CustomDataService : IntwentyDataService, IIntwentyDataService
    {
        public CustomDataService(IOptions<IntwentySettings> settings, IIntwentyModelService modelservice, IMemoryCache cache) : base(settings, modelservice, cache)
        {
            
        }

        /// <summary>
        /// EXAMPLE
        /// Set CustomerStatus when a new customer isadded
        /// </summary>
        protected override void BeforeSaveNew(ApplicationModel model, ClientStateInfo state, IDataClient client)
        {
            if (model.Application.MetaCode == "CUSTOMER")
            {
                //GIVE ALL CUSTOMERS THE SAME PHONE NO (Very useful)
                state.Data.SetValue("CustomerStatus", "NEW");
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
                    client.RunCommand(string.Format("update customer set customerstatus='ACTIVE' where customerid='{0}'", customerid));
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

            var item = client.GetResultSet("select * from item where ItemId=@P1", parameters: new IIntwentySqlParameter[] { new IntwentySqlParameter("@P1", itemid) });
            if (!item.HasRows)
                return;

            row.SetValue("ItemName", item.FirstRowGetAsString("ItemName"));


        }



        /// <summary>
        /// Example
        /// Serverside validation
        /// </summary>
        protected override OperationResult Validate(ApplicationModel model, ClientStateInfo state)
        {
            if (state.HasProperty("ISIMPORT"))
                return new OperationResult(true, MessageCode.RESULT, "Successfully validated", state.Id, state.Version);

            if (model.Application.MetaCode == "CUSTOMER")
            {
                var dv = state.Data.Values.FirstOrDefault(p => p.DbName == "CustomerEmail");
                if ((dv != null && !dv.HasValue) || dv == null)
                {
                    return new OperationResult(false, MessageCode.USERERROR, string.Format("The field {0} is mandatory", "Email"), state.Id, state.Version);
                }
                else
                {
                    return new OperationResult(true, MessageCode.RESULT, "Successfully validated", state.Id, state.Version);
                }
            }
            else
                return base.Validate(model, state);
        }

    }
}
