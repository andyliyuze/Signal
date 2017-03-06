using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Autofac;
using MeassageCache;
using Model;
using DAL;
using System.Web.SessionState;
 
using System.Threading.Tasks;
using System.Diagnostics;
using DAL.Interface;
namespace SignalRChat
{
    public class ChatHub : Hub 
    {

        #region Data Members
          static List<UserDetail> ConnectedUsers = new List<UserDetail>();
          static List<PrivateMessage> CurrentPriMessage = new List<PrivateMessage>();
          static List<BroadcastMessage> CurrentBroMessage = new List<BroadcastMessage>();
          
         
          private readonly object syncboot = new object();
        private readonly ILifetimeScope _hubLifetimeScope;
        private readonly ICacheService _service;
        private readonly IMessageService _Msgservice;
        private readonly IUserDetail_DAL _DALservice;
         #endregion
        public ChatHub(ILifetimeScope lifetimeScope)
        {
            // Create a lifetime scope for the hub.
            _hubLifetimeScope = lifetimeScope.BeginLifetimeScope();

            // Resolve dependencies from the hub lifetime scope.
            _service = _hubLifetimeScope.Resolve<ICacheService>();
            _DALservice = _hubLifetimeScope.Resolve<IUserDetail_DAL>();
            _Msgservice = _hubLifetimeScope.Resolve<IMessageService>();
           
            
       

           
        }
        #region Methods



        public void Connect(string userName,string Pwd)
        {
            PrivateMessages_DAL _DAL = new PrivateMessages_DAL();
            List<PrivateMessage> list2 = new List<PrivateMessage>();
            PrivateMessage model2 = new PrivateMessage { PrivateMessageId = Guid.NewGuid(), SenderId = "aSASAS" ,CreateTime=DateTime.Now};
            list2.Add(model2);
            _DAL.AddList(list2);
            UserDetail user =null;
            var newcid = Context.ConnectionId;   
            string uid =_service.GetUserIdByName(userName);
            if (string.IsNullOrEmpty(uid) == false)
            {
                bool flag = _service.Login(uid,Pwd);
                //如果登录成功·
                if (flag == true)
                {  
                    //获取用户信息
                    user = _service.GetUserDetail(uid);
                    try
                    {
                        //接着尝试删除此用户的Cid，表示之前正在登录
                        string oldcid = _service.AfterLogin(uid,newcid);
                        //在前端迫使之前登录的用户下线
                        Clients.Client(oldcid).OutLogin();
                        //int UnReadCount
                    }
                    catch { }
                   
                }
                else { Clients.Caller.errorPwd(); return; }
            }
            else 
            {
                Clients.Caller.errorPwd(); return;
            }
        
           _service.UpdateUserCId(user.UserDetailId.ToString(),newcid);
               //获得与每位好友的历史消息，最新一条以及未读消息数量
            List<HistoryMsgViewModel> hisMsglist=  _Msgservice.GetHistoryMsg(uid);
             //获得好友列表
             List<UserDetail> list = _service.GetMyFriendsDetail(_service.GetFriendsIds(user.UserDetailId.ToString()));
             list.OrderBy(a=>a.IsOnline).ToList();

             // //更新在线人数
             //Clients.All.updateCount(list.Where(a=>a.IsOnline==true).Count());   
         
             
           //注册对List集合的监听，当到达一定数量就做处理
             string Onlinegruop = user.UserName + "的在线好友";
             foreach (var model in list.Where(a=>a.IsOnline==true).ToList()) {

                 Groups.Add(model.UserCId,Onlinegruop);
             }
                   
            //并行执行两个相互不影响的方法；
            //两个并行执行的方法才能用Parallel类
            Parallel.Invoke(
            //  send to caller当前用户
           () => Clients.Caller.onConnected(user, list, hisMsglist),
            // send to friends,通知所有在线好友
           ()=>   Clients.Group(Onlinegruop).onNewUserConnected(uid, userName)
              );
        
           
           
         Clients.CallerState.Uid = uid; //用它来代替Session
         
        
        }

        public void SendMessageToAll( string userName, string message)
        {
            int uid = Convert.ToInt32(Clients.CallerState.Uid);  //用它来代替Session
          
           

        }

        public void SendPrivateMessage(PrivateMessage model)
        {
            
            //发送者uId
            string uid = Clients.CallerState.Uid;  //用它来代替Session

            if (uid.Trim() != model.SenderId) { return; }
            //发送者Cid
            string fromUserId = Context.ConnectionId;
            //接受者uid
            string toUserCId = _service.GetUserCId(model.RecevierId);
            //定义消息实体
           
            model.CreateTime = DateTime.Now;
            model.PrivateMessageId = Guid.NewGuid();
           
           
            
           
           
            //接受者在线说明存在cid
            if (string.IsNullOrEmpty(uid) != true && string.IsNullOrEmpty(toUserCId) != true)
            {


                Clients.Client(toUserCId).receivePrivateMessage(model);
                
             
                 
                }
         
            // 加入到缓存
            _Msgservice.InsertPrivateMsg(model);
            
        }

        public void GetLastMessage() {



            BroadcastMessages_DAL dal = new BroadcastMessages_DAL();
          var list=  dal.GetLastMessage(10);
          Clients.Caller.AppendLastMessage(list);
        }
        public override System.Threading.Tasks.Task OnDisconnected( bool flag)
        {
            
            var cid = Context.ConnectionId;
            string uid = _service.GetUserIdByCId(cid);          
            if (string.IsNullOrEmpty(uid)==false)
            {
                //用户下线方法
                string name = _service.LogOut(uid);
                //获得在线好友列表
                List<UserDetail> list = _service.GetMyFriendsDetail(_service.GetFriendsIds(uid)).Where(a => a.IsOnline == true).ToList();
                
                string Onlinegruop = name + "的在线好友";
                foreach (var model in list)
                {

                    Groups.Add(model.UserCId, Onlinegruop);
                }
                //通知在线好友用户已下线
                Clients.Group(Onlinegruop).onUserDisconnected(uid, name);
            }
            
            
            return base.OnDisconnected( false);
        }

        //ChatingUserId代表正在聊天的用户Id
        public void OpenNewChatWindows(string ChatingUserId)
        {
            //打开者uId
            string uid = Clients.CallerState.Uid;
            _Msgservice.SetUnreadMsgCount(uid, ChatingUserId, 0);
        
        }

        public void GetUnreadMsg(string ChatingUserId, int count)
        { 
        
        string uid =Clients.CallerState.Uid;
        _Msgservice.SetUnreadMsgCount(uid, ChatingUserId, 0);
        List<PrivateMessage> list = _Msgservice.GetUnreadMsg(uid, ChatingUserId, count);
        list= list.OrderBy(a => a.CreateTime).ToList();
        Clients.Caller.messageListReceived(list);
        
        }
        #endregion
      


}




 
}