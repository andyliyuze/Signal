using System;


namespace MeassageCache.Interface
{
    interface IGroupMemberService
    {
        bool GetItemByMemberId(Guid MemberId, Guid GroupId);
    }
}
