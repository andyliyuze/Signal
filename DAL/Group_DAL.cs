using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interface;
using Model.ViewModel;
using System.Data.Entity;

namespace DAL
{
    public class Group_DAL : BaseDAL<Group>, IGroup_DAL
    {
        //创建群
        public bool Create(Group model)
        {
            return Add(model);
        }


        //搜索群
        public GroupViewModel GetGroupDeatailByGroupName(string Name)
        {
            using (var context = new ChatContext())
            {
                try
                {
                    var model = context.Group.Where(a => a.GroupName.Contains(Name))
                        .Join(context.UserDetail, a => a.OwnerId, b => b.UserDetailId,
                        (a, b) => new GroupViewModel() { GroupAvatar = a.GroupAvatar,GroupId=a.GroupId,GroupName=a.GroupName,OwnerId=a.OwnerId, OwnerName = b.UserName }).FirstOrDefault();
                    return model;
                }
                catch
                {

                    return null;
                }
            }
        }


        //发送群消息，同意入群，通知群主入群申请，拒绝入群
        public Group GetItemByGroupId(Guid Id)
        {
            using (ChatContext context = new ChatContext())
            {
                var model=    context.Group.Where(a => a.GroupId == Id).FirstOrDefault();
                return model;
            }
        }
        //查找个人的所有群
        public List<Group> GetMyGroups(Guid UserId)
        {
            try
            {
                using (var context = new ChatContext())
                {
                    var list = context.GroupMember.Where(a => a.MemberId == UserId).Join(context.Group, a => a.GroupId, b => b.GroupId, (a, b) =>b
                   ).ToList();
                    return list;
                }
            }
            catch { return new List<Group>(); }

        }

    }
}
