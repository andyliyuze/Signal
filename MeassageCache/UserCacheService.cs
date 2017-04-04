using ServiceStack.Redis;
using MeassageCache.Interface;
namespace MeassageCache
{
    public class UserCacheService : IUserCacheService
    {
        //正式成为好友操作
        public bool BeFriends(string uidA, string uidB) 
        {
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {
                    string key=   "Friends:UserId:" + uidA + "";
                    string key2 = "Friends:UserId:" + uidB + "";
                    //双方好友集合都添加一条数据，表示双方成为好友
                    redisClient.AddItemToSet(key, uidB);
                    redisClient.AddItemToSet(key2,uidA);
                    return true;
                }
            }
            catch
            {
                return false;
            
            }
        } 
    }
}
