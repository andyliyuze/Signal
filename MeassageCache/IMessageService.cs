using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeassageCache
{
  public  interface IMessageService
    {
       List<PrivateMessage> GetPrivateMessage(string key, string beginStamp, string endStamp);
       bool  InsertPrivateMsg(PrivateMessage model);
        bool InsertBroadcastMsg(BroadcastMessage model);
        int GetUnReadCount(string Uid);
       List<HistoryMsgViewModel> GetHistoryMsg(string Id);
       bool SetUnreadMsgCount(string RecevierId, string SenderId, int count);
       List<PrivateMessage> GetUnreadMsg(string ReceiverId, string SenderId, string MsgId, int count);

       List<PrivateMessage> PopMesFromMessageList(string MessageListkey, int count);

      
    }
}
