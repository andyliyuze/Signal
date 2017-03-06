using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
 
using Model;
using ServiceStack;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using ServiceStack.Redis.Support;
using System.Diagnostics;
using MeassageCache.Common;
using Common;
using DAL;

namespace MeassageCache
{
    public class CacheService : ICacheService
    {
      
     
        //用户注册
        public bool Register(string id, string UserName, string Pwd,string avatarPic)
        {
           
            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {
                string key = "UserDetail:" + id + "";
                //插入用户详细信息
                UserDetail detail = new UserDetail { UserDetailId = Guid.Parse(id), IsOnline = false, UserName = UserName, AvatarPic = avatarPic };
                var kvp = RedisClient.ConvertToHashFn(detail);
                //   var kvp= SerializeHelper.ConvetToKeyValuePairs(model);
                redisClient.SetRangeInHash(key, kvp);
                //插入用户账号信息
                string key2 = "UserInfo:" + detail.UserDetailId + "";
                UserInfo userinfo =new UserInfo{AddTime=DateTime.Now,Pwd=Pwd,UserName=UserName,UserId=Guid.Parse(id)};
                var kvp2 = SerializeHelper.ConvetToKeyValuePairs(userinfo);
                redisClient.SetRangeInHash(key2, kvp2);

                


                //同时将用户信息，用户账号信息插入到list中,持久化到sqlserver使用
                //redisClient.SAdd("NewUserDetail", SerializeHelper.SerializeModel<UserDetail>(detail));
                //redisClient.SAdd("NewUserInfo", SerializeHelper.SerializeModel<UserInfo>(userinfo));
                redisClient.Set<string>("UserIdByName:" + detail.UserName + ":id", detail.UserDetailId.ToString()); //根据用户名找到id
            }
            return true;

        }
        //登录检验
        public bool Login(string Id,string pwd)
        {
            //1.根据用户名找到uid
            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {
                string key = "UserInfo:" + Id + "";
                //获取hash对象的方法
                //检验密码
                UserInfo User = redisClient.GetAllEntriesFromHash(key).ToJson().FromJson<UserInfo>();
                //     =  redisClient<UserInfo>(key);
                string Pwd = User.Pwd;
                if (pwd == Pwd)
                {
                  
                
                 
                    return true;
                }

                else { return false; }
            }
        }

        

        //用户登录成功之后的操作
        public string AfterLogin(string Id,string cid)
        {
            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {
                try
                {


                    string oldCid = redisClient.Get<string>("UserCIdById:" + Id + ":cid");
                    //UserIdByCId:2e83cccc-d410-4a46-b858-9dbbba0582e9:uid
                     redisClient.Del("UserIdByCId:" + oldCid + ":uid");


                          
                    //设置新的cId，根据cid找到uid
                    redisClient.Set<string>("UserIdByCId:" + cid + ":uid", Id);

                    redisClient.Set<string>("UserCIdById:" + Id + ":cid", cid);
                    //设置用户在线状态
                    redisClient.SetEntryInHash("UserDetail:" + Id + "", "IsOnline", "true");
                    //获取未读消息
                 
                    return oldCid;

                }
                catch
                {
                    return null;
                }
              
            }
        }


