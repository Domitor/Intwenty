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

        protected override void BeforeSave(ApplicationModel model, ClientStateInfo data, IDataClient client)
        {
            if (model.Application.MetaCode == "CUSTOMER")
            {
                //GIVE ALL CUSTOMERS THE SAME PHONE NO (Very useful)
                data.Data.SetValue("CustomerPhone", 0767345678);
            }
            else
                base.BeforeSave(model, data, client);
        }

        protected override OperationResult Validate(ApplicationModel model, ClientStateInfo state)
        {
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
