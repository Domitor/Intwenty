using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Interface;
using Intwenty.Areas.Identity.Data;
using Intwenty.Services;

namespace Intwenty.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly IntwentyUserManager _userManager;
        private readonly IIntwentyEventService _eventService;

        public ForgotPasswordModel(IntwentyUserManager usermanager, IIntwentyEventService eventservice)
        {
            _userManager = usermanager;
            _eventService = eventservice;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }


                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page("/Account/ResetPassword",pageHandler: null,values: new { area = "Identity", code },protocol: Request.Scheme);
                await _eventService.UserRequestedPasswordReset(new UserRequestedPasswordResetData() { Email=Input.Email, UserName = user.UserName, ConfirmCallbackUrl = callbackUrl });
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
