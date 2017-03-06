using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
  public   class PrivateMessageViewModel
    {
       
   
        public string SenderId { get; set; }
  
        public string content { get; set; }
   
        public string SenderName { get; set; }
        public string RecevierId { get; set; }
        public DateTime CreateTime { get; set; }


      
    }
}
