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

        public string QRCodeUrl { get; set; }

        [TempData]
        public string ExternalAuthenticationReference { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ExternalAuthenticationReference = string.Empty;

            returnUrl = returnUrl ?? Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            if (_settings.Value.UseExternalLogins)
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            }

            if (_settings.Value.UseFrejaEIdLogin)
            {
                var externalauthref = await _frejaClient.InitQRAuthentication();
                ExternalAuthenticationReference = externalauthref.authRef;
                if (!string.IsNullOrEmpty(ExternalAuthenticationReference))
                {
                    this.QRCodeUrl = _frejaClient.GetQRCode(ExternalAuthenticationReference).OriginalString;
                }
            }

            if (_settings.Value.UseBankIdLogin)
            {
                var request = new BankIDAuthRequest();
                request.EndUserIp = "62.20.76.122";
                var externalauthref = await _bankidClient.InitQRAuthentication(request);
                ExternalAuthenticationReference = externalauthref.AutoStartToken;
                if (!string.IsNullOrEmpty(ExternalAuthenticationReference))
                {
                    var b64qr = _bankidClient.GetQRCode(ExternalAuthenticationReference);
                    this.QRCodeUrl = b64qr;
                }
            }

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnGetFrejaLogin()
        {
            var model = new IntwentyLoginModel() { AccountType = AccountTypes.FrejaEId, ResultCode = "SUCCESS" };

            try
            {
                if (string.IsNullOrEmpty(ExternalAuthenticationReference))
                {
                    model.ResultCode = "FREJA_NO_AUTHREF";
                    return new JsonResult(model) { StatusCode = 503 };
                }

                var authresult = await _frejaClient.Authenticate(ExternalAuthenticationReference);
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

                    var result = await _signInManager.SignInFrejaId(attemptinguser, ExternalAuthenticationReference);
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
            var model = new IntwentyLoginModel() { AccountType = AccountTypes.FrejaEId, ResultCode = "SUCCESS" };

            try
            {
                if (string.IsNullOrEmpty(ExternalAuthenticationReference))
                {
                    model.ResultCode = "BANKID_NO_AUTHREF";
                    return new JsonResult(model) { StatusCode = 503 };
                }

                var request = new BankIDCollectRequest();
                request.OrderRef = ExternalAuthenticationReference;

                var authresult = await _bankidClient.Authenticate(request);
                if (authresult != null)
                {

                    var client = _userManager.GetIAMDataClient();

                    IntwentyUser attemptinguser = null;
                    await client.OpenAsync();
                    var userlist = await client.GetEntitiesAsync<IntwentyUser>();
                    attemptinguser = userlist.Find(p => p.NormalizedEmail == _settings.Value.DemoAdminUser);
                    await client.CloseAsync();

                    if (attemptinguser == null)
                    {
                        model.ResultCode = "INVALID_LOGIN_ATTEMPT";
                        return new JsonResult(model) { StatusCode = 401 };
                    }

                    var result = await _signInManager.SignInBankId(attemptinguser, ExternalAuthenticationReference);
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

        /*
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");


            if (ModelState.IsValid)
            {
                var client = _userManager.GetIAMDataClient();

                IntwentyUser attemptinguser = null;
                await client.OpenAsync();
                var userlist = await client.GetEntitiesAsync<IntwentyUser>();
                attemptinguser = userlist.Find(p => p.UserName == Input.UserName);
                await client.CloseAsync();

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    await _dbloggerService.LogIdentityActivityAsync("INFO", string.Format("User {0} logged in with password",Input.UserName), Input.UserName);
                    if (attemptinguser != null)
                    {
                        attemptinguser.LastLoginProduct = _settings.Value.ProductId;
                        attemptinguser.LastLogin = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        await client.OpenAsync();
                        await client.UpdateEntityAsync(attemptinguser);
                        await client.CloseAsync();
                    }

                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    if (attemptinguser != null && _settings.Value.RequireConfirmedAccount && !attemptinguser.EmailConfirmed)
                    {
                        ErrorMessage = "You must confirm your account by clicking the link in the confirmation email we sent you";
                        ModelState.AddModelError(string.Empty, ErrorMessage);
                    }
                    else
                    {
                        ErrorMessage = "No user with that combination";
                        ModelState.AddModelError(string.Empty, ErrorMessage);
                    }

                    await _dbloggerService.LogIdentityActivityAsync("INFO", string.Format("Failed log in attempt with password, user: {0}", Input.UserName), Input.UserName);
                   
                    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                    ReturnUrl = returnUrl;
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        */
    }
}
