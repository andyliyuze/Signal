using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interface;
using Model.ViewModel;
using System.Data.Entity;
using Common;
using System.Linq.Expressions;

namespace DAL
{
    public class FriendsApply_DAL : IFriendsApply_DAL
    {

        public bool SendAddFriendsApply(FriendsApply model)
        {
            try
            {
                using (var context = new ChatContext())
                {
                    model.ApplyTime = DateTime.Now;
                    model.ReplyTime = DateTime.Now;


                    context.FriendsApply.Add(model);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception e)
            {

                throw e;
            }






        }






        private BaseDAL<FriendsApply> basedal = new BaseDAL<FriendsApply>();

        public bool Add(FriendsApply model)
        {
            return basedal.Add(model);
        }

        public bool Delete(FriendsApply model)
        {
            return basedal.Delete(model);
        }

        public bool Update(FriendsApply model)
        {

            return basedal.Update(model);
        }


        private bool TryUpdate(FriendsApply model, ChatContext context)
        {


            FriendsApply apply = context.FriendsApply.Where(a => a.FriendsApplyId == model.FriendsApplyId && a.ReceiverUserId == model.ReceiverUserId).FirstOrDefault();
            if (apply != null)
            {
                apply.HasReadResult = model.HasReadResult;
                apply.Result = model.Result;
                apply.ReplyTime = DateTime.Now;
                context.Set<FriendsApply>().Attach(apply);
                context.Entry<FriendsApply>(apply).State = EntityState.Modified;
                return context.SaveChanges() > 0;
            }
            else
            {
                return false;
            }
        }


        public bool UpdateResult(FriendsApply model)
        {
            try
            {
                using (var context = new ChatContext())
                {
                    return TryUpdate(model, context);
                }
            }
            catch
            {
                return false;
            }

        }


        public bool SetReadByIds(List<string> ids)
        {

            using (ChatContext context = new ChatContext())
            {
                foreach (var id in ids)
                {
                    Guid Id = Guid.Parse(id);
                    var item = context.FriendsApply.Where(a => a.FriendsApplyId == Id).FirstOrDefault();
                    item.HasReadResult = "已读";
                    context.Entry<FriendsApply>(item).State = EntityState.Modified;

                }
                int i = context.SaveChanges();
                return i >= ids.Count;
            }


        }


        //获取所有未审核的申请消息
        public List<FriendsApplyViewModel> GetFriendsApplyByUId(Guid Id)
        {
            try
            {
                using (var context = new ChatContext())
                {
                    List<FriendsApplyViewModel> list =
                        context.FriendsApply.Where(a => a.Result == "待审" && a.ReceiverUserId == Id).Join(context.UserDetail, a => a.ApplyUserId, b => b.UserDetailId, (a, b) => new FriendsApplyViewModel()
                        {
                            ApplyUserId = a.ApplyUserId,
                            ApplyUserAvatar = b.AvatarPic,
                            ApplyUserName = b.UserName,
                            ReceiverUserId = a.ReceiverUserId,
                            FriendsApplyId = a.FriendsApplyId,
                            ApplyTime = a.ApplyTime,
                            IsOnline = b.IsOnline
                        }).OrderByDescending(a => a.ApplyTime).ToList<FriendsApplyViewModel>();
                    return list;

                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        //获取所有未读的回复消息,并且筛走被忽略的记录
        public List<FriendsReplyViewModel> GetFriendsReplyByUId(Guid Id)
        {
            try
            {
                using (ChatContext context = new ChatContext())
                {
                    Expression<Func<FriendsApply, UserDetail, FriendsReplyViewModel>>
                        expression = (a, b) => new FriendsReplyViewModel()
                        {
                            ReplyTime = a.ReplyTime,
                            AppyId = a.FriendsApplyId,
                            ReplyUserAvatar = b.AvatarPic,
                            ReplyUserId = b.UserDetailId,
                            ReplyUserName = b.UserName,
                            ReplyStatus = ReplyStatusHelper.ConvertToReplyStatus(a.Result),
                            HasReadResult = false
                        };
                    var list = context.FriendsApply.Where(a => a.ApplyUserId == Id && a.HasReadResult == "未读" && a.Result != "忽略")
                        .Join(context.UserDetail, a => a.ReceiverUserId, b => b.UserDetailId, expression.Compile()
                    ).ToList();
                    return list;
                }
            }
            catch (Exception e)
            {
                throw e;
                //return new List<GroupReplyViewModel>();
            }
        }
    }
}
