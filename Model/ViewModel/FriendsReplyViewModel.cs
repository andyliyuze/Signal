using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel 
{
  public  class FriendsReplyViewModel
    {
        private FriendsReplyViewModel() { }

        public Guid AppyId { get; set; }
        public Guid ReplyUserId { get; set; }
        public string ReplyUserName { get; set; }
        public string ReplyUserAvatar { get; set; }

   
        public DateTime ReplyTime { get; set; }

        public ReplyStatus ReplyStatus { get; set; }
        public bool HasReadResult { get; set; }
        public static FriendsReplyViewModel Create(UserDetail model, ReplyStatus status, string AppyId, bool HasReadResult)
        {
            
            return new FriendsReplyViewModel {
                AppyId=Guid.Parse(AppyId), ReplyStatus = status, 
                ReplyTime = DateTime.Now, ReplyUserAvatar = model.AvatarPic,
                ReplyUserId = model.UserDetailId, ReplyUserName = model.UserName,
                HasReadResult=HasReadResult
            };
        }
   
    }

  public enum ReplyStatus
  {


      Pass = 0,
      Decline = 1,
      Ignore = 2,

  }
}
