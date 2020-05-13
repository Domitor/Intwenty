using Intwenty.Data.Dto;
using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Intwenty.Data.Dto
{

    public class ClientStateInfo
    {
        public static readonly string DEFAULT_USERID = "SYSTEM";

        public int Id { get; set; }

        public int Version { get; set; }

        public string UserId { get; set; }

        public int ApplicationId { get; set; }

        public string Properties { get; set; }

        public ApplicationData Data { get; set; }

        private string _owneruserid { get; set; }

        public ClientStateInfo()
        {
            Data = new ApplicationData();
            UserId = DEFAULT_USERID;
            OwnerUserId = DEFAULT_USERID;
            Properties = "";
        }

        public string OwnerUserId
        {
            set { _owneruserid = value; }

            get
            {
                if (_owneruserid == DEFAULT_USERID && !string.IsNullOrEmpty(UserId))
                    return UserId;
                else
                    return _owneruserid;

            }
        }

        public bool HasData
        {
            get
            {
                return Data != null && Data.HasData;
            }
        }

        public bool HasDataAndModel
        {
            get
            {
                return   Data != null && Data.HasDataAndModel;
            }
        }

       

        public static ClientStateInfo CreateFromJSON(System.Text.Json.JsonElement model)
        {
            var state = new ClientStateInfo();
            state.Data = ApplicationData.CreateFromJSON(model);
            state.ApplicationId = state.Data.ApplicationId;
            state.Id = state.Data.Id;
            state.Version = state.Data.Version;
            return state;
        }


    }

   



}
