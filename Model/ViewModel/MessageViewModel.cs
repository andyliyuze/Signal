using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class  Message:PrivateMessage
   {
        public string GroupId { get; set; }
        public string ChattingId { get; set; }
        public string type { get; set; }
        public string SenderAvatar { get; set; }
    }
}
