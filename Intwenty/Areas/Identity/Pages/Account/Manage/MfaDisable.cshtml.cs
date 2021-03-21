using System;
using System.Collections.Generic;
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
    public class MfaDisableModel : PageModel
    {
        private readonly IntwentyUserManager _userManager;

        public MfaDisableModel(IntwentyUserManager userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            await _userManager.RemoveUserSetting(user, "BANKIDMFA");
            await _userManager.RemoveUserSetting(user, "SMSMFA");
            await _userManager.RemoveUserSetting(user, "EMAILMFA");
            await _userManager.RemoveUserSetting(user, "FIDO2MFA");
            await _userManager.RemoveUserSetting(user, "TOTPMFA");
            await _userManager.SetTwoFactorEnabledAsync(user, false);
           

            StatusMessage = "2fa has been disabled.";

            return RedirectToPage("./MfaAuth");
        }
    }
}