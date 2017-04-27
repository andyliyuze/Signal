using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Model.ViewModel;
using ServiceStack.Redis;
using MeassageCache.Common;
using MeassageCache.Interface;
namespace MeassageCache
{
   public  class GroupService : IGroupService
    {
        //创建群
        public bool Create(Group model)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
                {
                    //添加消息实体到hash类型
                    string key = "Group:" + model.GroupId.ToString() + "";
                    var kvp = RedisClient.ConvertToHashFn(model);                
                    redisClient.SetRangeInHash(key, kvp);
                    return true;
                }
            }
            catch { return false; }
        }


        //搜索群
        public GroupViewModel GetGroupDeatailByGroupName(string Name)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
                {
                 
                    //添加消息实体到hash类型
                    string Idkey = "Group:" + Name  + ":GroupId";
                    string key = "Group" +redisClient.Get<string>(Idkey);
                    Group item = redisClient.GetEntity<Group>(key);
                    string userkey = "UserDetail:" + item.OwnerId.ToString();
                    UserDetail owner = redisClient.GetEntity<UserDetail>(userkey);
                    GroupViewModel model = new GroupViewModel()
                    { GroupAvatar = item.GroupAvatar, GroupId = item.GroupId, GroupName = item.GroupName, OwnerId = item.OwnerId, OwnerName = owner.UserName };
                    return model;
                }
            }
            catch { return new GroupViewModel(); }
        }



        public Group GetItemByGroupId(Guid Id)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
                {

                    //添加消息实体到hash类型
                    string key = "Group:" + Id.ToString();
                    Group item = redisClient.GetEntity<Group>(key);
                   
                    return item;
                }
            }
            catch { return new Group(); }
        }


        //查找个人的所有群
        public List<Group> GetMyGroups(Guid UserId)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
                {
                    List<Group> groupList = new List<Group>();
                    //添加消息实体到hash类型
                    string key = "Group:UserId" + UserId.ToString() ;
                    List<string> GroupIdList = redisClient.GetAllItemsFromSet(key).ToList();
                    foreach (var id in GroupIdList)
                    {
                        Group item = redisClient.GetEntity<Group>(id);
                        groupList.Add(item);
                    }
                    return groupList;
                }
            }
            catch { return new List<Group>(); }
        }
    }
}
