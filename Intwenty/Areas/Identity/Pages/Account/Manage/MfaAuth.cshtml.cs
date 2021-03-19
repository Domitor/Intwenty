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
using Intwenty.Model;
using Microsoft.Extensions.Options;

namespace Intwenty.Areas.Identity.Pages.Account.Manage
{
    public class TwoFactorAuthenticationModel : PageModel
    {
        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}";

        private IntwentyUserManager UserManager { get; }
        private IntwentySignInManager SignInManager { get; }
        private IntwentySettings Settings { get; }

        public TwoFactorAuthenticationModel(
            IntwentyUserManager userManager,
            IntwentySignInManager signinmanager,
            IOptions<IntwentySettings> settings)
        {
            UserManager = userManager;
            SignInManager = signinmanager;
            Settings = settings.Value;
        }

        public bool HasAnyMFA { get; set; }
        public bool HasBankIdMFA{ get; set; }
        public bool HasSmsMFA { get; set; }
        public bool HasEmailMFA { get; set; }
        public bool HasFido2MFA { get; set; }
        public bool HasTotpMFA { get; set; }

        public bool IsMachineRemembered { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await UserManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
            }

            HasBankIdMFA = await UserManager.HasUserSettingWithValue(user, "BANKIDMFA", "TRUE");
            HasSmsMFA = await UserManager.HasUserSettingWithValue(user, "SMSMFA", "TRUE");
            HasEmailMFA = await UserManager.HasUserSettingWithValue(user, "EMAILMFA", "TRUE");
            HasFido2MFA = await UserManager.HasUserSettingWithValue(user, "FIDO2MFA", "TRUE");
            HasTotpMFA = await UserManager.HasUserSettingWithValue(user, "TOTPMFA", "TRUE");

            if (HasBankIdMFA || HasSmsMFA || HasEmailMFA || HasFido2MFA || HasTotpMFA)
                HasAnyMFA = true;


             var t = await UserManager.GetTwoFactorEnabledAsync(user);
            if (!t && HasAnyMFA)
                await UserManager.SetTwoFactorEnabledAsync(user, true);
            if (t && !HasAnyMFA)
                await UserManager.SetTwoFactorEnabledAsync(user, false);

            IsMachineRemembered = await SignInManager.IsTwoFactorClientRememberedAsync(user);


            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            return Page();
        }
    }
}