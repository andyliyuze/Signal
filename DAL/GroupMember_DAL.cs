using System;
using DAL.Interface;
using Model;
using System.Linq;

namespace DAL
{
   public class GroupMember_DAL :BaseDAL<GroupMember>, IGroupMember_DAL
    {


        public GroupMember GetItemByMemberId(Guid MemberId,Guid GroupId)
        {
            using (ChatContext context = new ChatContext())
            {
                try
                {
                    var model = context.GroupMember.Where(a => a.MemberId == MemberId &&a.GroupId==GroupId).FirstOrDefault();
                    if (model != null) { return model; }
                    return null;
                }
                catch { return null; }
            }
        }
    }
}
