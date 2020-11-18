using Intwenty.Interface;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Text;


namespace Intwenty.SystemEvents
{
   
    public class IntwentyEventService : IIntwentyEventService
    {
        protected readonly IEmailSender EmailService;
        protected readonly IIntwentyDataService DataService;

        public IntwentyEventService(IEmailSender emailsender, IIntwentyDataService dataservice)
        {
            EmailService = emailsender;
            DataService = dataservice;
        }

        public virtual void NewUserCreated(NewUserCreatedData data) 
        {
            EmailService.SendEmailAsync("test", "New user created", "A new Intwenty user was created.");
        }
        public virtual void UserInvitedToGroup(UserInvitedData data) { }
        public virtual void UserRemovedFromGroup(UserRemovedFromGroupData data) { }
        public virtual void UserRequestedToJoinGroup(UserRequestedToJoinGroupData data) { }


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

       public string ConfirmCallbackUrl { get; set; }
    }


}
