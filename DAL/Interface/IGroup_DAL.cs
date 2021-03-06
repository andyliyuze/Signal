﻿using Model;
using Model.ViewModel;
using System;
using System.Collections.Generic;

namespace DAL.Interface
{
    public   interface IGroup_DAL
    {
        bool  Create(Group model);
      
        GroupViewModel GetGroupDeatailByGroupName(string Name);
        Group GetItemByGroupId(Guid Id);
        List<Group> GetMyGroups(Guid UserId);
    }
}
