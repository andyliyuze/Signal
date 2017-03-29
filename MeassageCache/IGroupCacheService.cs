﻿

using Model;
using Model.ViewModel;
using System;
using System.Collections.Generic;

namespace MeassageCache
{
    interface IGroupCacheService
    {
        bool Create(Group model);
        List<Group> GetGroup(Guid UserId);
        GroupViewModel GetGroupDeatailByGroupName(string Name);
        Group GetItemByGroupId(Guid Id);
        List<Group> GetMyGroups(Guid UserId);
    }
}
