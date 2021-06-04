﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Areas.Identity.Models;
using Intwenty.Model;
using Microsoft.Extensions.Options;
using Intwenty.Areas.Identity.Data;
using Intwenty.Model.Dto;
using Intwenty.Services;
using Intwenty.Interface;
using Intwenty.Model.BankId;

namespace Intwenty.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IntwentySignInManager _signInManager;
        private readonly IntwentyUserManager _userManager;
        private readonly IntwentySettings _settings;
        private readonly IIntwentyEventService _eventservice;
        private readonly IIntwentyOrganizationManager _organizationManager;
        private readonly IFrejaClientService _frejaClient;
        private readonly IBankIDClientService _bankidClient;
        private readonly IIntwentyDbLoggerService _dbloggerService;

        public RegisterModel(
            IntwentyUserManager userManager,
            IntwentySignInManager signInManager,
            IIntwentyEventService eventservice,
            IIntwentyOrganizationManager orgmanager,
            IOptions<IntwentySettings> settings,
            IFrejaClientService frejaclient,
            IBankIDClientService bankIdclient,
            IIntwentyDbLoggerService dblogger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _eventservice = eventservice;
            _settings = settings.Value;
            _organizationManager = orgmanager;
            _frejaClient = frejaclient;
            _bankidClient = bankIdclient;
            _dbloggerService = dblogger;
        }

        public string ReturnUrl { get; set; }

        [TempData]
        public string AuthServiceOrderRef { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        private string GetExternalIP()
        {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            if (string.IsNullOrEmpty(ip))
                return _settings.BankIdClientExternalIP;
            if (ip.StartsWith(":"))
                return _settings.BankIdClientExternalIP;
            if (ip.Length < 9)
                return _settings.BankIdClientExternalIP;

            return ip;

        }

        public async Task  OnGetAsync(string returnUrl = null)
        {
            AuthServiceOrderRef = string.Empty;
            ReturnUrl = returnUrl;
            if (_settings.UseExternalLogins)
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostLocalRegistration([FromBody] RegisterVm model)
        {
            try
            {
                if (!_settings.AccountsAllowRegistration)
                    return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "Sorry, user registration is closed !")) { StatusCode = 500 };

                model.Message = "";
                model.ReturnUrl = Url.Content("~/");

                var user = new IntwentyUser { UserName = model.Email, Email = model.Email, Culture = model.Language };
                if (_settings.AccountsRegistrationRequireName)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                }
                if (!_settings.AccountsUseEmailAsUserName)
                    user.UserName = model.UserName;
                

                if (string.IsNullOrEmpty(user.Culture))
                    user.Culture = _settings.LocalizationDefaultCulture;


                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var org = await _organizationManager.FindByNameAsync(_settings.ProductOrganization);
                    if (org != null)
                    {
                        if (!string.IsNullOrEmpty(_settings.AccountsRegistrationAssignRoles))
                        {

                            var roles = _settings.AccountsRegistrationAssignRoles.Split(",".ToCharArray());
                            foreach (var r in roles)
                            {
                                await _userManager.AddUpdateUserRoleAuthorizationAsync(r, user.Id, org.Id, _settings.ProductId);
                            }
                        }

                        await _organizationManager.AddMemberAsync(new IntwentyOrganizationMember() { OrganizationId = org.Id, UserId = user.Id, UserName = user.UserName });
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { area = "Identity", userId = user.Id, code = code }, protocol: Request.Scheme);
                    await _eventservice.NewUserCreated(new NewUserCreatedData() { UserName = model.Email, ConfirmCallbackUrl = callbackUrl });
                    await _signInManager.SignInAsync(user, isPersistent: false);


                }
                else
                {
                    if (result.Errors != null && result.Errors.Count() > 0)
                    {
                        var errors = result.Errors.ToList();
                        return new JsonResult(new OperationResult(false, MessageCode.USERERROR, errors[0].Description)) { StatusCode = 500 };
                    } 
                    else 
                    {
                        throw new InvalidOperationException("Unexpected error registering user");
                    }
                }

            }
            catch
            {
                return new JsonResult(new OperationResult(false, MessageCode.USERERROR, "An unexpected error occured, contact support")) { StatusCode = 500 };
            }

            return new JsonResult(model);

        }


        public async Task<IActionResult> OnPostInitBankId([FromBody] RegisterVm model)
        {
            try
            {
                if (!_settings.UseBankIdLogin)
                    throw new InvalidOperationException("Bank ID is not active in settings");

                if (model.ActionCode != "BANKID_INIT_REG")
                    throw new InvalidOperationException("Invalid action");


                model.AccountType = AccountTypes.BankId;
                model.Message = "";
                model.ReturnUrl = Url.Content("~/");
                model.ResultCode = "BANKID_START_REG";

                return new JsonResult(model);

            }
            catch(Exception ex)
            {
                await _dbloggerService.LogIdentityActivityAsync("ERROR", "Error on Register.OnPostInitBankId: " + ex.Message);
            }

            model.ResultCode = "ERROR_BANKID_INIT_REG";
            return new JsonResult(model) { StatusCode = 401 };

         

        }

        public async Task<IActionResult> OnPostStartBankId([FromBody] RegisterVm model)
        {
            try
            {

                if (!_settings.UseBankIdLogin)
                    throw new InvalidOperationException("Bank ID is not active in settings");

                if (model.ActionCode == "BANKID_START_OTHER")
                {
                    var request = new BankIDAuthRequest();
                    request.EndUserIp = GetExternalIP();
                    var externalauthref = await _bankidClient.InitAuthentication(request);
                    AuthServiceOrderRef = string.Format("{0}{1}", "BID_", externalauthref.OrderRef);
                    var b64qr = _bankidClient.GetQRCode(externalauthref.AutoStartToken);
                    model.AuthServiceQRCode = b64qr;
                    if (string.IsNullOrEmpty(model.AuthServiceQRCode))
                        throw new InvalidOperationException("Could not generate bankid QR Code");

                    model.ResultCode = "BANKID_AUTH_QR";
                }
                else if (model.ActionCode == "BANKID_START_THIS")
                {
                    var request = new BankIDAuthRequest();
                    request.EndUserIp = GetExternalIP();
                    var externalauthref = await _bankidClient.InitAuthentication(request);
                    AuthServiceOrderRef = string.Format("{0}{1}", "BID_", externalauthref.OrderRef);
                    model.AuthServiceUrl = string.Format("bankid:///?autostarttoken={0}&redirect=null", externalauthref.AutoStartToken);
                    model.ResultCode = "BANKID_AUTH_BUTTON";
                }

                return new JsonResult(model);

                

            }
            catch (Exception ex)
            {
                await _dbloggerService.LogIdentityActivityAsync("ERROR", "Error on Register.OnPostStartBankId: " + ex.Message);
            }


            model.ResultCode = "ERROR_BANKID_START";
            return new JsonResult(model) { StatusCode = 500 };
        }

        public async Task<IActionResult> OnPostAuthenticateBankId([FromBody] RegisterVm model)
        {


            try
            {
                if (!_settings.UseBankIdLogin)
                    throw new InvalidOperationException("Bank ID is not active in settings");

                model.ResultCode = "SUCCESS";

                var authref = "";
                if (!string.IsNullOrEmpty(AuthServiceOrderRef))
                    authref = AuthServiceOrderRef.Substring(4);

                if (string.IsNullOrEmpty(authref))
                {
                    model.ResultCode = "BANKID_NO_AUTHREF";
                    return new JsonResult(model) { StatusCode = 503 };
                }


                var request = new BankIDCollectRequest() { OrderRef = authref };


                //TODO: Handle wrong, code, time out
                var authresult = await _bankidClient.Authenticate(request);
                if (authresult != null)
                {
                    //TODO: RETURN BANKID_TIMEOUT_FAILURE ?
                    //TODO: RETURN BANKID_AUTH_FAILURE ?

                    var attemptinguser = await _userManager.GetUserWithSettingValue("SWEPNR", authresult.CompletionData.User.PersonalNumber);
                    if (attemptinguser == null)
                    {
                        var user = new IntwentyUser { UserName = model.Email, Email = model.Email, Culture = model.Language };                      
                        user.FirstName = authresult.CompletionData.User.Name;
                        user.LastName = authresult.CompletionData.User.Surname;
                        
                        if (!_settings.AccountsUseEmailAsUserName)
                            user.UserName = model.UserName;                       

                        if (string.IsNullOrEmpty(user.Culture))
                            user.Culture = _settings.LocalizationDefaultCulture;

                        var result = await _userManager.CreateAsync(user);
                        if (result.Succeeded)
                        {
                            var org = await _organizationManager.FindByNameAsync(_settings.ProductOrganization);
                            if (org != null)
                            {
                                if (!string.IsNullOrEmpty(_settings.AccountsRegistrationAssignRoles))
                                {
                                    var roles = _settings.AccountsRegistrationAssignRoles.Split(",".ToCharArray());
                                    foreach (var r in roles)
                                    {
                                        await _userManager.AddUpdateUserRoleAuthorizationAsync(r, user.Id, org.Id, _settings.ProductId);
                                    }
                                }

                                await _organizationManager.AddMemberAsync(new IntwentyOrganizationMember() { OrganizationId = org.Id, UserId = user.Id, UserName = user.UserName });
                            }

                            await _userManager.AddUpdateUserSetting(user, "SWEPNR", authresult.CompletionData.User.PersonalNumber);

                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                            var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { area = "Identity", userId = user.Id, code = code }, protocol: Request.Scheme);
                            await _eventservice.NewUserCreated(new NewUserCreatedData() { UserName = model.Email, ConfirmCallbackUrl = callbackUrl });
                            await _signInManager.SignInBankId(user, authref);

                            model.ReturnUrl = Url.Content("~/");
                            model.ResultCode = "SUCCESS";
                            await _dbloggerService.LogIdentityActivityAsync("INFO", string.Format("User {0} created an account and signed in using swedish Bank ID", user.UserName), user.UserName);
                            return new JsonResult(model);


                        }
                        else
                        {
                            throw new InvalidOperationException("Unexpected error registering user");
                        }
                    }
                    else
                    {
                        var result = await _signInManager.SignInBankId(attemptinguser, authref);
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
                        else if (result.IsLockedOut)
                        {
                            model.ResultCode = "LOCKEDOUT";
                            model.RedirectUrl = "./Lockout";
                            return new JsonResult(model) { StatusCode = 401 };
                        }
                        else
                        {
                            model.ReturnUrl = Url.Content("~/");
                            model.ResultCode = "SUCCESS";
                            await _dbloggerService.LogIdentityActivityAsync("INFO", string.Format("User {0} tried to create account with swedish Bank ID, but it was already present", attemptinguser.UserName), attemptinguser.UserName);
                            return new JsonResult(model);

                        }

                    }


                }
                else
                {
                    model.ResultCode = "SERVICE_FAILURE";
                    return new JsonResult(model) { StatusCode = 401 };
                }

            }
            catch (Exception ex)
            {
                await _dbloggerService.LogIdentityActivityAsync("ERROR", "Error on Register.OnPostAuthenticateBankId: " + ex.Message);
            }

            model.ResultCode = "ERROR_BANKID_AUTH";
            return new JsonResult(model) { StatusCode = 500 };



        }

    }
}
