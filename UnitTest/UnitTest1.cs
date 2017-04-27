using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeassageCache;
using Model;
using System.Collections.Generic;
using ServiceStack.Redis;
using ServiceStack;
using ServiceStack.Redis.Support.Queue.Implementation;
using ServiceStack.Redis.Tests;
using DAL;
using Model.ViewModel;
using Common;
using SignalRChat;
using MeassageCache.Interface;
namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        RedisClient redisClient = new RedisClient("127.0.0.1", 6379);
        UserService service = new UserService();
        MessageService _Msgservice = new MessageService();
        PrivateMessages_DAL _DAL = new PrivateMessages_DAL();
        FriendsApply_DAL _applydal = new FriendsApply_DAL();
        JoinGroupApply_DAL _groupapplydal = new JoinGroupApply_DAL();
        Friends_DAL _frienddal = new Friends_DAL();
         
        [TestMethod]
        public void Test_CacheService_Register()
        {


            UserDetail model = new UserDetail {  UserName = "liyu" };
           
        }


        [TestMethod]
        public void Test_CacheService_GetUserDetail()
        {
            UserDetail expectModel = new UserDetail()
            {
                AvatarPic = "/Content/upload/image/HeadImage/20170126125314.Jpeg"
                ,
                IsOnline = false,
                UserCId = "3349469f-69e4-49f8-a8c3-b47da13e4947",
                UserDetailId = Guid.Parse("66847ba83b5343eaa69305e78827520f"),
                UserName = "mushui"

            };
            UserDetail actualModel= service.GetUserDetail("66847ba83b5343eaa69305e78827520f");
            Assert.AreEqual(expectModel, actualModel);
        }

        [TestMethod]
        public void Test_CacheService_GetFriendsIds()
        {


      //      List<string> ids = service.GetFriendsIds("06879b5b-33f4-4a26-a530-e0ea31004d86");
        }



        [TestMethod]
        public void Test_CacheService_GetMyFriendsDetail()
        {

       //     List<string> ids = service.GetFriendsIds("06879b5b-33f4-4a26-a530-e0ea31004d86");
        //    List<UserDetail> list = service.GetMyFriendsDetail(ids);
        }
          
        [TestMethod]
        public void TestMethod1()
        {
            RedisEndpoint config = new RedisEndpoint() {   Port = 6379, Host = "127.0.0.1" };
            RedisClient redisClient = new RedisClient(config);    
            var actual=  redisClient.Get<string>("UserIdByName:咸鱼翻身:id");
            Assert.IsNull(actual);
        }
          [TestMethod]
        public void test()
        {
            string str1 = "Cubic_Key.Y";
            string str2 = "Cubic_Key.Aga";
            int i = String.Compare(str1, str2);

        }
          [TestMethod]
          public void test2()
          {

           List<string>  list=   redisClient.SearchKeys("Un*");
              string key="UserInfo:06879b5b-33f4-4a26-a530-e0ea31004d86";
           UserInfo User = redisClient.GetAllEntriesFromHash(key).ToJson().FromJson<UserInfo>();
              object o =redisClient.GetFromHash<UserInfo>("UserInfo:06879b5b-33f4-4a26-a530-e0ea31004d86");
          }
        [TestMethod]
          public void test_SetUnreadMsgCount() {

             
              string key = "PrivateMessageSet:eb94d0af-3c0d-46f8-9870-5470f2c2ef40:06879b5b-33f4-4a26-a530-e0ea31004d86";
         
            int   startIndex=0;
            int EndIndex = 11;
         List<string> list=   redisClient.GetRangeFromSortedSet(key,startIndex,EndIndex);
          
          }


        [TestMethod]

        public void  test_GetUnreadMsg(){
        
        
        
    List<PrivateMessage> list=    _Msgservice.GetPrivateUnreadMsg("","","",0);
        
        }
          [TestMethod]
        public void test_RedisQueue() {

            using (var queue = new RedisSimpleWorkQueue<string>(10, 10, TestConfig.SingleHost, TestConfig.RedisPort,"PrivateMessageIdList"))
            {
               int numMessages = 6;
                var messages = new string[numMessages];
                for (int i = 0; i < numMessages; ++i)
                {
                    messages[i] = String.Format("message#{0}", i);
                    queue.Enqueue(messages[i]);
                }
                var batch = queue.Dequeue(numMessages );
                //test that batch size is respected
                Assert.AreEqual(batch.Count, numMessages);

                // test that messages are returned, in correct order
                for (int i = 0; i < numMessages; ++i)
                    Assert.AreEqual(messages[i], batch[i]);

                //test that messages were removed from queue
                batch = queue.Dequeue(numMessages * 2);
                Assert.AreEqual(batch.Count, 0);
            }
        
        }

          [TestMethod]
          public void test_Hash() {

             
               var redisTodos = redisClient.As<PrivateMessage>();
               var model= redisTodos.GetFromHash("PrivateMessageHash:107f3264-bcca-401e-a6c7-409d757f6848");
              
              
              }
             
          
          
           [TestMethod]
        public void test_pop(){
            List<PrivateMessage> list = new List<PrivateMessage>();
            PrivateMessage model = new PrivateMessage { MessageId = Guid.Parse("0d10ad72-bc1b-4bd5-8c1a-7b8a1793d48e"), SenderId = "aSASAS" };
            list.Add(model);
               _DAL.AddList(list);
            
        
        }

           [TestMethod]
           public void test_SendAddFriendsApply_DAL()
           {
               FriendsApply model = new FriendsApply { ApplyTime = DateTime.Now, ApplyUserId = Guid.Parse("527c4912-10d3-4796-b678-3c3c08b3310e"), ReceiverUserId = Guid.Parse("527c4912-10d3-4796-b678-3c3c08b3310e"), Result = "待审" };
           
               _applydal.SendAddFriendsApply(model);
           }

           [TestMethod]
           public void test_BeFriends_DAL()
           {
               Friends model = new Friends {ApplyUserId=Guid.NewGuid(),BefriendTime=DateTime.Now,FriendsId=Guid.NewGuid(),ReceiverUserId=Guid.NewGuid() };

               _frienddal.BeFriends(model);
           }

           [TestMethod]
           public void test_GetFriendsApplyByUId_DAL(){
               List<FriendsApplyViewModel> list = _applydal.GetFriendsApplyByUId(Guid.Parse("dda0e29c-a821-478e-b925-080ea9e1ee54"));
           }

          [TestMethod]
           public void test_UpdateResult() {
               FriendsApply model = new FriendsApply { ReceiverUserId = Guid.Parse("66847BA8-3B53-43EA-A693-05E78827520F"), Result = "通过", ApplyTime = DateTime.Now, FriendsApplyId = Guid.Parse("5EBA7A3D-35BB-447E-B48D-09DD2F29CE08") };
           _applydal.UpdateResult(model);
           }


        [TestMethod]

        public void test_Refelct()
        {

            ScaleConfig.GetAttribute(1);
        }


        [TestMethod]

        public void test_GetGroupApplyByUId()
        {
          var list=       _groupapplydal.GetGroupApplyByUId(Guid.Parse("DDA0E29C-A821-478E-B925-080EA9E1EE54"));


        }


        [TestMethod]

        public void test_GetGroupReplyByUId()
        {
            var list = _groupapplydal.GetGroupReplyByUId(Guid.Parse("bc01539f-0985-4db8-9dce-ef1073ba8f6c"));


        }

        [TestMethod]

        public void test_GetFriendsReplyByUId()
        {
            var list = _applydal.GetFriendsReplyByUId(Guid.Parse("66847ba8-3b53-43ea-a693-05e78827520f"));


        }

        [TestMethod]
        public void test_GetKey()
        {

        string key=    redisClient.GetKeysByPattern("UserIdByCId:bcbc100e-3efc-411f-a2f8-e8aa6e03eb6e:uid").FirstNonDefault();
        }

    }
}