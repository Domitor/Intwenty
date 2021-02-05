using Intwenty.Model.Dto;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Claims;
using Intwenty.Areas.Identity.Data;

namespace Intwenty.Model.Dto
{

    public class ClientStateInfo : HashTagPropertyObject
    {

        public int Id { get; set; }

        public int Version { get; set; }

        public string UserId { get; set; }

        public string UserTablePrefix { get; set; }

        public string OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationTablePrefix { get; set; }

        public int ApplicationId { get; set; }

        public int ApplicationViewId { get; set; }

        public ApplicationData Data { get; set; }

        public List<FilterValue> FilterValues { get; set; }


        public ClientStateInfo()
        {
            FilterValues = new List<FilterValue>();
            Data = new ApplicationData();
            UserId = ListFilter.DEFAULT_OWNERUSERID;
            Properties = "";
        }

        public ClientStateInfo(ClaimsPrincipal user)
        {
            Properties = "";
            FilterValues = new List<FilterValue>();
            Data = new ApplicationData();
            UserId = user.Identity.Name;
            UserTablePrefix = user.Identity.GetUserTablePrefix();
            OrganizationId = user.Identity.GetOrganizationId();
            OrganizationName = user.Identity.GetOrganizationName();
            OrganizationTablePrefix = user.Identity.GetOrganizationTablePrefix();
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
            state.ApplicationViewId = state.Data.GetAsInt("ApplicationViewId").Value;
            state.Id = state.Data.Id;
            state.Version = state.Data.Version;
            return state;
        }

        public static ClientStateInfo CreateFromJSON(System.Text.Json.JsonElement model, ClaimsPrincipal user)
        {
            var state = new ClientStateInfo(user);
            state.Data = ApplicationData.CreateFromJSON(model);
            state.ApplicationId = state.Data.ApplicationId;
            state.ApplicationViewId = state.Data.GetAsInt("ApplicationViewId").Value;
            state.Id = state.Data.Id;
            state.Version = state.Data.Version;
            return state;
        }


    }

   



}
