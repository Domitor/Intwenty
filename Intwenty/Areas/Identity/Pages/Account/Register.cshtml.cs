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
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Intwenty.Areas.Identity.Models;
using Intwenty.Model;
using Microsoft.Extensions.Options;
using Intwenty.Areas.Identity.Data;
using Intwenty.Model.Dto;
using Intwenty.SystemEvents;

namespace Intwenty.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IntwentyUser> _signInManager;
        private readonly IntwentyUserManager _userManager;
        private readonly IntwentySettings _settings;
        private readonly IIntwentySystemEventService _eventservice;

        public RegisterModel(
            IntwentyUserManager userManager,
            SignInManager<IntwentyUser> signInManager,
            IIntwentySystemEventService eventservice,
            IOptions<IntwentySettings> settings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _eventservice = eventservice;
            _settings = settings.Value;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {

            [Display(Name = "Language")]
            public string Language { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task  OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public IActionResult OnPost([FromBody] RegisterVm model)
        {
            try
            {


                model.Message = "";
                model.ReturnUrl = Url.Content("~/");

                var user = new IntwentyUser { UserName = model.Email, Email = model.Email, Culture = model.Language };
                if (string.IsNullOrEmpty(user.Culture))
                    user.Culture = _settings.DefaultCulture;


                var result = _userManager.CreateAsync(user, model.Password).Result;
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(_settings.NewUserRoles))
                    {
                        var roles = _settings.NewUserRoles.Split(",".ToCharArray());
                        foreach (var r in roles)
                        {
                            _userManager.AddToRoleAsync(user, r);
                        }

                    }

                    var code = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    _eventservice.NewUserCreated(new NewUserCreatedData() { UserName = model.Email, ConfirmCallbackUrl = callbackUrl });
                    _signInManager.SignInAsync(user, isPersistent: false);

                  
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        throw new InvalidOperationException(error.Description);
                    }
                }

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError("", ex.Message);
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return new JsonResult(model);

        }
    }
}
