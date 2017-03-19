using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
  public  class ReplyStatusHelper
    {
        public static ReplyStatus ConvertToReplyStatus(string resultstr)
        {

            if (resultstr.Trim().Contains("通过")) { return ReplyStatus.Pass; }
            if (resultstr.Trim().Contains("拒绝")) { return ReplyStatus.Decline; }
            if (resultstr.Trim().Contains("忽略")) { return ReplyStatus.Ignore; }
            else return ReplyStatus.Decline;


        }
    }



    public enum ReplyStatus
    {


        Pass = 0,
        Decline = 1,
        Ignore = 2,

    }
}
