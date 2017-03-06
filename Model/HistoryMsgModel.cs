using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{

    //存入redis的数据结构，存在a用户与b用户的最新消息Id与未读消息数量
  public  class HistoryMsgHashModel
    {
      public string MessageId { get; set; }
      public int UnReadMsgCount { get; set; }
    }

   //显示在前端的model
  public class HistoryMsgViewModel {

      public PrivateMessage Message { get; set; }

      public int UnReadMsgCount { get; set; }
  
  }
}
