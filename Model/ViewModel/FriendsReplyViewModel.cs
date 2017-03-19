using Common;
using System;

namespace Model.ViewModel
{
    public  class FriendsReplyViewModel
    {
        public FriendsReplyViewModel() { }

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


}
