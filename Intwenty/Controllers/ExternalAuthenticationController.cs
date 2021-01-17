using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Model.Dto;
using Microsoft.AspNetCore.Http;
using Intwenty.Interface;
using Microsoft.Extensions.Primitives;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Model;
using Intwenty.DataClient.Model;
using Intwenty.DataClient;
using Intwenty.Areas.Identity.Data;

namespace Intwenty.Controllers
{
   
    public class ExternalAuthenticationController : Controller
    {
        private IIntwentyDataService DataRepository { get; }
        private IntwentyUserManager UserManager { get; }
        private IntwentyOrganizationManager OrganizationManager { get; }

        public ExternalAuthenticationController(IIntwentyDataService dataservice, IntwentyUserManager usermanager, IntwentyOrganizationManager orgmanager)
        {
            DataRepository = dataservice;
            UserManager = usermanager;
            OrganizationManager = orgmanager;
        }


        /// <summary>
        /// Login and return the user API key
        /// Example: { "username":"user1@intwenty.dev","password":"lowride" }
        /// </summary>
        [HttpPost("API/Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] System.Text.Json.JsonElement data)
        {

            var ip = GetIpAddress();
            var state = ClientStateInfo.CreateFromJSON(data);

            if (!state.HasData)
            {
                DataRepository.LogInfo(string.Format("Call to API/Login without valid data, IP {0}", ip));
                return BadRequest();
            }

            var username = state.Data.GetAsString("username");
            var password = state.Data.GetAsString("password");
            var productid = state.Data.GetAsString("productid");


            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(productid))
            {
                DataRepository.LogInfo(string.Format("Call to API/Login without both username, password and productid, IP {0}", ip));
                return BadRequest();
            }

            var users = await UserManager.GetUsersAsync();
            var user = users.Find(p => p.NormalizedUserName == username.ToUpper());
            if (user == null)
            {
                DataRepository.LogInfo(string.Format("Call to API/Login with invalid username {0}, IP {1}", username, ip));
                return Unauthorized();
            }

            var client = DataRepository.GetIAMDataClient();
            await client.OpenAsync();
            var products = await client.GetEntitiesAsync<IntwentyProduct>();
            await client.CloseAsync();
            if (!products.Exists(p=>p.Id==productid))
            {
                DataRepository.LogInfo(string.Format("Call to API/Login with invalid productid {0}, IP {1}", productid, ip));
                return Unauthorized();
            }

            if (!await OrganizationManager.IsProductUser(productid, user))
            {
                DataRepository.LogInfo(string.Format("Call to API/Login without being product user {0}, IP {1}", username, ip));
                return Unauthorized();
            }

            var productinfo = await OrganizationManager.GetOrganizationProductAsync(productid, user.Id);
            if (productinfo == null)
            {
                DataRepository.LogError(string.Format("Could not find the organization product, IP {0}", ip));
                return Unauthorized();
            }

            var result = UserManager.CheckPasswordAsync(user, password).Result;
            if (result)
            {
                if (string.IsNullOrEmpty(user.APIKey))
                {
                    return Unauthorized();
                }

                DataRepository.LogInfo(string.Format("User {0} logged in via api, IP {1}", user.UserName, ip));

                var returnobject = new AuthenticatedProductInfo() { UserApiKey = user.APIKey, UserFullName = user.FullName, ForceAppVersion = "1.0.0", ProductURI = productinfo.ProductURI, ProductAPIPath = productinfo.APIPath, Organization = "", ProductId = productid, ProductName = productinfo.ProductName };

                return new JsonResult(returnobject);

            }
            else
            {
                DataRepository.LogInfo(string.Format("Call to API/Login with invalid username or password, IP {0}", ip));
                return Unauthorized();
            }

        }

        private string GetIpAddress()
        {
            try
            {
                return HttpContext.Connection.RemoteIpAddress.ToString();
            }
            catch (Exception ex)
            {
                DataRepository.LogError("ExternalAuthenticationController - GetIpAddress: " + ex.Message);
            }

            return string.Empty;
        }

    }
}
