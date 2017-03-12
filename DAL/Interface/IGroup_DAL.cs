using Model;
using Model.ViewModel;
using System;
using System.Collections.Generic;

namespace DAL.Interface
{
    public   interface IGroup_DAL
    {
        bool  Create(Group model);
        List<Group> GetGroup(Guid UserId);
        GroupViewModel GetGroupDeatailByGroupName(string Name);
        Group GetItemByGroupId(Guid Id); 
    }
}
