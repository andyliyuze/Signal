using MeassageCache.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.ViewModel;
using ServiceStack.Redis;
using MeassageCache.Common;

namespace MeassageCache
{
    public class FriendsApplyService : IFriendsApplyService
    {
        public List<FriendsApplyViewModel> GetFriendsApplyByUId(Guid Id)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {
                    List<FriendsApplyViewModel> ApplyIdList = new List<FriendsApplyViewModel>();
                   //获取好友申请的Id集合
                    string key = "FriendsApplySet:" + Id.ToString() + "";
                    List<string> FriendsApplyList = redisClient.GetAllItemsFromSet(key).ToList();
                    foreach (var id in FriendsApplyList)
                    {
                        FriendsApply  item = redisClient.GetEntity<FriendsApply>(id);
                        UserDetail user = redisClient.GetEntity<UserDetail>(item.ApplyUserId.ToString());
                        FriendsApplyViewModel ViewModel = FriendsApplyViewModel.ConvertToFriendsApplyViewModel(user, item);
                       if (ViewModel != null) { ApplyIdList.Add(ViewModel); }
                    }
                    return ApplyIdList;
                     
                }
            }
            catch { return new List<FriendsApplyViewModel>(); }
        }

        public List<FriendsReplyViewModel> GetFriendsReplyByUId(Guid Id)
        {
            throw new NotImplementedException();
        }

        public bool SendAddFriendsApply(FriendsApply model)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {
                    //添加消息实体到hash类型
                    string key = "FriendsApply:" + model.FriendsApplyId.ToString() + "";
                    var kvp = RedisClient.ConvertToHashFn(model);
                    redisClient.SetRangeInHash(key, kvp);
                    return true;
                }
            }
            catch { return false; }
        }

        public bool SetReadByIds(List<string> ids)
        {
            throw new NotImplementedException();
        }

        public bool UpdateResult(FriendsApply model)
        {
            throw new NotImplementedException();
        }
    }
}
