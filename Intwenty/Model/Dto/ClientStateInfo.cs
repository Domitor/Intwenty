using Intwenty.Model.Dto;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Claims;
using Intwenty.Areas.Identity.Data;
using Microsoft.IdentityModel.Tokens;

namespace Intwenty.Model.Dto
{

    public enum ActionModeOptions
    {
        AllTables, MainTable
    }


    public class ClientStateInfo : HashTagPropertyObject
    {
        public UserInfo User { get; set; }

        public int Id { get; set; }

        public int Version { get; set; }

        public int ApplicationId { get; set; }

        public int ApplicationViewId { get; set; }

        public ApplicationData Data { get; set; }

        public ActionModeOptions ActionMode { get; set; }

        public bool IgnoreTenantIsolation { get; set; }

        public ClientStateInfo()
        {
            Properties = "";
            Data = new ApplicationData();
            User = new UserInfo();
            ActionMode = ActionModeOptions.AllTables;
        }

        public ClientStateInfo(ClaimsPrincipal user)
        {
            Properties = "";
            Data = new ApplicationData();
            User = new UserInfo(user);
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
            state.ApplicationViewId = state.Data.GetAsInt("ApplicationViewId");
            state.Id = state.Data.Id;
            state.Version = state.Data.Version;
            state.Properties = state.Data.GetAsString("Properties");
            if (!string.IsNullOrEmpty(state.Properties))
            {
                state.Properties = Base64UrlEncoder.Decode(state.Properties);
               
            }

            return state;
        }

        public static ClientStateInfo CreateFromJSON(System.Text.Json.JsonElement model, ClaimsPrincipal user)
        {
            var state = new ClientStateInfo(user);
            state.Data = ApplicationData.CreateFromJSON(model);
            state.ApplicationId = state.Data.ApplicationId;
            state.ApplicationViewId = state.Data.GetAsInt("ApplicationViewId");
            state.Id = state.Data.Id;
            state.Version = state.Data.Version;
            state.Properties = state.Data.GetAsString("Properties");
            if (!string.IsNullOrEmpty(state.Properties))
            {
                state.Properties = Base64UrlEncoder.Decode(state.Properties);
            }

            return state;
        }


    }

    public class ClientStateInfo<TData> : HashTagPropertyObject 
    {
        public UserInfo User { get; set; }

        public int Id { get; set; }

        public int Version { get; set; }

        public int ApplicationId { get; set; }

        public int ApplicationViewId { get; set; }

        public TData Data { get; set; }


        public ClientStateInfo()
        {
            Properties = "";
            Data = default(TData);
            User = new UserInfo();
        }

        public ClientStateInfo(TData applicationdata)
        {
            Data = applicationdata;
            User = new UserInfo();
            Properties = "";
        }

        public ClientStateInfo(ClaimsPrincipal user)
        {
            Properties = "";
            Data = default(TData);
            User = new UserInfo(user);

        }


    }





}
