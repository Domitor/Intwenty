using Intwenty.Model.Dto;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Claims;
using Intwenty.Areas.Identity.Data;

namespace Intwenty.Model.Dto
{

    public enum ApplicationRetrievalMode
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

        public ApplicationRetrievalMode RetrievalMode { get; set; }


        public ClientStateInfo()
        {
            Properties = "";
            Data = new ApplicationData();
            User = new UserInfo();
            RetrievalMode = ApplicationRetrievalMode.AllTables;
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
