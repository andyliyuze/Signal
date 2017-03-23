using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Autofac;
using MeassageCache;
using Model;
using DAL;
using System.Threading.Tasks;
using DAL.Interface;
using Model.ViewModel;
using System.Diagnostics;
using SignalRChat.Extend;
using System.Web.Security;

namespace SignalRChat
{

    public class ChatHub : MyBaseHub    {

        #region Data Members
     
        private readonly ILifetimeScope _hubLifetimeScope;
        private readonly ICacheService _service;
        private readonly IMessageService _Msgservice;
        private readonly IUserDetail_DAL _DALservice;
        private readonly IFriendsApply_DAL _DALFriendsApplyservice;
        private readonly IGroup_DAL _IGroupDal;

        private   UserDetail CurrentUser;
         #endregion
        public ChatHub(ILifetimeScope lifetimeScope)
        {
            
            // Create a lifetime scope for the hub.
            _hubLifetimeScope = lifetimeScope.BeginLifetimeScope();

            // Resolve dependencies from the hub lifetime scope.
            _service = _hubLifetimeScope.Resolve<ICacheService>();
            _DALservice = _hubLifetimeScope.Resolve<IUserDetail_DAL>();
            _Msgservice = _hubLifetimeScope.Resolve<IMessageService>();

            _IGroupDal= _hubLifetimeScope.Resolve<IGroup_DAL>();
            _DALFriendsApplyservice = _hubLifetimeScope.Resolve<IFriendsApply_DAL>();
            CurrentUser = User.UserData;
        }
        
        #region Methods

      

    

        public void SendMessageToAll( string userName, string message)
        {
            int uid = Convert.ToInt32(CurrentUser.UserDetailId.ToString());  //用它来代替Session  
        }

        public void SendPrivateMessage(Message model)
        {
            model.RecevierId = model.ChattingId;
            model.type = "friend";      
            //接受者uid
            string toUserCId = _service.GetUserDetail(model.RecevierId).UserCId;
            //定义消息实体         
            model.CreateTime = DateTime.Now;
            model.MessageId = Guid.NewGuid();
            model.SenderId = CurrentUser.UserDetailId.ToString();
       
            //接受者在线说明存在cid
            if (string.IsNullOrEmpty(CurrentUser.UserDetailId.ToString()) != true && string.IsNullOrEmpty(toUserCId) != true)
            {
                Clients.Client(toUserCId).receivePrivateMessage(model);
            }        
            // 加入到缓存
           bool result=  _Msgservice.InsertPrivateMsg(model);
          
           if (result)
           {
               //告诉发送者发送成功
               Clients.Caller.sendMessageResult(SendMessageStatus.Success);
           }
           else {
               //告诉发送者发送失败
               Clients.Caller.sendMessageResult(SendMessageStatus.Failed);
           }
            
        }
        //确认收到消息的方法
        public void MessageConfirm(string ChatingUserId)
        
        {
            //让未读消息变为0
            _Msgservice.SetUnreadMsgCount(CurrentUser.UserDetailId.ToString(), ChatingUserId, 0);
        
        }
        public void GetLastMessage() {



          BroadcastMessages_DAL dal = new BroadcastMessages_DAL();
          var list=  dal.GetLastMessage(10);
          Clients.Caller.AppendLastMessage(list);
        }
 

        //获取未读消息数量
        public void GetUnreadMsg(string ChatingUserId, string MsgId, int count)
        {

                List<PrivateMessage> list = _Msgservice.GetUnreadMsg(CurrentUser.UserDetailId.ToString(), ChatingUserId, MsgId, count);
               list= list.OrderBy(a => a.CreateTime).ToList();
               Clients.Caller.messageListReceived(list);
            //让未读消息变为0
               _Msgservice.SetUnreadMsgCount(CurrentUser.UserDetailId.ToString(), ChatingUserId, 0);
        }

 


     
   

        private enum SendMessageStatus
        {
            Success = 0,        
            Failed = 1      
        }


        //添加组成员
        public Task Join(string groupName)
        {
            
            return Groups.Add(Context.ConnectionId, groupName);

        }
        //移除组成员，当用户下线时候
        public Task Leave(string groupName)
        {

            return Groups.Remove(Context.ConnectionId, groupName);

        }

        public async void  Leave(List<string> groupNames)
        {
            foreach (var groupName in groupNames)
            {
             await   Groups.Remove(Context.ConnectionId, groupName);
            }
          
        }
        public  void Join(List<Group> grouplist)
        {
            foreach (var group  in grouplist)
            {
                 Groups.Add(Context.ConnectionId, group.GroupName);
            }

        }



        [HubAuthorize]
        public void sendGroupMessage(Message model)
        {
            //组别Id就是接受者Id
            model.GroupId = model.ChattingId;
            model.type = "group";
           
            //接受者uid
            string GroupName = _IGroupDal.GetItemByGroupId(Guid.Parse(model.GroupId)).GroupName;
            //定义消息实体

            model.CreateTime = DateTime.Now;
            model.MessageId = Guid.NewGuid();
            model.SenderId = CurrentUser.UserDetailId.ToString();


            Clients.Group(GroupName,Context.ConnectionId).receiveGroupMessage(model);






        }


       
        #endregion


    }




 
}