using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Text;


namespace Intwenty.SystemEvents
{
    public interface IIntwentySystemEventService
    {
        void NewUserCreated(NewUserCreatedData data);
        void UserInvitedToGroup(UserInvitedData data);
        void UserRemovedFromGroup(UserRemovedFromGroupData data);
        void UserRequestedToJoinGroup(UserRequestedToJoinGroupData data);
    }

    public class IntwentySystemEventService : IIntwentySystemEventService
    {
        protected readonly IEmailSender EmailService;
        protected readonly IIntwentyDataService DataService;

        public IntwentySystemEventService(IEmailSender emailsender, IIntwentyDataService dataservice)
        {
            EmailService = emailsender;
            DataService = dataservice;
        }

        public virtual void NewUserCreated(NewUserCreatedData data)  { }
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
