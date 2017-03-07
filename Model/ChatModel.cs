using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Serializable]
        public class BroadcastMessage
        {

            [Key, Column(TypeName = "uniqueidentifier")]
            public Guid BroadcastMessageId { get; set; }
            [MaxLength(500, ErrorMessage = "消息长度不能超过500字")]
            public string Message { get; set; }
            [MaxLength(10, ErrorMessage = "用户名不能超过10字符")]
            public string UserName { get; set; }
            //public int SenderId { get; set; }

        }
 [Serializable]  //必须添加序列化特性
        public class PrivateMessage
        {
      
            [Key]
            public Guid PrivateMessageId { get; set; }
            public string SenderId { get; set; }
            [MaxLength(500, ErrorMessage = "消息长度不能超过500字")]
            public string content { get; set; }
            [MaxLength(10, ErrorMessage = "用户名不能超过10字符")]
            public string SenderName { get; set; }
            public string RecevierId { get; set; }
            public DateTime CreateTime { get; set; }

        }
      [Serializable]  //必须添加序列化特性
        public class UserDetail
        {
            [Key, Column(TypeName = "uniqueidentifier")]
            //[Key]
          
            public Guid UserDetailId { get; set; }
            [NotMapped]
            public string UserCId { get; set; }
           
            [MaxLength(10, ErrorMessage = "用户名不能超过10字符")]
            public string UserName { get; set; }
 
            public bool IsOnline { get; set; }

            public string AvatarPic { get; set; }
        }

 [Serializable]  //必须添加序列化特性
        public class UserInfo
        {
            [Key, Column(TypeName = "uniqueidentifier")]



            public Guid UserId { get; set; }
            public string UserName { get; set; }
            [MaxLength(12, ErrorMessage = "密码长度不能超过12字符"), MinLength(3, ErrorMessage = "密码长度不能超过12字符")]
            public string Pwd { get; set; }
            public DateTime AddTime
            {
                get;
                set;
            }
        }
 [Serializable]  //必须添加序列化特性

 public class FriendsApply {

     [Key, Column(TypeName = "uniqueidentifier")]
     public Guid FriendsApplyId { get; set; }
     [Column(TypeName = "uniqueidentifier")]
     public Guid ApplyUserId { get; set; }

     [Column(TypeName = "uniqueidentifier")]
     public Guid ReceiverUserId { get; set; }
     //申请时间
     public DateTime ApplyTime { get; set; }

    //回复时间
      [DefaultValue(typeof(DateTime) ,"1970-01-01"),Required(AllowEmptyStrings=true)]
     public DateTime ReplyTime { get; set; }
     //申请结果
     [Column(TypeName = "nvarchar"),MaxLength(10)]
     public string Result { get; set; }
    [Column(TypeName = "nvarchar"),  MaxLength(20),DefaultValue("待回复")]

     public string HasReadResult { get; set; }
 
 }

       [Serializable] 
         public class Friends {
         [Key, Column(TypeName = "uniqueidentifier")]
         public Guid FriendsId
         {
             get;
             set;
         }

         public Guid ApplyUserId { get;set;}
         public Guid ReceiverUserId { get; set; }

         public DateTime BefriendTime { get; set; }
 }


    [Serializable]
    public class Group
    {
        public Guid GroupId { get; set; }

        public Guid OwnerId { get; set; }

        public string GroupName { get; set; }

        public string GroupAvatar { get; set; }

     
    }
    [Serializable]
    public class GroupMember
    {

        public Guid Id { get; set; }
         
        public string GroupId { get; set; }

        public string MemberId { get; set; }

        public Guid ApproverId { get; set; }

    }
    [Serializable]
    public class JoinGroupApply
    {

        [Key, Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ApplyUserId { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid GroupIdId { get; set; }
       
        //申请时间
        public DateTime ApplyTime { get; set; }

        //回复时间
        [DefaultValue(typeof(DateTime), "1970-01-01"), Required(AllowEmptyStrings = true)]
        public DateTime ReplyTime { get; set; }
        
        //申请结果
        [Column(TypeName = "nvarchar"), MaxLength(10)]
        public string Result { get; set; }

        [Column(TypeName = "nvarchar"), MaxLength(20), DefaultValue("待回复")]
        public string HasReadResult { get; set; }

    }
}

