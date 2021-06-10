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
    public class MfaAuthModel : PageModel
    {

        private IntwentyUserManager UserManager { get; }
        private IntwentySignInManager SignInManager { get; }
        private IntwentySettings Settings { get; }

        public MfaAuthModel(
            IntwentyUserManager userManager,
            IntwentySignInManager signinmanager,
            IOptions<IntwentySettings> settings)
        {
            UserManager = userManager;
            SignInManager = signinmanager;
            Settings = settings.Value;
        }

        public bool HasAnyMFA { get; set; }
        public bool HasSmsMFA { get; set; }
        public bool HasEmailMFA { get; set; }
        public bool HasFido2MFA { get; set; }
        public bool HasTotpMFA { get; set; }




        public async Task<IActionResult> OnGet()
        {
            var user = await UserManager.GetUserAsync(User);
            var status = await UserManager.GetTwoFactorStatus(user);

            HasSmsMFA = status.HasSmsMFA;
            HasEmailMFA = status.HasEmailMFA;
            HasFido2MFA = status.HasFido2MFA;
            HasTotpMFA = status.HasTotpMFA;
            HasAnyMFA = status.HasAnyMFA;
            

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await UserManager.GetUserAsync(User);
            await UserManager.SetTwoFactorEnabledAsync(user, false);
            return Page();
        }
    }
}