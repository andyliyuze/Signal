using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using ServiceStack.Redis;
using Common;
using ServiceStack;
using Model.Extend;
namespace MeassageCache
{
    public class MessageService : IMessageService
    {

        

        //添加一对一消息
        public bool InsertPrivateMsg(PrivateMessage model )
        {

            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {
                    
                    //添加消息实体到hash类型
                    string key = "PrivateMessageHash:" + model.PrivateMessageId.ToString() + "";
                    var kvp = RedisClient.ConvertToHashFn(model);
                    redisClient.SetRangeInHash(key, kvp);
                    //将消息Id保存到list队列就nb
                    redisClient.PushItemToList("PrivateMessageList", model.PrivateMessageId.ToString());

                   //i等于1代表第一个参数大，第二个参数小，所以把小的放前面大的放后面
                    //从而构成
                    int i = String.Compare(model.SenderId, model.RecevierId);
                    //if(model.RecevierId>model.SenderId){
                    string setKey = "";
                    if (i == 1)
                    {
                        setKey = setKey + "PrivateMessageSet:" + model.SenderId + ":" + model.RecevierId + "";
                    }
                    else {

                        setKey = setKey + "PrivateMessageSet:" + model.RecevierId + ":" + model.SenderId + "";
                    }
                       long stamp=      TimeHelper.GetTimeStamp(model.CreateTime);

                    //讲消息id添加到消息集合
                    redisClient.AddItemToSortedSet(setKey, model.PrivateMessageId.ToString(), stamp);
                   
                     UpdateHistoryMsgHash(model, redisClient);
                    
                
                    return true;
                    
                }
            }
            catch { return false; }
        
        }

        public void UpdateHistoryMsgHash(PrivateMessage model, RedisClient redisClient)
        {
            //添加用户新消息到历史消息hash中，该hash对象存储a用户与b最新的MsgId，以及a用户未读b用户
            //发来的消息的条数
             
                string setHisMsgKey = "HistoryMsgHash:" + model.RecevierId + ":" + model.SenderId;

                HistoryMsgHashModel hisMsgModel = new HistoryMsgHashModel();
                hisMsgModel = redisClient.GetAllEntriesFromHash(setHisMsgKey).ToJson().FromJson<HistoryMsgHashModel>();
                hisMsgModel.MessageId = model.PrivateMessageId.ToString();
                hisMsgModel.UnReadMsgCount = hisMsgModel.UnReadMsgCount + 1; 
           
                var HisMsgkvp = RedisClient.ConvertToHashFn(hisMsgModel);
                redisClient.SetRangeInHash(setHisMsgKey, HisMsgkvp);
  

        }
            
        public bool RemoveUnReadPriMsg(string RecevierId)
        {


            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {

                    string setUnReadKey = "UnreadPrivateMessageSet:" + RecevierId;
                    return redisClient.Del(setUnReadKey) > 0;

                }

            }

            catch { return false; }
        
        
        
        
        }
         //获得两人聊天记录

        //获得单条消息
        public PrivateMessage GetItem(string key) {
            using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
            {
             PrivateMessage model=   redisClient.GetAllEntriesFromHash(key).ToJson().FromJson<PrivateMessage>();
             return model;
            }
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

        //获得未读消息数量
        public int GetUnReadCount(string Uid) {

            try {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                { 
                List<string> list =redisClient.GetAllItemsFromSortedSet("UnreadPrivateMessageSet:"+Uid);
                return list.Count();
                
               
                }
            }
            catch{return 0;}
            
            }
        //登陆后就获得与每个好友的历史消息
        public List<HistoryMsgViewModel> GetHistoryMsg(string Id)
        {
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {
                    List<string> list = redisClient.SearchKeys("HistoryMsgHash:" + Id + "*");
                    List<HistoryMsgViewModel> hisList = new List<HistoryMsgViewModel>();
                    foreach (string str in list)
                    {

                        HistoryMsgHashModel hisMsgModel = new HistoryMsgHashModel();
                        HistoryMsgViewModel hisMsgViewModel = new HistoryMsgViewModel();
                        PrivateMessage priMsgModel = new PrivateMessage();

                         //获得对应的历史消息model，存在redis的model
                        hisMsgModel = redisClient.GetAllEntriesFromHash(str).ToJson().FromJson<HistoryMsgHashModel>();
                        //获得单条消息model
                        priMsgModel = redisClient.GetAllEntriesFromHash("PrivateMessageHash:"+hisMsgModel.MessageId).ToJson().FromJson<PrivateMessage>();
                      //获得历史消息model ，前端显示的model
                        hisMsgViewModel.UnReadMsgCount = hisMsgModel.UnReadMsgCount;        
                        hisMsgViewModel.Message = priMsgModel;
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
        public bool SetUnreadMsgCount( string     RecevierId,string SenderId,int count) {
            try
            {
                using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
                {
                   string     setHisMsgKey =  "HistoryMsgHash:" +RecevierId +":"+SenderId;
                    //HistoryMsgHashModel hisMsgModel=new HistoryMsgHashModel();
                    //hisMsgModel.UnReadMsgCount=count;
                    //var HisMsgkvp = RedisClient.ConvertToHashFn(hisMsgModel);
                    //redisClient.SetRangeInHash(setHisMsgKey, HisMsgkvp);
                    redisClient.SetEntryInHash(setHisMsgKey, "UnReadMsgCount", count.ToString());
                    return true;
                }
            }
            catch 
            {
                return false;
            
            }
        
        }
        public List<PrivateMessage> GetUnreadMsg(string ReceiverId, string SenderId, string msgId, int count) {
            using (RedisClient redisClient = new RedisClient("localhost", 6379)) {
             
                int i = String.Compare(SenderId, ReceiverId);
                //if(model.RecevierId>model.SenderId){
                string setKey = "";
                if (i == 1)
                {
                    setKey = setKey + "PrivateMessageSet:" + SenderId + ":" + ReceiverId + "";
                }
                else
                {
                   
                    setKey = setKey + "PrivateMessageSet:" + ReceiverId + ":" + SenderId + "";
                 
                }



                int EndIndex = (int)redisClient.GetItemIndexInSortedSet(setKey, msgId) ;
                int startIndex =EndIndex- count+1;
                List<string> list = redisClient.GetRangeFromSortedSet(setKey, startIndex, EndIndex);
                List<PrivateMessage> MsgList = new List<PrivateMessage>();
                foreach (string str in list) {
                    PrivateMessage priMsgModel = new PrivateMessage();
                    priMsgModel = redisClient.GetAllEntriesFromHash("PrivateMessageHash:" + str).ToJson().FromJson<PrivateMessage>();
                    MsgList.Add(priMsgModel);
                }
                return MsgList;
            }
          
        
        
        
        }


        public List<PrivateMessage> PopMesFromMessageList(string MessageListkey,int count) {
            using (RedisClient redisClient = new RedisClient("localhost", 6379))
            {
                List<PrivateMessage> msgList = new List<PrivateMessage>();

                for (int i = 0; i < count; i++) {
                    string MessageItemKey = redisClient.PopItemFromList(MessageListkey);
                    if (MessageItemKey == null) { return msgList; }
                    MessageItemKey="PrivateMessageHash:"+MessageItemKey;
                 
                    PrivateMessage model = redisClient.GetAllEntriesFromHash(MessageItemKey).ToJson().FromJson<PrivateMessage>();
                    msgList.Add(model);
                }

           
               
                return msgList;
            }
        
        }
    }


    public static class MessageServiceExtend
    {
   

    }
      

}
