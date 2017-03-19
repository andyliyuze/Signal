using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
  public  class GroupApplyViewModel
    {

        public Guid GroupApplyId
        {
            get;
            set;
        }
        public Guid ApplyUserId { get; set; }
        public string ApplyUserName { get; set; }
        public string ApplyUserAvatar { get; set; }

        public bool IsOnline { get; set; }
        public Guid OwnerId { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public DateTime ApplyTime { get; set; }


        public static GroupApplyViewModel Create(UserDetail applymodel, Guid ApplyId,Group groupmodel)
        {

            return new GroupApplyViewModel
            {
                GroupApplyId = ApplyId,
                GroupId = groupmodel.GroupId,
                GroupName = groupmodel.GroupName,
                ApplyUserAvatar = applymodel.AvatarPic,
                ApplyUserId = applymodel.UserDetailId,
                ApplyUserName = applymodel.UserName,
                ApplyTime = DateTime.Now,
                IsOnline = applymodel.IsOnline
            };
        }
    }
}
