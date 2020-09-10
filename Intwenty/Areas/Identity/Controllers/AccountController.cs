using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Intwenty.Data.Dto;
using Intwenty.Model;
using Intwenty.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Intwenty.Areas.Identity.Data;

namespace Intwenty.Areas.Identity.Controllers
{

    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly SignInManager<IntwentyUser> _signInManager;
        private readonly IntwentyUserManager _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IntwentySettings _settings;

        public AccountController(IntwentyUserManager userManager,
            SignInManager<IntwentyUser> signInManager,
            IEmailSender emailSender,
            IOptions<IntwentySettings> settings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _settings = settings.Value;
        }

        [HttpGet("Identity/Account/Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("/Identity/Account/API/Register")]
        public IActionResult RegisterUserAsync([FromBody] RegisterVm model)
        {
            try
            {


                model.Message = "";
                model.ReturnUrl = Url.Content("~/");

                var user = new IntwentyUser { UserName = model.Email, Email = model.Email, Culture = model.Language };
                if (string.IsNullOrEmpty(user.Culture))
                    user.Culture = _settings.DefaultCulture;

                var is_waiting_for_group_adm = false;
                IntwentyGroup group=null;

                if (_settings.EnableUserGroups)
                {
                    if (model.AccountType == "GROUPADMIN")
                    {
                        if (_userManager.GroupExists(model.GroupName).Result)
                        {
                            throw new InvalidOperationException(string.Format("The group '{0}' already exists, please use another groupname", model.GroupName));
                        }

                        group = _userManager.AddGroupAsync(model.GroupName).Result;
                        _userManager.AddGroupMembershipAsync(user, group, model.AccountType, "ACCEPTED");
                    }

                    if (model.AccountType == "GROUPMEMBER")
                    {
                        if (!_userManager.GroupExists(model.GroupName).Result)
                        {
                            throw new InvalidOperationException(string.Format("The group '{0}' does not exists, please type an existing group", model.GroupName));
                        }
                        group = _userManager.GetGroupByNameAsync(model.GroupName).Result;
                        _userManager.AddGroupMembershipAsync(user, group, model.AccountType, "WAITING");
                        is_waiting_for_group_adm = true;
                    }
                }

                var result = _userManager.CreateAsync(user, model.Password).Result;
                if (result.Succeeded)
                {

                    var code = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    _emailSender.SendEmailAsync(model.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        var confurl = Url.Page(
                        "/Account/RegisterConfirmation",
                        pageHandler: null,
                        values: new { area = "Identity", email = model.Email },
                        protocol: Request.Scheme);
                        model.ReturnUrl = HtmlEncoder.Default.Encode(confurl);
                        return new JsonResult(model);
                    }
                    else
                    {
                        if (!is_waiting_for_group_adm)
                            _signInManager.SignInAsync(user, isPersistent: false);
                        else
                        {
                            if (group != null)
                                 model.Message = String.Format("Your account was created but must be verified by the {0} group administrator before you can log in", group.Name);
                        }
                        return new JsonResult(model);
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        throw new InvalidOperationException(error.Description);
                    }
                }

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError("", ex.Message);
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return new JsonResult(model);

        }

    }
}
