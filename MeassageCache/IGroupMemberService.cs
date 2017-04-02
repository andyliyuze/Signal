using System;


namespace MeassageCache
{
    interface IGroupMemberService
    {
        bool GetItemByMemberId(Guid MemberId, Guid GroupId);
    }
}
