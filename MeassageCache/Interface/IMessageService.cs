using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeassageCache.Interface
{
  public  interface IMessageService
    {
       List<PrivateMessage> GetPrivateMessage(string key, string beginStamp, string endStamp);
       bool  InsertPrivateMsg(PrivateMessage model);
    
       List<HistoryMsgViewModel> GetHistoryMsg(string Id);
       bool SetUnreadPrivateMsgCount(string RecevierId, string SenderId, int count);
       List<PrivateMessage> GetPrivateUnreadMsg(string ReceiverId, string SenderId, string MsgId, int count);

       List<PrivateMessage> PopMesFromMessageList(string MessageListkey, int count);




        bool InsertBroadcastMsg(BroadcastMessage model);

        List<BroadcastMessage> GetUnreadBroadcastMsg(string groupId, string msgId, int count);
        bool SetUnreadBroadcasMsgCount(string groupId, int count);
    }
}
