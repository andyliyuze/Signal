﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Autofac;
using Model;
using DAL;
using DAL.Interface;
using SignalRChat.Extend;
using MeassageCache.Interface;

namespace SignalRChat
{

    public class ChatHub : MyBaseHub
    {
        #region Data Members
        private readonly ILifetimeScope _hubLifetimeScope;
        private readonly IUserService _service;
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
            _service = _hubLifetimeScope.Resolve<IUserService>();
            _DALservice = _hubLifetimeScope.Resolve<IUserDetail_DAL>();
            _Msgservice = _hubLifetimeScope.Resolve<IMessageService>();
            _IGroupDal= _hubLifetimeScope.Resolve<IGroup_DAL>();
            _DALFriendsApplyservice = _hubLifetimeScope.Resolve<IFriendsApply_DAL>();
            try
            {
                CurrentUser = User.UserData;
            }
            catch { }
        }     
        #region Methods
    
        public void SendPrivateMessage(PrivateMessage model)
        
        {
            var User= Context.User;
            model.RecevierId = model.ChattingId;
            model.type = "single";      
            //接受者uid
            string toUserCId = _service.GetUserDetail(model.RecevierId).UserCId;
            //定义消息实体         
            model.CreateTime = DateTime.Now;
            model.MessageId = Guid.NewGuid();
            model.SenderId = CurrentUser.UserDetailId.ToString();
          
            //接受者在线说明存在cid
            if (string.IsNullOrEmpty(CurrentUser.UserDetailId.ToString()) != true && !string.IsNullOrEmpty(toUserCId))
            {
                Clients.Client(toUserCId).receivePrivateMessage(model);
            }        
            // 加入到缓存
            bool result=  _Msgservice.InsertPrivateMsg(model);
            if (result)
           {
               //告诉发送者发送成功
               Clients.Caller.sendMessageResult(SendMessageStatus.Success,model.MessageIdUserForJs, model.RecevierId);
           }
           else {
               //告诉发送者发送失败
               Clients.Caller.sendMessageResult(SendMessageStatus.Failed, model.MessageIdUserForJs, model.RecevierId);
           }           
        }
        //确认收到消息的方法
        public void MessageConfirm(string ChatingUserId)      
        {
            //让未读消息变为0
            _Msgservice.SetUnreadPrivateMsgCount(CurrentUser.UserDetailId.ToString(), ChatingUserId, 0);   
        }
        public void GetLastMessage()
        {
          BroadcastMessages_DAL dal = new BroadcastMessages_DAL();
          var list=  dal.GetLastMessage(10);
          Clients.Caller.AppendLastMessage(list);
        }
        //获取未读消息数量
        public void GetUnreadMsg(string ChatingUserId, string MsgId, int count)
        {
                List<PrivateMessage> list = _Msgservice.GetPrivateUnreadMsg(CurrentUser.UserDetailId.ToString(), ChatingUserId, MsgId, count);
               list= list.OrderBy(a => a.CreateTime).ToList();
               Clients.Caller.unreadMessageListReceived(list);
            //让未读消息变为0
               _Msgservice.SetUnreadPrivateMsgCount(CurrentUser.UserDetailId.ToString(), ChatingUserId, 0);
        }

        //获取历史消息
        public void GetHistoryMsg(string ChatingUserId, string MsgId)
        {
            List<PrivateMessage> list = _Msgservice.GetHisToryMsg<PrivateMessage>(CurrentUser.UserDetailId.ToString(), ChatingUserId, MsgId, 10);
            list = list.OrderByDescending(a => a.CreateTime).ToList();
            Clients.Caller.messageListReceived(list);
        }
       
        private enum SendMessageStatus
        {
            Success = 0,        
            Failed = 1      
        }
       
       [HubAuthorize]
        public void sendGroupMessage(BroadcastMessage model)
        {
            //组别Id就是接受者Id
            model.GroupId = model.ChattingId;
            model.type = "group";
            //接受者uid
            string GroupName = _IGroupDal.GetItemByGroupId(Guid.Parse(model.GroupId)).GroupName.Trim(); ;
            //定义消息实体
            model.CreateTime = DateTime.Now;
            model.MessageId = Guid.NewGuid();
            model.SenderId = CurrentUser.UserDetailId.ToString();
            
            Clients.Group(GroupName,Context.ConnectionId).receiveGroupMessage(model);
            // 加入到缓存
            bool result = _Msgservice.InsertBroadcastMsg(model);
            if (result)
            {
                //告诉发送者发送成功
                Clients.Caller.sendMessageResult(SendMessageStatus.Success,model.MessageIdUserForJs, model.GroupId);
            }
            else
            {
                //告诉发送者发送失败
                Clients.Caller.sendMessageResult(SendMessageStatus.Failed,model.MessageIdUserForJs, model.GroupId);
            }
        }       
        #endregion
    }
}