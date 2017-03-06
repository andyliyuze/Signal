using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
   public class FriendsApplyViewModel
    {
       public Guid FriendsApplyId
       {
           get;
           set;
       }
       public Guid ApplyUserId { get; set; }
       public string ApplyUserName { get; set; }
       public string ApplyUserAvatar { get; set; }

       public bool IsOnline { get; set; }
       public Guid ReceiverUserId { get; set; }
       public DateTime ApplyTime { get; set; }


       public static FriendsApplyViewModel Create(UserDetail applymodel, Guid ApplyId)
      {

          return new FriendsApplyViewModel { FriendsApplyId=ApplyId, ApplyUserAvatar = applymodel.AvatarPic,
                                             ApplyUserId = applymodel.UserDetailId,
                                             ApplyUserName = applymodel.UserName,
                                             ApplyTime = DateTime.Now,
                                             IsOnline = applymodel.IsOnline
          };
      }
       
    }


 
}
