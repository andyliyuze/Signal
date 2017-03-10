using Model;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
 public   interface IGroup_DAL
    {
      bool  Create(Group model);
        List<Group> GetGroup(string UserId);
        GroupViewModel GetGroupDeatailByGroupName(string Name);
    }
}
