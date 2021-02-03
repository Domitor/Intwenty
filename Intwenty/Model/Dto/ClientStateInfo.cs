using Intwenty.Model.Dto;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Intwenty.Model.Dto
{

    public class ClientStateInfo : HashTagPropertyObject
    {

        public int Id { get; set; }

        public int Version { get; set; }

        public string UserId { get; set; }

        public string OrganizationId { get; set; }

        public int ApplicationId { get; set; }

        public ApplicationData Data { get; set; }

        public List<FilterValue> FilterValues { get; set; }


        public ClientStateInfo()
        {
            FilterValues = new List<FilterValue>();
            Data = new ApplicationData();
            UserId = ListFilter.DEFAULT_OWNERUSERID;
            Properties = "";
        }

  
        public bool HasData
        {
            get
            {
                return Data != null && Data.HasData;
            }
        }



        public static ClientStateInfo CreateFromJSON(System.Text.Json.JsonElement model)
        {
            var state = new ClientStateInfo();
            state.Data = ApplicationData.CreateFromJSON(model);
            state.ApplicationId = state.Data.ApplicationId;
            state.Id = state.Data.Id;
            state.Version = state.Data.Version;
            state.UserId = state.Data.OwnerUserId;
            return state;
        }


    }

   



}
