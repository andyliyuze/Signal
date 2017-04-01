using Model;
using System;
 

namespace MeassageCache
{
    interface IGroupMemberService
    {
        GroupMember GetItemByMemberId(Guid MemberId);
    }
}
