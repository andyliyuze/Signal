using DAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity;
using Model.ViewModel;
using System.Linq.Expressions;
using Common;

namespace DAL
{


    public class JoinGroupApply_DAL : BaseDAL<JoinGroupApply>, IJoinGroupApply_DAL
    {


        public JoinGroupApply GetItemById(Guid Id)
        {
            using (ChatContext context = new ChatContext())
            {
                JoinGroupApply model = context.JoinGroupApply.Where(a => a.Id == Id).FirstOrDefault();

                return model;
            }

        }

        public bool SetReadByIds(List<string> ids)
        {
            using (ChatContext context = new ChatContext())
            {
                foreach (var id in ids)
                {
                    Guid Id = Guid.Parse(id);
                    var item = context.JoinGroupApply.Where(a => a.Id == Id).FirstOrDefault();
                    item.HasReadResult = "已读";
                    context.Entry<JoinGroupApply>(item).State = EntityState.Modified;

                }
                int i = context.SaveChanges();
                return i >= ids.Count;
            }
        }

        public bool UpdateResult(JoinGroupApply apply)
        {
            try
            {
                using (var context = new ChatContext())
                {

                    Func<JoinGroupApply, bool> f = x =>
                     {

                         JoinGroupApply model = context.JoinGroupApply.Where(a => a.Id == x.Id).FirstOrDefault();
                         if (model != null)
                         {
                             model.HasReadResult = x.HasReadResult;
                             model.Result = x.Result;
                             model.ReplyTime = DateTime.Now;
                             context.Set<JoinGroupApply>().Attach(model);
                             context.Entry<JoinGroupApply>(model).State = EntityState.Modified;
                             return context.SaveChanges() > 0;
                         }
                         else
                         {
                             return false;
                         }
                     };

                    return f(apply);
                }
            }
            catch
            {
                return false;
            }
        }

        private bool TryUpdate(JoinGroupApply model, ChatContext context)
        {
            JoinGroupApply apply = context.JoinGroupApply.Where(a => a.Id == model.Id).FirstOrDefault();
            if (apply != null)
            {
                apply.HasReadResult = model.HasReadResult;
                apply.Result = model.Result;
                apply.ReplyTime = DateTime.Now;
                context.Set<JoinGroupApply>().Attach(apply);
                context.Entry<JoinGroupApply>(apply).State = EntityState.Modified;
                return context.SaveChanges() > 0;
            }
            else
            {
                return false;
            }
        }

        //获取所有未审核的群申请加入消息
        public List<GroupApplyViewModel> GetGroupApplyByUId(Guid Id)
        {
            try
            {
                using (var context = new ChatContext())
                {
                    //三表联合查询
                    var GroupApplyList = context.JoinGroupApply.Where(a => a.Result== "待审").Join(context.Group, a => a.GroupId, b => b.GroupId, (a, b) => new { applyModel = a, GroupModel = b })
                            .Join(context.UserDetail, b => b.applyModel.ApplyUserId, c => c.UserDetailId, (b, c) => new GroupApplyViewModel()
                            {
                                ApplyTime = b.applyModel.ApplyTime,
                                ApplyUserAvatar = c.AvatarPic,
                                ApplyUserId = c.UserDetailId,
                                ApplyUserName = c.UserName,
                                GroupName = b.GroupModel.GroupName,
                                IsOnline = c.IsOnline,
                                GroupId = b.GroupModel.GroupId,
                                GroupApplyId = b.applyModel.Id,
                                OwnerId = b.GroupModel.OwnerId
                            }).Where(a => a.OwnerId == Id).ToList();
                    return GroupApplyList;
                }
            }
            catch { return new List<GroupApplyViewModel>(); }

        }

        public List<GroupReplyViewModel> GetGroupReplyByUId(Guid Id)
        {
            try
            {
                using (ChatContext context = new ChatContext())
                {
                    Expression<Func<JoinGroupApply, Group, GroupReplyViewModel>>
                        expression = (a, b) => new GroupReplyViewModel()
                        {
                            ReplyTime = a.ReplyTime,
                            AppyId = a.Id,
                            ReplyGroupAvatar = b.GroupAvatar,
                            ReplyGroupName = b.GroupName,
                            ReplyGroupId = b.GroupId,
                            ReplyStatus = ReplyStatusHelper.ConvertToReplyStatus(a.Result),
                            HasReadResult=false
                        };



                    var list = context.JoinGroupApply.Where(a => a.ApplyUserId == Id && a.HasReadResult == "未读")
                        .Join(context.Group, a => a.GroupId, b => b.GroupId, expression.Compile()
                       ).ToList();
                    return list;

                }

            }
            catch (Exception e){
                throw e;
                //return new List<GroupReplyViewModel>();
            }
        }


     
    }
}
