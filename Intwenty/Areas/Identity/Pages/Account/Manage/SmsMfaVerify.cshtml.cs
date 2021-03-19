using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Intwenty.Areas.Identity.Entity;
using Intwenty.Areas.Identity.Data;

namespace Intwenty.Areas.Identity.Pages.Account.Manage
{
    public class SmsMfaVerifyModel : PageModel
    {
        private readonly IntwentyUserManager _userManager;

        public SmsMfaVerifyModel(IntwentyUserManager userManager)
        {
            _userManager = userManager;
        }

       

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Verification Code")]
            public string Code { get; set; }

            public string PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string phonenumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            StatusMessage = "We sent a code to your mobile phone, please input the code below.";

            Input = new InputModel();
            Input.PhoneNumber = phonenumber;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _userManager.ChangePhoneNumberAsync(user, Input.PhoneNumber, Input.Code);
            if (result.Succeeded)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                await _userManager.AddUpdateUserSetting(user, "SMSMFA", "TRUE");
                StatusMessage = "Sms mfa is now set up.";
            }
            else
            {
                await _userManager.AddUpdateUserSetting(user, "SMSMFA", "FALSE");
                StatusMessage = "Error - That did not work, please try again and make sure your phonenumber is correct.";
                return RedirectToPage("./SmsMfaEnable");
            }

            return RedirectToPage("./MfaAuth");
            
        }

    }
}
