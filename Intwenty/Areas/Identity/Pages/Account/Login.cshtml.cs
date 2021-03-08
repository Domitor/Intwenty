using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Intwenty.Model;
using Microsoft.Extensions.Options;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Areas.Identity.Data;
using Intwenty.Interface;

namespace Intwenty.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        public readonly IIntwentyDataService _dataService;
        private readonly SignInManager<IntwentyUser> _signInManager;
        private readonly IntwentyUserManager _userManager;
        private readonly IOptions<IntwentySettings> _settings;

        public LoginModel(SignInManager<IntwentyUser> signInManager,
            IIntwentyDataService dataservice,
            IOptions<IntwentySettings> settings,
            IntwentyUserManager userManager)
        {
            _dataService = dataservice;
            _signInManager = signInManager;
            _settings = settings;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Display(Name = "Email")]
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Display(Name = "Password")]
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
          
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();


            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var client = _dataService.GetIAMDataClient();

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    await _dataService.LogIdentityActivity("INFO", string.Format("User {0} logged in with password",Input.Email), Input.Email);
                    client.Open();
                    var signedinuser = client.GetEntities<IntwentyUser>().Find(p => p.NormalizedEmail == Input.Email.ToUpper());
                    if (signedinuser != null)
                    {
                        signedinuser.LastLoginProduct = _settings.Value.ProductId;
                        signedinuser.LastLogin = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        client.UpdateEntity(signedinuser);
                    }
                    client.Close();

                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    client.Open();
                    var signedinuser = client.GetEntities<IntwentyUser>().Find(p => p.NormalizedEmail == Input.Email.ToUpper());
                    if (signedinuser != null)
                    {
                        signedinuser.LastLogin = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        client.UpdateEntity(signedinuser);
                    }
                    client.Close();
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    await _dataService.LogIdentityActivity("INFO", string.Format("Failed log in attempt with password, user: {0}", Input.Email), Input.Email);
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                    ReturnUrl = returnUrl;
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
