

using Model;
using Model.ViewModel;
using System;
using System.Collections.Generic;

namespace MeassageCache.Interface
{
    interface IGroupCacheService
    {
        bool Create(Group model);
        GroupViewModel GetGroupDeatailByGroupName(string Name);
        Group GetItemByGroupId(Guid Id);
        List<Group> GetMyGroups(Guid UserId);
    }
}
