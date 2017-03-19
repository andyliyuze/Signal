using Common;
using System;

namespace Model.ViewModel
{
    public class GroupReplyViewModel
    {
        public GroupReplyViewModel() { }

        public Guid AppyId { get; set; }
        public Guid ReplyGroupId { get; set; }
        public string ReplyGroupName { get; set; }
        public string ReplyGroupAvatar { get; set; }


        public DateTime ReplyTime { get; set; }

        public ReplyStatus ReplyStatus { get; set; }
        public bool HasReadResult { get; set; }
        public static GroupReplyViewModel Create(Group model, ReplyStatus status, string AppyId, bool HasReadResult)
        {

            return new GroupReplyViewModel
            {
                AppyId = Guid.Parse(AppyId),
                ReplyStatus = status,
                ReplyTime = DateTime.Now,
                ReplyGroupAvatar=model.GroupAvatar,
                ReplyGroupId=model.GroupId,
                ReplyGroupName=model.GroupName,
                HasReadResult = HasReadResult
            };
        }

    }

     
}
