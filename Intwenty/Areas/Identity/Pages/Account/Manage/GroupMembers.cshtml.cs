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
using Intwenty.Model;
using Microsoft.Extensions.Options;
using Intwenty.Areas.Identity.Models;
using Intwenty.Areas.Identity.Data;

namespace Intwenty.Areas.Identity.Pages.Account.Manage
{
    public partial class GroupMembersModel : PageModel
    {
        private readonly IntwentyUserManager _userManager;
        private readonly IntwentySettings _settings;

        public GroupMembersModel(
            IntwentyUserManager userManager,
            SignInManager<IntwentyUser> signInManager,
            IOptions<IntwentySettings> settings)
        {
            _userManager = userManager;
            _settings = settings.Value;
            GroupMembers = new List<IntwentyUserGroup>();
        }

        public string Group { get; set; }

        public List<IntwentyUserGroup> GroupMembers { get; set; }

        public class StatusModel
        {
            public string UserId { get; set; }
            public string Status { get; set; }
            public string GroupId { get; set; }
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!_settings.EnableUserGroups)
                return Page();

            var usergroup = _userManager.GetUserGroup(user);
            if (usergroup == null)
                return Page();
            if (usergroup.Result == null)
                return Page();
            if (usergroup.Result.MembershipType != "GROUPADMIN")
                return Page();

            var group = _userManager.GetGroupByIdAsync(usergroup.Result.GroupId).Result;

            Group = group.Name;
            GroupMembers = _userManager.GetGroupMembers(group).Result.Where(p=> p.UserId != user.Id).ToList();

            return Page();
        }

        public IActionResult OnPostMembershipChange([FromBody] StatusModel model)
        {

            var user =  _userManager.FindByIdAsync(model.UserId).Result;
            if (user != null)
            {
                if (model.Status == "REMOVE")
                {
                    _userManager.DeleteAsync(user);
                }
                else
                {
                    var group = _userManager.GetGroupByIdAsync(model.GroupId).Result;
                    _userManager.UpdateGroupMembershipAsync(user, group, model.Status);

                }
            }

            return new JsonResult("{}");
      
        }

        public IActionResult OnPostSendInvitation([FromBody] StatusModel model)
        {

            var x = ""!;

            return new JsonResult("{}");

        }


    }
}
