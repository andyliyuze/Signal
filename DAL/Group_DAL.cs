using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interface;
using Model.ViewModel;
using System.Data.Entity;
using System.Linq.Expressions;

namespace DAL
{
    public class Group_DAL : BaseDAL<Group>, IGroup_DAL
    {
        //创建群
        public bool Create(Group model)
        {
            using (var context = new ChatContext())
            {
                //创建群
                context.Group.Add(model);
                //将群主入群
                GroupMember groupMember = new GroupMember { ApproverId = model.OwnerId, GroupId = model.GroupId, Id = Guid.NewGuid(), MemberId = model.OwnerId };
                context.GroupMember.Add(groupMember);

                //这是一个事务操作
                return context.SaveChanges() > 0;


            }
        }


        //搜索群
        public GroupViewModel GetGroupDeatailByGroupName(string Name)
        {
            using (var context = new ChatContext())
            {
                try
                {
                    Expression<Func<Group, UserDetail, GroupViewModel>>
                      expression = (a, b) => new GroupViewModel()
                      {
                          GroupAvatar = a.GroupAvatar,
                          GroupId = a.GroupId,
                          GroupName = a.GroupName,
                          OwnerId = a.OwnerId,
                          OwnerName = b.UserName
                      };
                    var model = context.Group.Where(a => a.GroupName.Contains(Name))
                        .Join(context.UserDetail, a => a.OwnerId, b => b.UserDetailId,
            //(a, b) => new GroupViewModel() { GroupAvatar = a.GroupAvatar, GroupId = a.GroupId, GroupName = a.GroupName, OwnerId = a.OwnerId, OwnerName = b.UserName }
            expression.Compile()
                        ).FirstOrDefault();
                    return model;
                }
                catch(Exception e)
                {

                    throw e;
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
