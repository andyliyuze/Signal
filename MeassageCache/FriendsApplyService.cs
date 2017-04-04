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
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {

                    //首先取得该用户所有申请记录Id，并且是未读的，已读的话会被清理了
                   List<string> applyIds=  redisClient.GetAllItemsFromSet("FreindsApply:ApplyUser:" + Id).ToList();
                    List<FriendsReplyViewModel> replys = new List<FriendsReplyViewModel>();

                    foreach (string id in applyIds)
                    {
                       FriendsApply apply=   redisClient.GetEntity<FriendsApply>(id);
                       UserDetail user= redisClient.GetEntity<UserDetail>(apply.ReceiverUserId.ToString());
                        FriendsReplyViewModel item =FriendsReplyViewModel.ConvertToFriendsReplyViewModel(user, apply);
                        if (item != null) { replys.Add(item); }
                    }
                    return replys;
                }
            }
            catch { return new List<FriendsReplyViewModel>(); }
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
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {
                    string key = "HasReadResult";
                    string value = "已读";
                    foreach (string id in ids)
                    {
                         redisClient.SetEntryInHash("FriendsApply:" + id + "", key, value);
                    }
                    return true;
                }
            }
            catch { return false; }
        }

        public bool UpdateResult(FriendsApply model)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {
                    List<KeyValuePair<string, string>> kvp = new List<KeyValuePair<string, string>>();
                    kvp.Add(new KeyValuePair<string, string>("Result", model.Result));
                    kvp.Add(new KeyValuePair<string, string>("ReplyTime", model.ReplyTime.ToString()));
                    kvp.Add(new KeyValuePair<string, string>("HasReadResult", model.HasReadResult));
                    string key = "FriendsApply" + model.FriendsApplyId;
                    redisClient.SetRangeInHash(key, kvp);
                    return true;
                }
            }
            catch { return false; }
        }
    }
}