        //获取用户信息
        public UserDetail GetUserDetail(string Id)
        {
            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {

                var map = redisClient.GetAllEntriesFromHash("UserDetail:" + Id + "");
                UserDetail model = SerializeHelper.ConvetToEntity<UserDetail>(map);
                return model;
            }

        }
        //更新用户Cid
        public bool UpdateUserCId(string Uid, string Cid)
        {


            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {

                return redisClient.SetEntryInHash("UserDetail:" + Uid + "", "UserCId", Cid);
            }

        }
        //获取ID
        public string GetUserIdByName(string name)
        {

            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {
                string Id = redisClient.Get<string>("UserIdByName:" + name + ":id");
                //如果在Id不存在则表示不存在该用户
                if (String.IsNullOrEmpty(Id) == true) { return null; }
                else { return Id; }
            }

        }
        //获取好友Id
        public List<string> GetFriendsIds(string id)
        {
            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {
                List<string> IdList = redisClient.GetAllItemsFromSet("Friends:UserId:" + id + "").ToList<string>();

                return IdList;
            }
        }
        // 获取好友信息
        public List<UserDetail> GetMyFriendsDetail(List<string> ids)
        {
            List<UserDetail> userlist = new List<UserDetail>();
            foreach (var id in ids)
            {

                userlist.Add(GetUserDetail(id));


            }
            return userlist;

        }
        public bool AddNewUser(string id, string cid, string username)
        {
            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {
                if (redisClient.Get<string>("UserIdByCId:" + cid + ":id") == null)
                {
                    //先往OnlineUsers集合里插入元素，表示在线用户信息
                    var ser = new ObjectSerializer();
                    byte[] buffer = ser.Serialize(new UserDetail { UserDetailId =Guid.Parse(id), UserName = username });

                    redisClient.SAdd("OnlineUsers", buffer);
                    //接着设置不同字段，该作用是用来条件查询

                    bool flag = redisClient.Set<string>("user:" + id + ":name", username) //根据id找到用户名
                    && redisClient.Set<string>("user:" + cid + ":id", id.ToString())     //根据cid找到 id
                    && redisClient.Set<string>("user:" + id + ":cid", cid)  //根据id找到 cid
                    && redisClient.Set<string>("user:" + username + ":id", id.ToString()); //根据用户名找到id


                    return flag;
                }
                else { return false; }
            }
        }
        //退出登录
        public string LogOut(string uid)
        {
            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {
                try
                {
                    string oldCid = redisClient.Get<string>("UserCIdById:" + uid + ":cid");
                    //UserIdByCId:2e83cccc-d410-4a46-b858-9dbbba0582e9:uid
                    redisClient.Del("UserIdByCId:" + oldCid + ":uid");
                    redisClient.Del("UserCIdById:" + uid + ":cid");
                }
                catch { }
             
            }
                UserDetail model = GetUserDetail(uid);
                if (model != null)
                {
                    //设置用户在线状态为false
                    UpdateUserField("IsOnline", "false", model.UserDetailId.ToString());
                    
                    return model.UserName;
                }
                else { return null; }
            }
         
        
        public string GetUserIdByCId(string cid)
        {
            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {
                string cmd = "UserIdByCId:"+cid+ ":uid";
                string uid = redisClient.Get<string>(cmd);
                if (uid != null)
                {
                    return uid;
                }
                else { return null; }
            }
        }
        public string GetUserCId(string uid)
        {
            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {
                string cmd = "UserCIdById:" + uid + ":cid";
                string cid = redisClient.Get<string>(cmd);
                if (cid != null)
                {
                    return cid;
                }
                else { return null; }
            }
        }



    
        public bool AddBroadcastMessage(List<BroadcastMessage> list)
        {


            RedisClient redisClient = new RedisClient("127.0.0.1", 6379);



            //   byte[] buffer3 = SerializeUtilities.Serialize<BroadcastMessage>(bro);
            //使用ServiceStack.Redis.Support提供的序列化方法
            var ser = new ObjectSerializer();
            byte[] buffer3 = ser.Serialize(list);


            // 将序列化后的集合查到到redis的list类型
            long BeforeLen = redisClient.LLen("BroadCastList");
            long AfterLen = redisClient.LPush("BroadCastList", buffer3);
            list.Clear();
            if (AfterLen - BeforeLen > 0) { return true; }
            else { return false; }



        }
        public bool AddPrivateMessage(List<PrivateMessage> list)
        {
            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {
                // byte[] buffer3 = SerializeUtilities.Serialize<PrivateMessage>(bro);               
                //使用ServiceStack.Redis.Support提供的序列化方法
                var ser = new ObjectSerializer();
                byte[] buffer3 = ser.Serialize(list);
                long BeforeLen = redisClient.LLen("PrivateList");
                long AfterLen = redisClient.LPush("PrivateList", buffer3);
                if (AfterLen - BeforeLen > 0) { return true; }
                else { return false; }
            }
        }

        public bool UpdateUserField(string key, string value, string UId)
        {

            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {

                return redisClient.SetEntryInHash("UserDetail:" + UId + "", key, value);

            }
        }

    }

}