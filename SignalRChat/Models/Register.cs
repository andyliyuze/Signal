using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalRChat.Models
{
    public class RegisterModel
    {
       
        public Guid UserDetailId { get; set; }
     
        public string UserCId { get; set; }
        [Required(ErrorMessage = "请输入用户名")]
 
        [MaxLength(10, ErrorMessage = "用户名长度不能超过10个字符")]
        [MinLength(4, ErrorMessage = "用户名长度至少为6个字符")]
        public string UserName { get; set; }

        public int Age { get; set; }
      
        public bool IsOnline { get; set; }

        public string AvatarPic { get; set; }


        

        [Required(ErrorMessage = "请输入密码！")]
        [StringLength(100, ErrorMessage = "密码必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Pwd { get; set; }
        [System.ComponentModel.DataAnnotations.Compare("Pwd", ErrorMessage = "密码不一致")]
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]

        public string ConfirmPassword { get; set; }
        public DateTime AddTime
        {
            get;
            set;
        }
    }
}