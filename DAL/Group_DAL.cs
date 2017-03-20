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
        public bool Create(Group model)
        {
            return Add(model);
        }

        public List<Group> GetGroup(Guid UserId)
        {
            using (var context = new ChatContext())
            {
                List<Group> list =
                    context.GroupMember.Where(a => a.MemberId ==  UserId).Join(context.Group, a => a.GroupId, b => b.GroupId, (a, b) =>
                   new Group()
                   {
                       GroupAvatar = b.GroupAvatar,
                       GroupId = b.GroupId,
                       GroupName = b.GroupName,
                       OwnerId = b.OwnerId
                   }).ToList();
                return list;

            }

        }

        public GroupViewModel GetGroupDeatailByGroupName(string Name)
        {
            using (var context = new ChatContext())
            {
                try
                {
                    var model = context.Group.Where(a => a.GroupName.Contains(Name))
                        .Join(context.UserDetail, a => a.OwnerId, b => b.UserDetailId,
                        (a, b) => new GroupViewModel() { Group = a, OwnerName = b.UserName }).FirstOrDefault();
                    return model;
                }
                catch
                {

                    return null;
                }
            }
        }

        public Group GetItemByGroupId(Guid Id)
        {
            using (ChatContext context = new ChatContext())
            {
                var model=    context.Group.Where(a => a.GroupId == Id).FirstOrDefault();
                return model;
            }
        }

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
