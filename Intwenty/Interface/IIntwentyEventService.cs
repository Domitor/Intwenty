using Intwenty.SystemEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Interface
{
    public interface IIntwentyEventService
    {
        void NewUserCreated(NewUserCreatedData data);
        void UserInvitedToGroup(UserInvitedData data);
        void UserRemovedFromGroup(UserRemovedFromGroupData data);
        void UserRequestedToJoinGroup(UserRequestedToJoinGroupData data);
    }

}
