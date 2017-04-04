using Common;
using Model;
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
        public static FriendsReplyViewModel ConvertToFriendsReplyViewModel(UserDetail user, FriendsApply apply)
        {
            if (user == null || apply == null) { return null; }
            ReplyStatus status= ReplyStatusHelper.ConvertToReplyStatus(apply.Result);
            bool HasReadResult = false;
            if (apply.HasReadResult == "已读") {  HasReadResult = true; }

            FriendsReplyViewModel model = new FriendsReplyViewModel()
            {
                AppyId = apply.FriendsApplyId,
                HasReadResult = HasReadResult,
                ReplyUserAvatar = user.AvatarPic,
                ReplyStatus = status,
                ReplyTime = apply.ReplyTime,
                ReplyUserId = apply.ReceiverUserId,
                ReplyUserName = user.UserName

            };
            return model;
            
        }


      
    }


}
