using Model;
using System;

namespace DAL.Interface
{
    public  interface IGroupMember_DAL: IDAL<GroupMember>
    {
        GroupMember GetItemByMemberId(Guid MemberId, Guid GroupId);
        
    }
}
