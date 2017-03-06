using Model;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interface
{
  public  interface IFriendsApply_DAL :IDAL<FriendsApply>
    {
      bool SendAddFriendsApply(FriendsApply model);
      List<FriendsApplyViewModel> GetFriendsApplyByUId(Guid Id);

      bool UpdateResult(FriendsApply model);
      bool SetReadByIds(List<string> ids);
    }
}
