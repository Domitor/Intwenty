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
using Intwenty.Areas.Identity.Models;
using Intwenty.Services;
using Intwenty.Model.FrejaEId;
using Microsoft.AspNetCore.Authorization;

namespace Intwenty.Areas.Identity.Pages.Account
{

    [AllowAnonymous]
    public class FrejaLoginModel : PageModel
    {
        private readonly IntwentyUserManager _userManager;
        private readonly IIntwentyEventService _eventService;
        private readonly IFrejaClient _frejaClient;


        public string QRCodeUrl {get; set;}

        public FrejaLoginModel(IntwentyUserManager usermanager, IIntwentyEventService eventservice, IFrejaClient frejaclient)
        {
            _userManager = usermanager;
            _eventService = eventservice;
            _frejaClient = frejaclient;
        }

       

        public async Task<IActionResult> OnGet()
        {
            try
            {
                var startref = await _frejaClient.InitQRAuthentication();
                var t = _frejaClient.GetQRCode(startref.authRef);
                this.QRCodeUrl = t.OriginalString;
            }
            catch
            {
            }

            return Page();
        }

        public IActionResult OnGetLoad()
        {

            try
            {
              

            }
            catch
            {
            }

            return  new JsonResult("{}") { StatusCode = 501 };
        }


        public async Task<IActionResult> OnPostInitiateAuth([FromBody] IntwentyMfaModel model)
        {
            try
            {

                //{"userInfoType":"EMAIL","userInfo":"joe.black@verisec.com","minRegistrationLevel":"EXTENDED","attributesToReturn":[{"attribute":"BASIC_USER_INFO"},{"attribute":"SSN"}]}
                //var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"ssn\":" + Convert.ToInt64(model.Email) + ", \"country\":\"" + "SE" + "\"}"));
                //var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"ssn\":" + Convert.ToInt64(model.Email) + ", \"country\":\"" + "SE" + "\"}"));

                // Authenticate with freja eID v1.0
                bool success = await _frejaClient.Authenticate("EMAIL", model.Email);
                if (success == false)
                {
                    return new JsonResult(new ResponseData(false, "", "Was not able to authenticate you with Freja eID. If you have a Freja eID app with a valid certificate, try again."));
                }

                /*
                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.ChangePhoneNumberTokenProvider, model.Code);
                if (result)
                {
                    model.ResultCode = string.Empty;
                    await _userManager.SetTwoFactorEnabledAsync(user, true);
                    await _userManager.AddUpdateUserSetting(user, "EMAILMFA", "TRUE");
                    return new JsonResult(model);
                }
                else
                {
                    model.ResultCode = "ERROR_VERIFY_TOKEN";
                    await _userManager.SetTwoFactorEnabledAsync(user, false);
                    await _userManager.AddUpdateUserSetting(user, "EMAILMFA", "FALSE");
                    return new JsonResult(model) { StatusCode = 501 };
                }
                */
            }
            catch(Exception ex)
            {
                var s = "";
                model.ResultCode = "ERROR_AUTHENTICATING";
            }

            return new JsonResult(model) { StatusCode = 501 };
        }

    }
}
