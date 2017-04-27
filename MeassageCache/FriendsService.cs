using ServiceStack.Redis;
using MeassageCache.Interface;
namespace MeassageCache
{
    public class FriendsService : IFriendsService
    {
        //正式成为好友操作
        public bool BeFriends(string uidA, string uidB) 
        {
            try
            {
                RedisEndpoint config = new RedisEndpoint() { Password = "123456", Port = 6379, Host = "127.0.0.1" };
                using (RedisClient redisClient = new RedisClient(config))
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
