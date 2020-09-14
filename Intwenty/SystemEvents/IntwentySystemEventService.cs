using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.SystemEvents
{
    public interface IIntwentySystemEventService
    {
        void UserInvitedToGroup(UserInvitedData data);
        void UserRemovedFromGroup(UserRemovedFromGroupData data);
        void UserRequestedToJoinGroup(UserRequestedToJoinGroupData data);
    }

    public class IntwentySystemEventService : IIntwentySystemEventService
    {
        public virtual void UserInvitedToGroup(UserInvitedData data){}
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


}
