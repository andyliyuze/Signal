using Model;
using Model.ViewModel;
using System;
using System.Collections.Generic;

namespace MeassageCache.Interface
{
  public  interface IFriendsApplyService
    {
        bool SendAddFriendsApply(FriendsApply model);
        List<FriendsApplyViewModel> GetFriendsApplyByUId(Guid Id);
        List<FriendsReplyViewModel> GetFriendsReplyByUId(Guid Id);
        bool UpdateResult(FriendsApply model);
        bool SetReadByIds(List<string> ids);
    }
}
