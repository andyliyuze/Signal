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
    public class GroupApplyService : IGroupApplyService
    {
        public bool Add(JoinGroupApply model)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
                {
                    //添加消息实体到hash类型
                    string key = "GroupApply:" + model.Id.ToString() + "";
                    var kvp = RedisClient.ConvertToHashFn(model);
                    redisClient.SetRangeInHash(key, kvp);
                    return true;
                }
            }
            catch { return false; }
        }

        public List<GroupApplyViewModel> GetGroupApplyByUId(Guid Id)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
                {
                    List<GroupApplyViewModel> ApplyIdList = new List<GroupApplyViewModel>();
                    //获取好友申请的Id集合
                    string key = "FriendsApplySet:" + Id.ToString() + "";
                    List<string> FriendsApplyList = redisClient.GetAllItemsFromSet(key).ToList();
                    foreach (var id in FriendsApplyList)
                    {
                        JoinGroupApply item = redisClient.GetEntity<JoinGroupApply>(id);
                        UserDetail user = redisClient.GetEntity<UserDetail>(item.ApplyUserId.ToString());
                        Group group = redisClient.GetEntity<Group>(item.GroupId.ToString());
                        GroupApplyViewModel ViewModel = GroupApplyViewModel.ConvertToGroupApplyViewModel(user, item, group);
                        if (ViewModel != null) { ApplyIdList.Add(ViewModel); }
                    }
                    return ApplyIdList;

                }
            }
            catch { return new List<GroupApplyViewModel>(); }
        }

        public List<GroupReplyViewModel> GetGroupReplyByUId(Guid Id)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
                {

                    //首先取得该用户所有申请记录Id，并且是未读的，已读的话会被清理了
                    List<string> applyIds = redisClient.GetAllItemsFromSet("FreindsApply:ApplyUser:" + Id).ToList();
                    List<GroupReplyViewModel> replys = new List<GroupReplyViewModel>();

                    foreach (string id in applyIds)
                    {
                        JoinGroupApply apply = redisClient.GetEntity<JoinGroupApply>(id);
                        Group group = redisClient.GetEntity<Group>(apply.GroupId.ToString());
                        GroupReplyViewModel item = GroupReplyViewModel.ConvertToGroupReplyViewModel(group, apply);
                        if (item != null) { replys.Add(item); }
                    }
                    return replys;
                }
            }
            catch { return new List<GroupReplyViewModel>(); }
        }

        public JoinGroupApply GetItemById(Guid Id)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
                {
                    string key = "GroupApply" + Id;
                    return redisClient.GetEntity<JoinGroupApply>(key);
                }
            }
            catch
            {
                return new JoinGroupApply();
            }

        }
        public bool SetReadByIds(List<string> ids)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
                {
                    string key = "HasReadResult";
                    string value = "已读";
                    foreach (string id in ids)
                    {
                        redisClient.SetEntryInHash("GroupApply:" + id + "", key, value);
                    }
                    return true;
                }
            }
            catch { return false; }
        }

        public bool UpdateResult(JoinGroupApply apply)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
                {
                    List<KeyValuePair<string, string>> kvp = new List<KeyValuePair<string, string>>();
                    kvp.Add(new KeyValuePair<string, string>("Result", apply.Result));
                    kvp.Add(new KeyValuePair<string, string>("ReplyTime", apply.ReplyTime.ToString()));
                    kvp.Add(new KeyValuePair<string, string>("HasReadResult", apply.HasReadResult));
                    string key = "GroupApply" + apply.Id;       
                    redisClient.SetRangeInHash(key, kvp);
                    return true;
                }
            }
            catch { return false; }
        }
    }
}
