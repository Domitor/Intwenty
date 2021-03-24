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

        public LoginModel(IntwentySignInManager signInManager, IIntwentyDataService dataservice, IOptions<IntwentySettings> settings, IntwentyUserManager userManager, IIntwentyDbLoggerService dblogger)
        {
            _dataService = dataservice;
            _signInManager = signInManager;
            _settings = settings;
            _userManager = userManager;
            _dbloggerService = dblogger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {

            [Required]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ErrorMessage = string.Empty;

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            ErrorMessage = string.Empty;

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
    }
}
