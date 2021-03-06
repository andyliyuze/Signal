﻿using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using ServiceStack;
using ServiceStack.Redis;
using ServiceStack.Redis.Support;
using MeassageCache.Common;
using MeassageCache.Interface;

namespace MeassageCache
{
    public class UserService : IUserService
    {


        //用户注册
        public bool Register(string id, string UserName, string Pwd, string avatarPic)
        {

            using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
            {
                string key = "UserDetail:" + id + "";
                //插入用户详细信息
                UserDetail detail = new UserDetail { UserDetailId = Guid.Parse(id), IsOnline = false, UserName = UserName, AvatarPic = avatarPic };
                var kvp = RedisClient.ConvertToHashFn(detail);
                //   var kvp= SerializeHelper.ConvetToKeyValuePairs(model);
                redisClient.SetRangeInHash(key, kvp);
                //插入用户账号信息
                string key2 = "UserInfo:" + detail.UserDetailId + "";
                UserInfo userinfo = new UserInfo { AddTime = DateTime.Now, Pwd = Pwd, UserName = UserName, UserId = Guid.Parse(id) };
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
        public bool Login(string Id, string pwd)
        {
            //1.根据用户名找到uid
            using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
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
        public string AfterLogin(string Id, string cid)
        {
            using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
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
            using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
            {

                var map = redisClient.GetAllEntriesFromHash("UserDetail:" + Id + "");
                UserDetail model = SerializeHelper.ConvetToEntity<UserDetail>(map);
                return model;
            }

        }
        //更新用户Cid
        public bool UpdateUserCId(string Uid, string NewCid)
        {


            using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
            {

                return redisClient.SetEntryInHash("UserDetail:" + Uid + "", "UserCId", NewCid);
            }

        }
        //获取ID
        public string GetUserIdByName(string name)
        {

            using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
            {
                string Id = redisClient.Get<string>("UserIdByName:" + name + ":id");
                //如果在Id不存在则表示不存在该用户
                if (String.IsNullOrEmpty(Id) == true) { return null; }
                else { return Id; }
            }

        }

        //获取UserId

        public string GetUserIdByCId(string cid)
        {
            using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
            {
                string cmd = "UserIdByCId:" + cid + ":uid";
                string uid = redisClient.Get<string>(cmd);
                if (uid != null)
                {
                    return uid;
                }
                else { return null; }
            }
        }
        //获取Uid
        public string GetUserCIdByUId(string uid)
        {
            using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
            {
                var map = redisClient.GetAllEntriesFromHash("UserDetail:" + uid + "");
                UserDetail model = SerializeHelper.ConvetToEntity<UserDetail>(map);
                return model.UserCId;
            }
        }

    
        //退出登录
        public bool LogOut(string uid)
        {
            using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
            {
                try
                {
                    string oldCid = redisClient.Get<string>("UserCIdById:" + uid + ":cid");
                    //UserIdByCId:2e83cccc-d410-4a46-b858-9dbbba0582e9:uid
                    redisClient.Del("UserIdByCId:" + oldCid + ":uid");
                    redisClient.Del("UserCIdById:" + uid + ":cid");
                    UserDetail model = GetUserDetail(uid);
                    if (model != null)
                    {
                        //设置用户在线状态为false
                        UpdateUserField("IsOnline", "false", model.UserDetailId.ToString());
                    }
                    return true;
                }
                catch { return false; }

            }
          
           
        }


        //更新属性值
      
        public bool UpdateUserField(string key, string value, string UId)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
                {
                    return redisClient.SetEntryInHash("UserDetail:" + UId + "", key, value);
                }
            }
            catch { return false; }
        }
        
        
        //获取好友
        public List<UserDetail> GetMyFriends(string Uid)
        {
            
            var ids = GetFriendsIds(Uid);
            return GetMyFriendsDetail(ids);
        }
        //获取好友Id
        private List<string> GetFriendsIds(string id)
        {
            using (RedisClient redisClient = new RedisClient(RedisCofig.DefaultEndpoint))
            {
                List<string> IdList = redisClient.GetAllItemsFromSet("Friends:UserId:" + id + "").ToList<string>();

                return IdList;
            }
        }
        // 获取好友信息
        private List<UserDetail> GetMyFriendsDetail(List<string> ids)
        {
            List<UserDetail> userlist = new List<UserDetail>();
            foreach (var id in ids)
            {

                userlist.Add(GetUserDetail(id));


            }
            return userlist;

        }
    }

}