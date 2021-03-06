﻿
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Intwenty.Model.Dto;
using Microsoft.AspNetCore.Http;
using Intwenty;
using Intwenty.Interface;
using Intwenty.Areas.Identity.Data;
using IntwentyDemo.Models;

namespace IntwentyDemo.Controllers
{
   
    [AllowAnonymous]
    public class CustomAPIController : Controller
    {
        private IIntwentyDbLoggerService DbLogger { get; }
        private IIntwentyDataService DataRepository { get; }
        private IIntwentyModelService ModelRepository { get; }
        private IntwentyUserManager UserManager { get; }
        private IIntwentyOrganizationManager OrganizationManager { get; }

        public CustomAPIController(IIntwentyDataService dataservice, IIntwentyModelService modelservice, IntwentyUserManager usermanager, IIntwentyOrganizationManager orgmanager, IIntwentyDbLoggerService dblogger) 
        {
            DataRepository = dataservice;
            ModelRepository = modelservice;
            UserManager = usermanager;
            OrganizationManager = orgmanager;
            DbLogger = dblogger;
        }


        [HttpPost("MyAPI/PostAnyThing")]
        public IActionResult Implementation([FromBody] System.Text.Json.JsonElement model)
        {
            return new JsonResult("{}");
        }

        /// <summary>
        /// Login and return the user API key
        /// Example: {"username":"admin@aaaaaa.com","password":"bbbbbb","productid":"INTWENTYDEMO"}
        /// </summary>
        [HttpPost("MyAPI/Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] System.Text.Json.JsonElement data)
        {

            var ip = await GetIpAddress();
            var state = ClientOperation.CreateFromJSON(data);

            if (!state.HasData)
            {
                await DbLogger.LogInfoAsync(string.Format("Call to API/Login without valid data, IP {0}", ip));
                return BadRequest();
            }

            var username = state.Data.GetAsString("username");
            var password = state.Data.GetAsString("password");
            var productid = state.Data.GetAsString("productid");


            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(productid))
            {
                await DbLogger.LogInfoAsync(string.Format("Call to API/Login without both username, password and productid, IP {0}", ip));
                return BadRequest();
            }

            var users = await UserManager.GetUsersAsync();
            var user = users.Find(p => p.NormalizedUserName == username.ToUpper());
            if (user == null)
            {
                await DbLogger.LogInfoAsync(string.Format("Call to API/Authenticate with invalid username {0}, IP {1}", username, ip));
                return Unauthorized();
            }


            var user_org_products = await OrganizationManager.GetUserOrganizationProductsInfoAsync(user.Id, productid);
            if (user_org_products == null)
            {
                await DbLogger.LogInfoAsync(string.Format("Call to API/Authenticate the user is not member of an organization that uses the product: {0}, IP {1}", productid, ip));
                return Unauthorized();
            }
            if (user_org_products.Count == 0)
            {
                await DbLogger.LogInfoAsync(string.Format("Call to API/Authenticate the user is not member of an organization that uses the product: {0}, IP {1}", productid, ip));
                return Unauthorized();
            }
            if (user_org_products.Count > 1)
            {
                await DbLogger.LogInfoAsync(string.Format("Call to API/Authenticate with productid {0} returned the same product in multiple organizations, IP {1}", productid, ip));
            }



            var result = await UserManager.CheckPasswordAsync(user, password);
            if (result)
            {
                if (string.IsNullOrEmpty(user.APIKey))
                {
                    return Unauthorized();
                }

                await DbLogger.LogInfoAsync(string.Format("User {0} logged in via api, IP {1}", user.UserName, ip));

                var returnobject = new UserClientInfo() { UserApiKey = user.APIKey, UserFullName = user.FullName };
                returnobject.ProductInfo = user_org_products;
                return new JsonResult(returnobject);

            }
            else
            {
                await DbLogger.LogInfoAsync(string.Format("Call to API/Login with invalid username or password, IP {0}", ip));
                return Unauthorized();
            }

        }

        private async Task<string> GetIpAddress()
        {
            try
            {
                return HttpContext.Connection.RemoteIpAddress.ToString();
            }
            catch (Exception ex)
            {
                await DbLogger.LogErrorAsync("ExternalAuthenticationController - GetIpAddress: " + ex.Message);
            }

            return string.Empty;
        }



    }
        

}
