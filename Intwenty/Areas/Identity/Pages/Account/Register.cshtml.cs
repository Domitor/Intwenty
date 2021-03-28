using System;
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

        public RegisterModel(
            IntwentyUserManager userManager,
            IntwentySignInManager signInManager,
            IIntwentyEventService eventservice,
            IIntwentyOrganizationManager orgmanager,
            IOptions<IntwentySettings> settings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _eventservice = eventservice;
            _settings = settings.Value;
            _organizationManager = orgmanager;
        }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        public async Task  OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPost([FromBody] RegisterVm model)
        {
            try
            {
                if (!_settings.AccountsAllowRegistration)
                {
                    throw new InvalidOperationException("User registration is closed");
                }

                model.Message = "";
                model.ReturnUrl = Url.Content("~/");

                var user = new IntwentyUser { UserName = model.Email, Email = model.Email, Culture = model.Language };
                if (_settings.AccountsRegistrationRequireName)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                }
                if (!_settings.AccountsUseEmailAsUserName)
                {
                    user.UserName = model.UserName;
                }

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
    }
}
