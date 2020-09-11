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
using Intwenty.Data.Dto;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Intwenty.Areas.Identity.Pages.Account.Manage
{
    public partial class GroupsModel : PageModel
    {
        private readonly IntwentyUserManager _userManager;
        private readonly IntwentySettings _settings;

        public GroupsModel(
            IntwentyUserManager userManager,
            SignInManager<IntwentyUser> signInManager,
            IOptions<IntwentySettings> settings)
        {
            _userManager = userManager;
            _settings = settings.Value;
        }

        public class SetMembershipModel
        {
            public string UserId { get; set; }
            public string Status { get; set; }
            public string GroupId { get; set; }
        }

        public class RenameGroupModel
        {
            public string GroupName { get; set; }
            public string NewName { get; set; }
            public string GroupId { get; set; }
        }

        public class InviteToGroupModel
        {
            public string GroupName { get; set; }
            public string Email { get; set; }
            public string GroupId { get; set; }
        }


        public class CreateOrJoinGroupModel
        {
            public string GroupName { get; set; }
        }

        public class MyGroupConnections
        {
            public List<IntwentyUserGroup> MyGroups  { get; set; }

            public List<GroupMemberShip> MyGroupsMembers { get; set; }

            public MyGroupConnections()
            {
                MyGroups = new List<IntwentyUserGroup>();
                MyGroupsMembers = new List<GroupMemberShip>();
            }
        }

        public class GroupMemberShip
        {
            public string GroupId { get; set; }

            public string GroupName { get; set; }

            public List<IntwentyUserGroup> Members { get; set; }

            public GroupMemberShip()
            {
                Members = new List<IntwentyUserGroup>();
            }
        }

        public IActionResult OnGetAsync()
        {
            return Page();
        }

        public IActionResult OnGetLoadGroups()
        {
            var result = new MyGroupConnections();

            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                if (user == null)
                {
                    throw new InvalidOperationException("Could not find the user when listing groups");
                }

                var usergroups = _userManager.GetUserGroups(user).Result;
                foreach (var ug in usergroups)
                {
                    if (ug.MembershipType == "GROUPADMIN")
                    {
                        result.MyGroups.Add(ug);
                        result.MyGroupsMembers.Add(new GroupMemberShip() { GroupId = ug.GroupId, GroupName = ug.GroupName }); 

                    }
                }
                foreach (var group in result.MyGroupsMembers) 
                {
                    var groupmembers = _userManager.GetGroupMembers(new IntwentyGroup() { Id = group.GroupId, Name=group.GroupName }).Result;
                    foreach (var gm in groupmembers)
                    {
                        group.Members.Add(gm);
                        
                    }
                }

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError(ex.Message, "There was an error when asdding a group.");
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return new JsonResult(result);
        }

       
        public IActionResult OnPostMembershipChange([FromBody] SetMembershipModel model)
        {

            var user =  _userManager.FindByIdAsync(model.UserId).Result;
            if (user != null)
            {
                if (model.Status == "REMOVE")
                {
                    _userManager.RemoveFromGroupAsync(user.Id, model.GroupId);
                }
                else
                {
                    var group = _userManager.GetGroupByIdAsync(model.GroupId).Result;
                    _userManager.UpdateGroupMembershipAsync(user, group, model.Status);

                }
            }

            return new JsonResult("{}");
      
        }

        
       public IActionResult OnPostInviteToGroup([FromBody] InviteToGroupModel model)
       {

           var x = ""!;

           return new JsonResult("{}");

       }

        public IActionResult OnPostRenameGroup([FromBody] RenameGroupModel model)
        {

            var x = ""!;

            return new JsonResult("{}");

        }


        public IActionResult OnPostCreateGroup([FromBody] CreateOrJoinGroupModel model)
        {

            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                if (user == null)
                {
                    throw new InvalidOperationException("Could not find the user when adding a group");
                }

                if (_userManager.GroupExists(model.GroupName).Result)
                    throw new InvalidOperationException("The group already exists, type another name.");

                var group = _userManager.AddGroupAsync(model.GroupName).Result;
                _userManager.AddGroupMembershipAsync(user, group, "GROUPADMIN", "ACCEPTED");

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError("There was an error when asdding a group.", ex.Message);
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return new JsonResult("{}");

        }

        public IActionResult OnPostJoinGroup([FromBody] CreateOrJoinGroupModel model)
        {

            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                if (user == null)
                {
                    throw new InvalidOperationException("Could not find the user when adding a group");
                }

                if (!_userManager.GroupExists(model.GroupName).Result)
                    throw new InvalidOperationException("The group does not exists, type another groupname.");

                var usergroups = _userManager.GetUserGroups(user).Result;
                if (usergroups.Exists(p=> p.GroupName== model.GroupName))
                    throw new InvalidOperationException("You are already a member or admin of this group.");

                var group = _userManager.GetGroupByNameAsync(model.GroupName).Result;
                _userManager.AddGroupMembershipAsync(user, group, "GROUPMEMBER", "WAITING");

            }
            catch (Exception ex)
            {
                var r = new OperationResult();
                r.SetError("There was an error when asdding a group.", ex.Message);
                var jres = new JsonResult(r);
                jres.StatusCode = 500;
                return jres;
            }

            return new JsonResult("{}");

        }


    }
}
