using Intwenty.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.SystemEvents
{
   
    public class IntwentyEventService : IIntwentyEventService
    {
        protected readonly IIntwentyEmailService EmailService;
        protected readonly IIntwentyDataService DataService;

        public IntwentyEventService(IIntwentyEmailService emailsender, IIntwentyDataService dataservice)
        {
            EmailService = emailsender;
            DataService = dataservice;
        }

        public virtual async Task NewUserCreated(NewUserCreatedData data) 
        {
            await DataService.LogIdentityActivity("INFO", string.Format("A new user {0} created an account", data.UserName), data.UserName);
        }
        public virtual async Task EmailChanged(EmailChangedData data) 
        {
            await DataService.LogIdentityActivity("INFO", string.Format("A user {0} changed registered email to {1}", data.UserName, data.Email), data.UserName);
        }
        public virtual Task UserInvitedToGroup(UserInvitedData data) { return Task.FromResult(0);  }
        public virtual Task UserRemovedFromGroup(UserRemovedFromGroupData data) { return Task.FromResult(0);  }
        public virtual Task UserRequestedToJoinGroup(UserRequestedToJoinGroupData data) { return Task.FromResult(0); }


    }

   

    public class SenderReceiverUserData
    {
        public string SenderUserName { get; set; }
        public string ReceiverUserName { get; set; }
    }

    public class UserInvitedData : SenderReceiverUserData
    {
        public string GroupName { get; set; }
    }

    public class UserRemovedFromGroupData : SenderReceiverUserData
    {
        public string GroupName { get; set; }
    }

    public class UserRequestedToJoinGroupData : SenderReceiverUserData
    {
        public string GroupName { get; set; }
    }

    public class NewUserCreatedData
    {
       public string UserName { get; set; }

        public string Email { get; set; }

        public string ConfirmCallbackUrl { get; set; }
    }

    public class EmailChangedData
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string ConfirmCallbackUrl { get; set; }
    }

}
