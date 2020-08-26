using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Intwenty.Data.Identity;
using Intwenty.Model;
using Microsoft.Extensions.Options;

namespace IntwentyDemo.Areas.Identity.Pages.Account.Manage
{
    public partial class LanguageModel : PageModel
    {
        private readonly UserManager<IntwentyUser> _userManager;
        private readonly SignInManager<IntwentyUser> _signInManager;
        private readonly IntwentySettings _settings;

        public LanguageModel(
            UserManager<IntwentyUser> userManager,
            SignInManager<IntwentyUser> signInManager,
            IOptions<IntwentySettings> settings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _settings = settings.Value;
        }

        public string Language { get; set; }


        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Language")]
            public string NewLanguage{ get; set; }
        }

        private async Task LoadAsync(IntwentyUser user)
        {
            var currentuser = await _userManager.GetUserAsync(User);

            Input = new InputModel
            {
                NewLanguage = currentuser.Language,
            };

            Language = Input.NewLanguage;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeLanguageAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (Input.NewLanguage != user.Language && !string.IsNullOrEmpty(Input.NewLanguage))
            {
                user.Language = Input.NewLanguage;
                await _userManager.UpdateAsync(user);

                StatusMessage = "Language settings changed.";
                return RedirectToPage();
            }

            StatusMessage = "Language settings unchanged.";
            return RedirectToPage();
        }

      
    }
}
