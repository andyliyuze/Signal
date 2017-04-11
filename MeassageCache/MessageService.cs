using System;
using System.Collections.Generic;
using Model;
using ServiceStack.Redis;
using Common;
using ServiceStack;
using MeassageCache.Interface;
namespace MeassageCache
{
    public class MessageService : IMessageService
    {


        #region 单人消息
        //添加一对一消息
        public bool InsertPrivateMsg(PrivateMessage model)
        {

            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {

                    //添加消息实体到hash类型
                    string key = "PrivateMessageHash:" + model.MessageId.ToString() + "";
                    var kvp = RedisClient.ConvertToHashFn(model);
                    kvp.Remove("ChattingId");
                    kvp.Remove("type");
                    redisClient.SetRangeInHash(key, kvp);
                    //将消息Id保存到list队列就nb
                    redisClient.PushItemToList("PrivateMessageList", model.MessageId.ToString());

                    //i等于1代表第一个参数大，第二个参数小，所以把小的放前面大的放后面
                    //从而构成
                    int i = String.Compare(model.SenderId, model.RecevierId);
                    string setKey = "";
                    if (i == 1)
                    {
                        setKey = setKey + "PrivateMessageSet:" + model.SenderId + ":" + model.RecevierId + "";
                    }
                    else
                    {

                        setKey = setKey + "PrivateMessageSet:" + model.RecevierId + ":" + model.SenderId + "";
                    }
                    long stamp = TimeHelper.GetTimeStamp(model.CreateTime);

                    //讲消息id添加到消息集合
                    redisClient.AddItemToSortedSet(setKey, model.MessageId.ToString(), stamp);

                    UpdateHistoryMsgHash(model, redisClient);


                    return true;

                }
            }
            catch { return false; }

        }
        //单人消息的历史消息hash ,储存未读消息数量以及最新消息Id
        private void UpdateHistoryMsgHash(PrivateMessage model, RedisClient redisClient)
        {
            //更新接受方与发送方，该hash对象存储a用户与b最新的MsgId，以及a用户未读b用户
            //发来的消息的条数
            string HisMsgKeyForRecevier = "HistoryMsgHash:" + model.RecevierId + ":" + model.SenderId;
            HistoryMsgHashModel hisMsgModel = new HistoryMsgHashModel();
            hisMsgModel = redisClient.GetAllEntriesFromHash(HisMsgKeyForRecevier).ToJson().FromJson<HistoryMsgHashModel>();
            hisMsgModel.MessageId = model.MessageId.ToString();
            hisMsgModel.UnReadMsgCount = hisMsgModel.UnReadMsgCount + 1;
            var HisMsgkvp = RedisClient.ConvertToHashFn(hisMsgModel);
            redisClient.SetRangeInHash(HisMsgKeyForRecevier, HisMsgkvp);


            //更新发送方与接收方的最新消息Id
            string HisMsgKeyForSender = "HistoryMsgHash:" + model.SenderId + ":" + model.RecevierId;
            redisClient.SetEntryInHash(HisMsgKeyForSender, "MessageId", model.MessageId.ToString());
        }
      
