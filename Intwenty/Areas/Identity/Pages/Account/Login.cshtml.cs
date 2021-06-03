using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Intwenty.Model;
using Microsoft.Extensions.Options;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Areas.Identity.Data;
using Intwenty.Interface;
using Intwenty.Areas.Identity.Models;
using System.Security.Claims;
using Intwenty.Model.BankId;

namespace Intwenty.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IIntwentyDbLoggerService _dbloggerService;
        private readonly IIntwentyDataService _dataService;
        private readonly IntwentySignInManager _signInManager;
        private readonly IntwentyUserManager _userManager;
        private readonly IOptions<IntwentySettings> _settings;
        private readonly IFrejaClientService _frejaClient;
        private readonly IBankIDClientService _bankidClient;

        public LoginModel(IntwentySignInManager signinmanager, 
                          IIntwentyDataService dataservice, 
                          IOptions<IntwentySettings> settings, 
                          IntwentyUserManager usermanager, 
                          IIntwentyDbLoggerService dblogger,
                          IFrejaClientService frejaclient,
                          IBankIDClientService bankIdclient)
        {
            _dataService = dataservice;
            _signInManager = signinmanager;
            _settings = settings;
            _userManager = usermanager;
            _dbloggerService = dblogger;
            _frejaClient = frejaclient;
            _bankidClient = bankIdclient;


        }

      
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        public string AuthServiceQRCode { get; set; }

        public string AuthServiceUrl { get; set; }

        public string Method { get; set; }

        [TempData]
        public string ExternalAuthReference { get; set; }


        private string GetExternalIP() 
        {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            if (string.IsNullOrEmpty(ip))
                return _settings.Value.BankIdClientExternalIP;
            if (ip.StartsWith(":"))
                return _settings.Value.BankIdClientExternalIP;
            if (ip.Length < 9)
                return _settings.Value.BankIdClientExternalIP;

            return ip;

        }

        public async Task OnGetAsync(string returnUrl = null, string method="")
        {
            ExternalAuthReference = string.Empty;

            returnUrl = returnUrl ?? Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            if (_settings.Value.UseExternalLogins)
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            }

            if (_settings.Value.UseFrejaEIdLogin)
            {
                var externalauthref = await _frejaClient.InitAuthentication();
                ExternalAuthReference = externalauthref.authRef;
                if (!string.IsNullOrEmpty(externalauthref.authRef))
                {
                    this.AuthServiceQRCode = _frejaClient.GetQRCode(externalauthref.authRef).OriginalString;
                }
            }

            if (_settings.Value.UseBankIdLogin)
            {
                if (method == "BID_THIS")
                {
                    var request = new BankIDAuthRequest();
                    request.EndUserIp = GetExternalIP();
                    var externalauthref = await _bankidClient.InitAuthentication(request);
                    ExternalAuthReference = string.Format("{0}{1}", "BID_", externalauthref.OrderRef);
                    if (!string.IsNullOrEmpty(externalauthref.AutoStartToken))
                    {
                        var b64qr = _bankidClient.GetQRCode(externalauthref.AutoStartToken);
                        this.AuthServiceQRCode = b64qr;
                    }
                }
                else if (method == "BID_OTHER")
                {
                    var request = new BankIDAuthRequest();
                    request.EndUserIp = GetExternalIP();
                    var externalauthref = await _bankidClient.InitAuthentication(request);
                    ExternalAuthReference = string.Format("{0}{1}", "BID_", externalauthref.OrderRef);
                    if (!string.IsNullOrEmpty(externalauthref.AutoStartToken))
                    {
                        this.AuthServiceUrl = string.Format("bankid:///?autostarttoken={0}&redirect=null", externalauthref.AutoStartToken);
                    }
                }


            }

            Method = method;
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnGetFrejaLogin()
        {
            var model = new IntwentyLoginModel() { AccountType = AccountTypes.FrejaEId, ResultCode = "SUCCESS" };

            try
            {
          
                if (string.IsNullOrEmpty(ExternalAuthReference))
                {
                    model.ResultCode = "FREJA_NO_AUTHREF";
                    return new JsonResult(model) { StatusCode = 503 };
                }

                var authresult = await _frejaClient.Authenticate(ExternalAuthReference);
                if (authresult != null)
                {

                    var client = _userManager.GetIAMDataClient();

                    IntwentyUser attemptinguser = null;
                    await client.OpenAsync();
                    var userlist = await client.GetEntitiesAsync<IntwentyUser>();
                    attemptinguser = userlist.Find(p => p.NormalizedEmail == authresult.emailAddress.ToUpper());
                    await client.CloseAsync();

                    if (attemptinguser == null)
                    {
                        model.ResultCode = "INVALID_LOGIN_ATTEMPT";
                        return new JsonResult(model) { StatusCode = 401 };
                    }

                    var result = await _signInManager.SignInFrejaId(attemptinguser, ExternalAuthReference);
                    if (result.IsNotAllowed)
                    {
                        model.ResultCode = "INVALID_LOGIN_ATTEMPT";
                        return new JsonResult(model) { StatusCode = 401 };
                    }
                    else if (result.RequiresTwoFactor)
                    {
                        model.ResultCode = "REQUIREMFA";
                        model.RedirectUrl = "./LoginWith2fa";
                        return new JsonResult(model) { StatusCode = 401 };
                    }
                    else
                    {
                        model.ReturnUrl = Url.Content("~/");
                        model.ResultCode = "SUCCESS";
                        await _dbloggerService.LogIdentityActivityAsync("INFO", string.Format("User {0} logged in with Freja e Id", attemptinguser.UserName), attemptinguser.UserName);
                        return new JsonResult(model);

                    }
                }
                else
                {
                    model.ResultCode = "INVALID_LOGIN_ATTEMPT";
                    return new JsonResult(model) { StatusCode = 401 };
                }

            }
            catch (Exception ex)
            {
                await _dbloggerService.LogIdentityActivityAsync("ERROR", "Error on login.OnGetFrejaLogin: " + ex.Message);
            }

            model.ResultCode = "UNEXPECTED_ERROR";
            return new JsonResult(model) { StatusCode = 500 };
        }

        public async Task<IActionResult> OnGetBankIdLogin()
        {
            var model = new IntwentyLoginModel() { AccountType = AccountTypes.BankId, ResultCode = "SUCCESS" };

            try
            {
                var authref = "";
                if (!string.IsNullOrEmpty(ExternalAuthReference))
                {
                    if (ExternalAuthReference.Contains("BID_"))
                    {
                        authref = ExternalAuthReference.Substring(4);
                    }
                }

                if (string.IsNullOrEmpty(authref))
                {
                    model.ResultCode = "BANKID_NO_AUTHREF";
                    return new JsonResult(model) { StatusCode = 503 };
                }

                var request = new BankIDCollectRequest();
                request.OrderRef = authref;

                var authresult = await _bankidClient.Authenticate(request);
                if (authresult != null)
                {

                    var client = _userManager.GetIAMDataClient();

                    var attemptinguser = await _userManager.GetUserWithSettingValue("SWEPNR", authresult.CompletionData.User.PersonalNumber);

                    if (attemptinguser == null)
                    {
                        model.ResultCode = "INVALID_LOGIN_ATTEMPT";
                        return new JsonResult(model) { StatusCode = 401 };
                    }

                    var result = await _signInManager.SignInBankId(attemptinguser, ExternalAuthReference);
                    if (result.IsNotAllowed)
                    {
                        model.ResultCode = "INVALID_LOGIN_ATTEMPT";
                        return new JsonResult(model) { StatusCode = 401 };
                    }
                    else if (result.RequiresTwoFactor)
                    {
                        model.ResultCode = "REQUIREMFA";
                        model.RedirectUrl = "./LoginWith2fa";
                        return new JsonResult(model) { StatusCode = 401 };
                    }
                    else
                    {
                        model.ReturnUrl = Url.Content("~/");
                        model.ResultCode = "SUCCESS";
                        await _dbloggerService.LogIdentityActivityAsync("INFO", string.Format("User {0} logged in with swedish Bank ID", attemptinguser.UserName), attemptinguser.UserName);
                        return new JsonResult(model);

                    }
                }
                else
                {
                    model.ResultCode = "INVALID_LOGIN_ATTEMPT";
                    return new JsonResult(model) { StatusCode = 401 };
                }

            }
            catch (Exception ex)
            {
                await _dbloggerService.LogIdentityActivityAsync("ERROR", "Error on login.OnGetBankIdLogin: " + ex.Message);
            }

            model.ResultCode = "UNEXPECTED_ERROR";
            return new JsonResult(model) { StatusCode = 500 };
        }

        public async Task<IActionResult> OnPostLocalLogin([FromBody] IntwentyLoginModel model)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                model.ResultCode = "MISSING_USERNAME_OR_PWD";
                return new JsonResult(model) { StatusCode = 401 };
            }

            var client = _userManager.GetIAMDataClient();

            IntwentyUser attemptinguser = null;
            await client.OpenAsync();
            var userlist = await client.GetEntitiesAsync<IntwentyUser>();
            attemptinguser = userlist.Find(p => p.UserName == model.UserName);
            await client.CloseAsync();

            if (attemptinguser==null)
            {
                model.ResultCode = "INVALID_LOGIN_ATTEMPT";
                return new JsonResult(model) { StatusCode = 401 };
            }

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                await _dbloggerService.LogIdentityActivityAsync("INFO", string.Format("User {0} logged in with password", model.UserName), model.UserName);
                if (attemptinguser != null)
                {
                    attemptinguser.LastLoginProduct = _settings.Value.ProductId;
                    attemptinguser.LastLogin = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    await client.OpenAsync();
                    await client.UpdateEntityAsync(attemptinguser);
                    await client.CloseAsync();
                }

                model.ResultCode = "SUCCESS";
                return new JsonResult(model);
            }
            if (result.RequiresTwoFactor)
            {
                model.ResultCode = "REQUIREMFA";
                model.RedirectUrl = "./LoginWith2fa";
                return new JsonResult(model) { StatusCode = 401 };

            }
            if (result.IsLockedOut)
            {
                model.ResultCode = "LOCKEDOUT";
                model.RedirectUrl = "./Lockout";
                return new JsonResult(model) { StatusCode = 401 };
            }
            else
            {
                if (attemptinguser != null && _settings.Value.AccountsRequireConfirmed && !attemptinguser.EmailConfirmed)
                {
                    model.ResultCode = "REQUIRECONFIRMATION";
                   
                }
                else
                {
                    model.ResultCode = "INVALID_LOGIN_ATTEMPT";
                   
                }

                await _dbloggerService.LogIdentityActivityAsync("INFO", string.Format("Failed log in attempt with password, user: {0}", model.UserName), model.UserName);


                return new JsonResult(model) { StatusCode = 401 };
            }
        }

     
    }
}
