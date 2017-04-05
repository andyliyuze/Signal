using System;


namespace MeassageCache.Interface
{
  public  interface IGroupMemberService
    {
        bool GetItemByMemberId(Guid MemberId, Guid GroupId);
    }
}
