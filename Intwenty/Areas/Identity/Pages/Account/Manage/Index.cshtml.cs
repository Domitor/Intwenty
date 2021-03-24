using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Areas.Identity.Data;
using Intwenty.Model;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Intwenty.Interface;
using Intwenty.Services;
using Intwenty.Helpers;

namespace Intwenty.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly IntwentyUserManager _userManager;
        private readonly IntwentySignInManager _signInManager;
        private readonly IntwentySettings _settings;
        private readonly IIntwentyEventService _eventService;

        public IndexModel(IntwentyUserManager usermanager, IntwentySignInManager signinmanager, IOptions<IntwentySettings> settings, IIntwentyEventService eventservice)
        {
            _userManager = usermanager;
            _signInManager = signinmanager;
            _settings = settings.Value;
            _eventService = eventservice;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            public string PhoneNumber { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }
        }

        private void Load(IntwentyUser user)
        {

            Username = user.UserName;

            Input = new InputModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            Load(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
           
            if (!ModelState.IsValid)
            {
                Load(user);
                return Page();
            }

            var emailconf = false;
            var doupdate = false;
            if (Input.Email != user.Email && !string.IsNullOrEmpty(Input.Email))
            {
                user.Email = Input.Email;
                doupdate = true;
                if (_settings.RequireConfirmedAccount)
                {
                    emailconf = true;
                }
            }

            if (Input.PhoneNumber != user.PhoneNumber && !string.IsNullOrEmpty(Input.PhoneNumber))
            {
                var phone = Input.PhoneNumber.GetCellPhone();
                if (phone == "INVALID")
                {
                    ModelState.AddModelError("Input.PhoneNumber", "Invalid phone number format.");
                    Load(user);
                    return Page();
                }
                user.PhoneNumber = phone;
                doupdate = true;
            }

            if (Input.FirstName != user.FirstName)
            {
                user.FirstName = Input.FirstName;
                doupdate = true;
            }

            if (Input.LastName != user.LastName)
            {
                user.LastName = Input.LastName;
                doupdate = true;
            }

            if (doupdate)
            {
                var updateresult = await _userManager.UpdateAsync(user);
                if (!updateresult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException("Unexpected error occurred updating user.");
                }
                else
                {
                    if (emailconf)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { area = "Identity", userId = user.Id, code = code }, protocol: Request.Scheme);
                        await _eventService.EmailChanged(new EmailChangedData() { UserName = user.Email, ConfirmCallbackUrl = callbackUrl });
                    }

                }


            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