        public List<PrivateMessage> GetPrivateMessage(string key, string beginStamp, string endStamp)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {
                    List<string> msglist = redisClient.SearchSortedSet(key, beginStamp, endStamp);
                    List<PrivateMessage> list = new List<PrivateMessage>(); ;
                    foreach (string str in msglist)
                    {
                        PrivateMessage model = redisClient.GetAllEntriesFromHash(str).ToJson().FromJson<PrivateMessage>();
                        list.Add(model);
                    }
                    return list;
                }
            }
            catch { return null; }
        }
  
        //登陆后就获得与每个好友的历史消息
        public List<HistoryMsgViewModel> GetHistoryMsg(string UserId)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {
                    //接收方与发送方
                    List<string> list = redisClient.SearchKeys("HistoryMsgHash:" + UserId + "*");
                    List<HistoryMsgViewModel> hisList = new List<HistoryMsgViewModel>();
                    foreach (string str in list)
                    {
                        HistoryMsgHashModel hisMsgModel = new HistoryMsgHashModel();
                        HistoryMsgViewModel hisMsgViewModel = new HistoryMsgViewModel();
                        PrivateMessage priMsgModel = new PrivateMessage();
                        //获得对应的历史消息model，存在redis的model
                        hisMsgModel = redisClient.GetAllEntriesFromHash(str).ToJson().FromJson<HistoryMsgHashModel>();
                        //获得单条消息model
                        priMsgModel = redisClient.GetAllEntriesFromHash("PrivateMessageHash:" + hisMsgModel.MessageId).ToJson().FromJson<PrivateMessage>();
                        //获得历史消息model ，前端显示的model
                        hisMsgViewModel.UnReadMsgCount = hisMsgModel.UnReadMsgCount;
                        hisMsgViewModel.Message = priMsgModel;
                        if (priMsgModel.SenderId.Trim() == UserId.Trim()) { hisMsgViewModel.Message.ChattingId = priMsgModel.RecevierId; }
                        else { hisMsgViewModel.Message.ChattingId = priMsgModel.SenderId; }
                        hisList.Add(hisMsgViewModel);
                    }
                    return hisList;
                }
            }
            catch
            {
                return null;
            }
        }
        //让未读消息变为0
        public bool SetUnreadPrivateMsgCount(string recevierId, string senderId, int count)
        {

            string HisMsgKey = "HistoryMsgHash:" + recevierId + ":" + senderId;
            return SetUnreadMsgCount(HisMsgKey, count);
        }

        public List<PrivateMessage> GetPrivateUnreadMsg(string ReceiverId, string SenderId, string msgId, int count)
        {
                string hashKey = "PrivateMessageHash";
                int i = String.Compare(SenderId, ReceiverId);
                string setKey = "";
                if (i == 1)
                {
                    setKey = setKey + "PrivateMessageSet:" + SenderId + ":" + ReceiverId + "";
                }
                else
                {
                    setKey = setKey + "PrivateMessageSet:" + ReceiverId + ":" + SenderId + "";
                }
            return GetUnreadMsgList<PrivateMessage>(setKey, hashKey, msgId, count);
        }
        
        public List<PrivateMessage> PopMesFromMessageList(string MessageListkey, int count)
        {
            using (RedisClient redisClient = new RedisClient("localhost", 6379))
            {
                List<PrivateMessage> msgList = new List<PrivateMessage>();

                for (int i = 0; i < count; i++)
                {
                    string MessageItemKey = redisClient.PopItemFromList(MessageListkey);
                    if (MessageItemKey == null) { return msgList; }
                    MessageItemKey = "PrivateMessageHash:" + MessageItemKey;

                    PrivateMessage model = redisClient.GetAllEntriesFromHash(MessageItemKey).ToJson().FromJson<PrivateMessage>();
                    msgList.Add(model);
                }



                return msgList;
            }

        }
        #endregion


        private string GetKeyForTowUserOrder(string keyBegin, string senderId, string recevierId)
        {
            //i等于1代表第一个参数大，第二个参数小，所以把小的放前面大的放后面
            //从而构成
            int i = String.Compare(senderId, recevierId);
            //if(model.RecevierId>model.SenderId){
            string setKey = keyBegin;
            if (i == 1)
            {
                setKey = setKey + senderId + ":" + recevierId + "";
            }
            else
            {

                setKey = setKey + recevierId + ":" + senderId + "";
            }
            return setKey;

        }


        #region 群聊消息
        //插入多人消息
        public bool InsertBroadcastMsg(BroadcastMessage model)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {
                    //添加消息实体到hash类型
                    string key = "BroadcastMessageHash:" + model.MessageId.ToString() + "";
                    var kvp = RedisClient.ConvertToHashFn(model);
                    kvp.Remove("ChattingId");
                    kvp.Remove("type");
                    redisClient.SetRangeInHash(key, kvp);
                    //将消息Id保存到list队列
                    redisClient.PushItemToList("BroadcastMessageList", model.MessageId.ToString());                  
                    string setKey = "BroadcastMessageSet:" + model.GroupId + "";
                    long stamp = TimeHelper.GetTimeStamp(model.CreateTime);
                    //讲消息id添加到消息集合
                    redisClient.AddItemToSortedSet(setKey, model.MessageId.ToString(), stamp);
                    UpdateBroadcastMsgHash(model, redisClient);
                    return true;
                }
            }
            catch { return false; }
        }
        //多人消息的历史hash
        private void UpdateBroadcastMsgHash(BroadcastMessage model, RedisClient redisClient)
        {
            //添加用户新消息到历史消息hash中，该hash对象存储a用户与b最新的MsgId，以及a用户未读b用户
            //发来的消息的条数
            string setHisMsgKey = "HistoryBroadcastMsgHash:" + model.GroupId;
            HistoryMsgHashModel hisMsgModel = new HistoryMsgHashModel();
            hisMsgModel = redisClient.GetAllEntriesFromHash(setHisMsgKey).ToJson().FromJson<HistoryMsgHashModel>();
            hisMsgModel.MessageId = model.MessageId.ToString();
            hisMsgModel.UnReadMsgCount = hisMsgModel.UnReadMsgCount + 1;
            var HisMsgkvp = RedisClient.ConvertToHashFn(hisMsgModel);
            redisClient.SetRangeInHash(setHisMsgKey, HisMsgkvp);
        }
        //获取未读的历史消息
        public List<BroadcastMessage> GetUnreadBroadcastMsg(string groupId, string msgId, int count)
        {
            string setKey = "BroadcastMessageSet:" + groupId;
            string hashKey = "HistoryBroadcastMsgHash";
            return  GetUnreadMsgList<BroadcastMessage>(setKey, hashKey, msgId, count);
        }
        //让未读消息变为0
        public bool SetUnreadBroadcasMsgCount(string groupId, int count)
        {
            string HisMsgKey = "HistoryMsgHash:" + groupId;
            return SetUnreadMsgCount(HisMsgKey, count);
        }
        #endregion

        #region 通用方法
        //获得单条消息
        public T GetItem<T>(string key)
        {
            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {
                T model = redisClient.GetAllEntriesFromHash(key).ToJson().FromJson<T>();
                return model;
            }
        }
        //获去未读历史消息
        private List<T> GetUnreadMsgList<T>(string setKey, string hashKey, string msgId, int count)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient("localhost", 6379))
                {
                    int EndIndex = (int)redisClient.GetItemIndexInSortedSet(setKey, msgId);
                    int startIndex = EndIndex - count + 1;
                    List<string> list = redisClient.GetRangeFromSortedSet(setKey, startIndex, EndIndex);
                    List<T> MsgList = new List<T>();
                    foreach (string str in list)
                    {
                        T priMsgModel = redisClient.GetAllEntriesFromHash(hashKey + ":" + str).ToJson().FromJson<T>();
                        MsgList.Add(priMsgModel);
                    }
                    return MsgList;
                }
            }
            catch { return new List<T>(); }
        }
        private bool SetUnreadMsgCount(string HisMsgKey, int count)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {

                    redisClient.SetEntryInHash(HisMsgKey, "UnReadMsgCount", count.ToString());
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}