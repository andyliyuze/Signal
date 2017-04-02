using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
   public class FriendsApplyViewModel: FriendsApply
    {
       
       public string ApplyUserName { get; set; }
       public string ApplyUserAvatar { get; set; }
       public bool IsOnline { get; set; }
       public static FriendsApplyViewModel Create(UserDetail applymodel, Guid ApplyId)
      {

          return new FriendsApplyViewModel { FriendsApplyId=ApplyId, ApplyUserAvatar = applymodel.AvatarPic,
                                             ApplyUserId = applymodel.UserDetailId,
                                             ApplyUserName = applymodel.UserName,
                                             ApplyTime = DateTime.Now,
                                             IsOnline = applymodel.IsOnline
          };
      }
        public static FriendsApplyViewModel ConvertToFriendsApplyViewModel(UserDetail user, FriendsApply Apply)
        {
            if (user==null || Apply==null) { return null; }
            FriendsApplyViewModel viewModel = new FriendsApplyViewModel()
            {
                ApplyTime = Apply.ApplyTime,
                ApplyUserAvatar = user.AvatarPic,
                ApplyUserId = user.UserDetailId,
                ApplyUserName = user.UserName,
                FriendsApplyId = Apply.FriendsApplyId,
                HasReadResult = Apply.HasReadResult,
                IsOnline = user.IsOnline,
                ReceiverUserId = Apply.ReceiverUserId,
                ReplyTime = Apply.ReplyTime,
                Result = Apply.Result

            };
            return viewModel;

        }
    }


 
}
