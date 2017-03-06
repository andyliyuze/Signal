using MeassageCache.Common;
using Model;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
         //好友申请操作
        public void SendAddFriendsApply(string uidA, string uidB) {

            //uidA代表申请发出者
            //uidB代表申请接受者
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {
                    string key = "Friends:Reply:" + uidA + "";
                    string key2 = "Friends:Receive:" + uidB + "";
                    //Friends:Reply代表发出的好友添加申请
                    //Friends:Receive代表收到的好友添加申请
                    redisClient.AddItemToList(key, uidB);
                    redisClient.AddItemToList(key2, uidA);
               
                }
            }
            catch(Exception e)
            {
                throw e;

            }
        }

 
    }
}
