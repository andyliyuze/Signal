﻿using System;
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
     
        UserDetail CurrentUser = new UserDetail();
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
          
 
           
        }
        #region Methods

        [HubAuthorize]
     
        public  void Connect(string userName,string Pwd)
        {

            UserDetail u = User.UserData;

            //表示新尝试登陆的用户的连接Id
            var newcid = Context.ConnectionId;            
            if (!CheckUserInRedis(userName) || !TryLogin(userName, Pwd)) { return; }
            GetUserDetail();
            TryMakeLogonUserOffline();
            SaveUserInfoStatus();
              
             _service.UpdateUserCId(Clients.CallerState.Uid, newcid);
               //获得与每位好友的历史消息，最新一条以及未读消息数量
             List<HistoryMsgViewModel> hisMsglist=  _Msgservice.GetHistoryMsg(Clients.CallerState.Uid);
             //获得好友列表
             List<UserDetail> friendlist = _service.GetMyFriendsDetail(_service.GetFriendsIds(Clients.CallerState.Uid));

             friendlist.OrderBy(a=>a.IsOnline).ToList();
             string Onlinegruop = CurrentUser.UserName + "的在线好友";

            //获取群列表
            List<Group> grouplist = _IGroupDal.GetMyGroups(Guid.Parse(Clients.CallerState.Uid));
         
            foreach (var model in friendlist.Where(a=>a.IsOnline==true).ToList())
            {
                 Groups.Add(model.UserCId,Onlinegruop);
             }
        
            //并行执行两个相互不影响的方法；
            //两个并行执行的方法才能用Parallel类
            Parallel.Invoke(
            //  send to caller当前用户
             () => Clients.Caller.onConnected(CurrentUser, friendlist, grouplist, hisMsglist),
            // send to friends,通知所有在线好友
             ()=>   Clients.Group(Onlinegruop).onNewUserConnected(Clients.CallerState.Uid, userName),
             () =>Join(grouplist)
              );



 
        }

    

        public void SendMessageToAll( string userName, string message)
        {
            int uid = Convert.ToInt32(Clients.CallerState.Uid);  //用它来代替Session  
        }

     
        public void SendPrivateMessage(Message model)
        {
        

            model.RecevierId = model.ChattingId;
            model.type = "friend";
            //发送者Cid
            string fromUserId = Context.ConnectionId;
            //接受者uid
            string toUserCId = _service.GetUserCId(model.RecevierId);
            //定义消息实体
           
            model.CreateTime = DateTime.Now;
            model.MessageId = Guid.NewGuid();
            model.SenderId = Clients.CallerState.Uid;
           
            
           
           
            //接受者在线说明存在cid
            if (string.IsNullOrEmpty(Clients.CallerState.Uid) != true && string.IsNullOrEmpty(toUserCId) != true)
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
            _Msgservice.SetUnreadMsgCount(Clients.CallerState.Uid, ChatingUserId, 0);
        
        }
        public void GetLastMessage() {



          BroadcastMessages_DAL dal = new BroadcastMessages_DAL();
          var list=  dal.GetLastMessage(10);
          Clients.Caller.AppendLastMessage(list);
        }
        public override System.Threading.Tasks.Task OnDisconnected(bool flag)
        {


            if (string.IsNullOrEmpty(Clients.CallerState.Uid) == false)
            {
                //用户下线方法
                string name = _service.LogOut(Clients.CallerState.Uid);
                //获得在线好友列表
                string uid = Clients.CallerState.Uid;
                List<UserDetail> list = _service.GetMyFriendsDetail(_service.GetFriendsIds(uid)).Where(a => a.IsOnline == true).ToList();
                
                string Onlinegruop = name + "的在线好友";
                foreach (var model in list)
                {

                    Groups.Add(model.UserCId, Onlinegruop);
                }
                //通知在线好友用户已下线
                Clients.Group(Onlinegruop).onUserDisconnected(Clients.CallerState.Uid, name);
            }
            
            
            return base.OnDisconnected( false);
        }


        //获取未读消息数量
        public void GetUnreadMsg(string ChatingUserId, string MsgId, int count)
        {

                List<PrivateMessage> list = _Msgservice.GetUnreadMsg(Clients.CallerState.Uid, ChatingUserId, MsgId, count);
               list= list.OrderBy(a => a.CreateTime).ToList();
               Clients.Caller.messageListReceived(list);
            //让未读消息变为0
               _Msgservice.SetUnreadMsgCount(Clients.CallerState.Uid, ChatingUserId, 0);
        }


        //检查用户名是否存在，并且保存UserId字段的值
        private bool CheckUserInRedis(string userName)
        {

           
           Clients.CallerState.Uid = _service.GetUserIdByName(userName);
            if (string.IsNullOrEmpty(Clients.CallerState.Uid))
            {

                Clients.Caller.loginResult(LoginStatus.UserUnExist);
                return false;
            }
            else { return true; }
        }
        private bool TryLogin(string userName, string Pwd)
        {
            bool flag = _service.Login(Clients.CallerState.Uid, Pwd);
            if (flag == true)
            {
                Clients.Caller.loginResult(LoginStatus.Success);
                return true;
            }
            else
            {
                Clients.Caller.loginResult(LoginStatus.Failed);
                return false;
            }
          
        }

        private void TryMakeLogonUserOffline()
        {



            //接着尝试删除此用户的Cid，表示之前正在登录
            string oldcid = _service.AfterLogin(Clients.CallerState.Uid, Context.ConnectionId);
            if (!string.IsNullOrEmpty(oldcid)) 
            {
                //在前端迫使之前登录的用户下线
                Clients.Client(oldcid).OutLogin();
            }
           



        }

        private void UpUateCIdToRedis()
        {
            _service.UpdateUserCId(Clients.CallerState.Uid, Context.ConnectionId);
        }
        //获取CurrentUser字段值
        private void GetUserDetail()
        {
            try
            {
                CurrentUser= _service.GetUserDetail(Clients.CallerState.Uid);
                MyFormsPrincipal<UserDetail>.SignIn(CurrentUser.UserName, CurrentUser, 60);


            }
            catch
            {
              
            }
         
        }


        //保存一下用户状态
        private void SaveUserInfoStatus() {

            Clients.CallerState.User = CurrentUser;
            Clients.CallerState.Uid = CurrentUser.UserDetailId.ToString();
              
        }
        private enum LoginStatus
        {
            Success = 0,
            UserUnExist = 1,
            Failed = 2
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
            //发送者Cid
            string fromUserId = Context.ConnectionId;
            //接受者uid
            string GroupName = _IGroupDal.GetItemByGroupId(Guid.Parse(model.GroupId)).GroupName;
            //定义消息实体

            model.CreateTime = DateTime.Now;
            model.MessageId = Guid.NewGuid();
            model.SenderId = Clients.CallerState.Uid;


            Clients.Group(GroupName,Context.ConnectionId).receiveGroupMessage(model);






        }


        public override Task OnReconnected()
        {
           
            string newcid = Context.ConnectionId;
            _service.UpdateUserCId(Clients.CallerState.Uid, newcid);
            return base.OnReconnected();
        }
        #endregion


    }




 
}