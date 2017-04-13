using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
   // public class  Message:PrivateMessage
   //{
   //     public string GroupId { get; set; }
   //     public string ChattingId { get; set; }
   //     public string type { get; set; }
   //     public string SenderAvatar { get; set; }
   // }

    public class Message  
    {     
        [NotMapped]
        public string ChattingId { get; set; }

        [NotMapped]
        public string type { get; set; }
        [NotMapped]
        public string MessageIdUserForJs { get; set; }

        public string SenderAvatar { get; set; }     
        public virtual Guid MessageId { get; set; }
        public virtual string SenderId { get; set; }     
        public virtual string content { get; set; }
        public virtual string SenderName { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

}
