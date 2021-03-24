﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Intwenty.Areas.Identity.Data;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Interface;
using Intwenty.Model;
using Intwenty.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Intwenty.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginWith2faModel : PageModel
    {
        private readonly IIntwentyDbLoggerService _dbloggerService;
        private readonly IntwentySignInManager _signInManager;
        private readonly IntwentyUserManager _userManager;
        private readonly IntwentySettings _settings;
        private readonly IIntwentyEventService _eventService;

        public LoginWith2faModel(IntwentySignInManager siginmanager, IntwentyUserManager usermanager, IIntwentyEventService eventservice, IOptions<IntwentySettings> settings, IIntwentyDbLoggerService dblogger)
        {
            _signInManager = siginmanager;
            _userManager = usermanager;
            _eventService = eventservice;
            _settings = settings.Value;
            _dbloggerService = dblogger;
        }

        public bool HasAnyMFA { get; set; }
        public bool HasBankIdMFA { get; set; }
        public bool HasSmsMFA { get; set; }
        public bool HasEmailMFA { get; set; }
        public bool HasFido2MFA { get; set; }
        public bool HasTotpMFA { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            public string TwoFactorCode { get; set; }
            public bool RememberMachine { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            ReturnUrl = returnUrl;

            RememberMe = rememberMe;

            HasBankIdMFA = await _userManager.HasUserSettingWithValue(user, "BANKIDMFA", "TRUE");
            HasSmsMFA = await _userManager.HasUserSettingWithValue(user, "SMSMFA", "TRUE");
            HasEmailMFA = await _userManager.HasUserSettingWithValue(user, "EMAILMFA", "TRUE");
            HasFido2MFA = await _userManager.HasUserSettingWithValue(user, "FIDO2MFA", "TRUE");
            HasTotpMFA = await _userManager.HasUserSettingWithValue(user, "TOTPMFA", "TRUE");

            if (HasBankIdMFA || HasSmsMFA || HasEmailMFA || HasFido2MFA || HasTotpMFA)
                HasAnyMFA = true;

            if (HasSmsMFA)
            {
                var code = await _userManager.GenerateTwoFactorTokenAsync(user, _userManager.Options.Tokens.ChangePhoneNumberTokenProvider);
                await _eventService.UserRequestedSmsMfaCode(new UserRequestedSmsMfaCodeData() { Code = code, PhoneNumber = user.PhoneNumber, UserName = user.UserName });
            }
            else if (HasEmailMFA)
            {
                var code = await _userManager.GenerateTwoFactorTokenAsync(user, _userManager.Options.Tokens.ChangePhoneNumberTokenProvider);
                await _eventService.UserRequestedEmailMfaCode(new UserRequestedEmailMfaCodeData() { Code = code, Email = user.Email, UserName = user.UserName });
            }
            else
            {

            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            Microsoft.AspNetCore.Identity.SignInResult result = null;
            if (await _userManager.HasUserSettingWithValue(user, "SMSMFA", "TRUE")) 
            {
                result = await _signInManager.TwoFactorSignInAsync(_userManager.Options.Tokens.ChangePhoneNumberTokenProvider, Input.TwoFactorCode, true, Input.RememberMachine);
            }
            else if (await _userManager.HasUserSettingWithValue(user, "EMAILMFA", "TRUE"))
            {
                result = await _signInManager.TwoFactorSignInAsync(_userManager.Options.Tokens.ChangePhoneNumberTokenProvider, Input.TwoFactorCode, true, Input.RememberMachine);
            }
            else 
            {
                var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
                result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);

            }

        
            if (result.Succeeded)
            {
                if (user != null)
                {
                    user.LastLoginProduct = _settings.ProductId;
                    user.LastLogin = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var client = _userManager.GetIAMDataClient();
                    await client.OpenAsync();
                    await client.UpdateEntityAsync(user);
                    await client.CloseAsync();
                }

                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                if (_settings.RequireConfirmedAccount && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Code or unconfirmed account.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Code.");
                }
               
                return Page();
            }

          
            
        }
    }
}
