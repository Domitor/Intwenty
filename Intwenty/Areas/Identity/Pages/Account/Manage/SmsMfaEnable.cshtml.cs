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
using Intwenty.Interface;
using Intwenty.Areas.Identity.Data;

namespace Intwenty.Areas.Identity.Pages.Account.Manage
{
    public class SmsMfaEnableModel : PageModel
    {
        private readonly IntwentyUserManager _userManager;
        private readonly IIntwentySmsService _smsService;


        public SmsMfaEnableModel(IntwentyUserManager usermanager, IIntwentySmsService smsservice)
        {
            _userManager = usermanager;
            _smsService = smsservice;
        }

        public string PhoneNumber { get; set; }


        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Input = new InputModel();
            Input.PhoneNumber = user.PhoneNumber;

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

            //var code = await _userManager.GenerateTwoFactorTokenAsync(user, _userManager.Options.Tokens.ChangePhoneNumberTokenProvider);
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, Input.PhoneNumber);
            await _smsService.SendSmsAsync(Input.PhoneNumber, code);

       

            return RedirectToPage("./SmsMfaVerify", new { PhoneNumber=Input.PhoneNumber });
            
        }

    }
}
