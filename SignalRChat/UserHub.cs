using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Autofac;
using Model;
using DAL.Interface;
using Model.ViewModel;
using Common;
using SignalRChat.Extend;
using System.Linq;
using System.Threading.Tasks;
using MeassageCache.Interface;

namespace SignalRChat
{

    public class UserHub :MyBaseHub
    {

        #region Data Members




        private readonly ILifetimeScope _hubLifetimeScope;
        private readonly IFriendsService _Userservice;
        private readonly IFriendsApply_DAL _friendsApplyDal;
        private readonly IFriends_DAL _friendsDal;
        private readonly IMessageService _Msgservice;
        private readonly IUserService _service;
        private readonly IGroup_DAL _IGroupDal;
        private readonly IGroupMember_DAL _IGroupMemberDal;
        private readonly IJoinGroupApply_DAL _IJoinGroupApplyDal;
        #endregion
        public UserHub(ILifetimeScope lifetimeScope)
        {
            // Create a lifetime scope for the hub.
            _hubLifetimeScope = lifetimeScope.BeginLifetimeScope();
            // Resolve dependencies from the hub lifetime scope.
            _Userservice = _hubLifetimeScope.Resolve<IFriendsService>();
            _friendsApplyDal = _hubLifetimeScope.Resolve<IFriendsApply_DAL>();
            _friendsDal = _hubLifetimeScope.Resolve<IFriends_DAL>();
            _Msgservice = _hubLifetimeScope.Resolve<IMessageService>();
            _service = _hubLifetimeScope.Resolve<IUserService>();
            _IGroupDal = _hubLifetimeScope.Resolve<IGroup_DAL>();
            _IGroupMemberDal = _hubLifetimeScope.Resolve<IGroupMember_DAL>();
            _IJoinGroupApplyDal = _hubLifetimeScope.Resolve<IJoinGroupApply_DAL>();
        }
   
        
        
        #region Methods


        //双方成为好友操作,uidA代表是通过者，uidB表示是申请者
        public void BeFriend(string uidA, string uidB, string applyId)
        {
            //通过者model
            UserDetail respondserModel = _service.GetUserDetail(uidA);
            //申请者model
            UserDetail applicantModel = _service.GetUserDetail(uidB);
            //通知通过者
            Clients.Caller.beFriends(applicantModel);
            //接受者uid
            string toUserCId = _service.GetUserDetail(uidB).UserCId;
            //回复模型
            FriendsReplyViewModel ReplyViewModel = FriendsReplyViewModel.Create(respondserModel, ReplyStatus.Pass,applyId,false);
            //接受者在线说明存在cid
            if (!string.IsNullOrEmpty(toUserCId))
            {
                //通知申请人
                Clients.Client(toUserCId).receiveReplyResult(ReplyViewModel);
            }
            //redis操作
            _Userservice.BeFriends(uidA, uidB);
            //持久化操作
            Friends model = new Friends { ApplyUserId = Guid.Parse(uidA), FriendsId = Guid.NewGuid(), ReceiverUserId = Guid.Parse(uidB), BefriendTime = DateTime.Now };

            _friendsDal.BeFriends(model);
            //更新一下好友申请
            UpdateApplyResult("通过", applyId);
        }

        //好友或群搜索
        public void SearchUser(string name,string type)
        {
            if (type == "好友")
            {
                string id = _service.GetUserIdByName(name);
                UserDetail model = _service.GetUserDetail(id); 
                var json = new { result = model, type = "User" };
                Clients.Caller.searchResultReceived(json);
            }
            else if (type == "群")
            {
                GroupViewModel model = _IGroupDal.GetGroupDeatailByGroupName(name);
                var json = new { result = model, type = "Group" };
                Clients.Caller.searchResultReceived(json);
            }
        }

        //好友申请
        //申请人uidA，接收人uidB
        public void SendAddFriendsReply(string uidA, string uidB)
        {
            if (!CheckIsvalid(uidA) || CheckIsFriend(uidA, uidB) || CheckIsSeft(uidA,uidB) ){ return ; };
            try
            {
                //将添加请求持久化到sqlserver
                var ApplyId = Guid.NewGuid();
                FriendsApply model = new FriendsApply {FriendsApplyId=ApplyId, ApplyUserId = Guid.Parse(uidA), ReceiverUserId = Guid.Parse(uidB), ReplyTime = DateTime.Now, Result = "待审" };
           
                _friendsApplyDal.SendAddFriendsApply(model);
                Clients.Caller.applyResult(ApplyStatus.Success);
                //尝试通知接受者
                TryTellReceiverForFriendsApply(uidA, uidB, ApplyId);
            }
            catch 
            {
                Clients.Caller.applyResult(ApplyStatus.Failed);
            }
        }
        private void TryTellReceiverForFriendsApply(string uidA, string uidB,Guid ApplyId)
        {
            //接受者uid
            string toUserCId = _service.GetUserDetail(uidB).UserCId;
            if (!string.IsNullOrEmpty(toUserCId))
            {
                //发送人信息
                var user = _service.GetUserDetail(uidA);
                FriendsApplyViewModel viewmodel = FriendsApplyViewModel.Create(user, ApplyId);
              
                Clients.Client(toUserCId).recevieFriendApply(viewmodel);
            }

        }
        //检查是否合法用户
        private bool CheckIsvalid(string uid)
        {
            if (uid != User.UserData.UserDetailId.ToString()) { Clients.Caller.applyResult(ApplyStatus.UnAuthorize); return false; }
            else
            {
                return true;
            }
            #endregion
        }
        [HubAuthorize]

        //建立连接，业务上是用户正式上线操作
        public override Task OnConnected()
        {

            UserDetail CurrentUser = User.UserData;
            //当有新的用户上线
            OnNewUserContented();
            //获得与每位好友的历史消息，最新一条以及未读消息数量
            List<HistoryMsgViewModel> hisMsglist = _Msgservice.GetHistoryMsg(CurrentUser.UserDetailId.ToString());
            //获得好友列表
            List<UserDetail> friendlist = _service.GetMyFriends(CurrentUser.UserDetailId.ToString());

            friendlist.OrderBy(a => a.IsOnline).ToList();
            string Onlinegruop = CurrentUser.UserName + "的在线好友";

            //获取群列表
            List<Group> grouplist = _IGroupDal.GetMyGroups(Guid.Parse(CurrentUser.UserDetailId.ToString()));

            foreach (var model in friendlist.Where(a => a.IsOnline == true).ToList())
            {
               
                Groups.Add(model.UserCId, Onlinegruop);
            }

            //并行执行两个相互不影响的方法；
            //两个并行执行的方法才能用Parallel类
            Parallel.Invoke(
            //  send to caller当前用户
             () => Clients.Caller.onConnected(CurrentUser, friendlist, grouplist, hisMsglist),
            // send to friends,通知所有在线好友
             () => Clients.Group(Onlinegruop).onNewUserConnected(CurrentUser.UserDetailId.ToString(), CurrentUser.UserName),
             () => Join(grouplist)
              );
            return base.OnConnected();
        }
        public void Join(List<Group> grouplist)
        {
            IHubContext Chat = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            foreach (var group in grouplist)
            {
                
                Chat.Groups.Add(Context.ConnectionId, group.GroupName);
            }

        }
        public void Leave(List<string> groupNames)
        {
            foreach (var groupName in groupNames)
            {
                Groups.Remove(Context.ConnectionId, groupName);
            }
        }
        private void OnNewUserContented()
        {

            string uid = User.UserData.UserDetailId.ToString();
            string newcid = Context.ConnectionId;
            //获取此前的cid
            string oldcid = _service.GetUserDetail(uid).UserCId;

            if (!string.IsNullOrEmpty(oldcid) && newcid != oldcid)
            {
                //在前端迫使之前登录的用户下线
                Clients.Client(oldcid).OutLogin();
            }
            //将新的cid更新到redis
            _service.UpdateUserCId(uid, newcid);
            _service.UpdateUserField("IsOnline", "true", uid);
        }
        [HubAuthorize]
        //重新连接，业务上表示重新上线，更新用户Cid到redis
        public override Task OnReconnected()
        {
        
         
            string uid = User.UserData.UserDetailId.ToString();
            string newcid = Context.ConnectionId;
            _service.UpdateUserCId(uid, newcid);
            _service.UpdateUserField("IsOnline", "true", uid);
            //获取群列表
            List<Group> grouplist = _IGroupDal.GetMyGroups(Guid.Parse(uid));
            Join(grouplist);
            return base.OnReconnected();
        }
        [HubAuthorize]

        //用户下线
        public override Task OnDisconnected(bool flag)
        {
            if (User == null) return base.OnDisconnected(false);
            string uid = User.UserData.UserDetailId.ToString();
            //用户下线方法
            string name = User.UserData.UserName;
             _service.LogOut(uid);
            //获得在线好友列表
            List<UserDetail> list = _service.GetMyFriends(uid).Where(a => a.IsOnline == true).ToList();

            string Onlinegruop = name + "的在线好友";
            foreach (var model in list)
            {
                Groups.Remove(model.UserCId, Onlinegruop);
            }
            //通知在线好友用户已下线
            Clients.Group(Onlinegruop).onUserDisconnected(uid, name);
            return base.OnDisconnected(false);
        }
     

        //检查是否已经是好友

        private bool CheckIsFriend(string uidA, string uidB)
        {

            if (_friendsDal.IsFriend(uidA, uidB)) { Clients.Caller.applyResult(ApplyStatus.Friended); return true; }
            else { return false; }
        }
     
        //检查是否为自己
        private bool CheckIsSeft(string uidA, string uidB)
        {

            if (uidA.Trim() == uidB.Trim()) { Clients.Caller.applyResult(ApplyStatus.YourSelf); return true; }
            else { return false; }
        }
      
        //确定查看到回复
        public void FriendReplyHaveRead(List<string> appIds)
        {

            var app = new List<string>();
            _friendsApplyDal.SetReadByIds(appIds);
        
        
        }

        public void GroupReplyHaveRead(List<string> appIds)
        {

            var app = new List<string>();
             _IJoinGroupApplyDal.SetReadByIds(appIds);


        }
        
        //申请好友结果的状态枚举
        private enum ApplyStatus
         {
            Success = 0,
            UnAuthorize=1,
            Failed=2,
            Friended=3,
            YourSelf=4,
            BeenMember=5
       }

        //拒绝操作
        public void  RefuseApply(string uidA,string applyUserId, string applyId)
        {
            //通过者model
            UserDetail respondserModel = _service.GetUserDetail(uidA);
            //回复模型
            FriendsReplyViewModel ReplyViewModel = FriendsReplyViewModel.Create(respondserModel, ReplyStatus.Decline, applyId, false);
            //检查申请人是否在线
            string Cid = _service.GetUserDetail(applyUserId).UserCId;
            if (!string.IsNullOrEmpty(Cid))
            {
                //尝试通知申请人
                Clients.Client(Cid).receiveReplyResult(ReplyViewModel);
            }
            // 更新一下数据库，申请表
            string result = "拒绝";
            UpdateApplyResult(result, applyId);
           
        }

        //好友申请结果更新
        private void UpdateApplyResult(string result, string applyId) 
        {


            //更新一下好友申请
            FriendsApply apply = new FriendsApply { FriendsApplyId = Guid.Parse(applyId), ReceiverUserId = Guid.Parse(User.UserData.UserDetailId.ToString()), ReplyTime = DateTime.Now, Result = result, HasReadResult = "未读" };
             _friendsApplyDal.UpdateResult(apply);
        }


        //发送添加群操作，groupId表示群id
        public void SendAdGroupReply(string uidA,string groupId)
        {
            if (!CheckIsvalid(uidA) || CheckIsInGruop(uidA, groupId)) { return; };
            try
            {
                //将添加请求持久化到sqlserver
                var ApplyId = Guid.NewGuid();
                JoinGroupApply model = new JoinGroupApply
                {
                    ApplyTime = DateTime.Now,
                    ApplyUserId = Guid.Parse(uidA),
                    GroupId = Guid.Parse(groupId),
                    HasReadResult = "待回复",
                    Id= ApplyId,
                    ReplyTime=DateTime.Now,
                    Result="待审"
                };
                _IJoinGroupApplyDal.Add(model);
            
                Clients.Caller.applyResult(ApplyStatus.Success);
                //尝试通知接受者
                TryTellReceiverForGroupApply(uidA, groupId, ApplyId);
            }
            catch
            {
                Clients.Caller.applyResult(ApplyStatus.Failed);
            }
        }


        //检查是否添加人已经在群里
        private bool CheckIsInGruop(string uidA,string groupId)
        {
            if (_IGroupMemberDal.GetItemByMemberId(Guid.Parse(uidA), Guid.Parse(groupId)) != null)
            {
                Clients.Caller.applyResult(ApplyStatus.BeenMember);
                return true;

            }
            else {
                return false; }
        }

        private void TryTellReceiverForGroupApply(string uidA, string uidB, Guid ApplyId)
        {
            //找出组别信息
            var group = _IGroupDal.GetItemByGroupId(Guid.Parse(uidB));
            //uidB为群Id，需要先找出群住Id
            string ownerId= group.OwnerId.ToString();
            //接受者uid
            string toUserCId = _service.GetUserDetail(ownerId).UserCId;
            if (!string.IsNullOrEmpty(toUserCId))
            {
                //发送人信息
                var user = _service.GetUserDetail(uidA);             
                GroupApplyViewModel viewmodel = GroupApplyViewModel.Create(user, ApplyId, group);       
                Clients.Client(toUserCId).recevieGroupApply(viewmodel);
            }
        }

        //同意入群操作,uidA代表是组别Id，uidB表示是申请者
        public void BeGroupMember(string uidA, string uidB, string applyId)
        {
            //拿到组别Id
            Guid GroupId = _IJoinGroupApplyDal.GetItemById(Guid.Parse(applyId)).GroupId;

            //拿到组别实体
            Group group = _IGroupDal.GetItemByGroupId(GroupId);

            //申请者model
            UserDetail applicantModel = _service.GetUserDetail(uidB);
            //通知通过者
            Clients.Caller.beGroupMember(applicantModel);
            //接受者uid
            string toUserCId = _service.GetUserDetail(uidB).UserCId;
            //回复模型
            GroupReplyViewModel ReplyViewModel = GroupReplyViewModel.Create(group, ReplyStatus.Pass, applyId, false);
            //接受者在线说明存在cid
            if (!string.IsNullOrEmpty(toUserCId))
            {
                //通知申请人
                Clients.Client(toUserCId).receiveGroupReplyResult(ReplyViewModel);
                //signalR保存一下  
                List<Group> GroupList = new List<Group>();
                GroupList.Add(group);
                IHubContext Chat = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                Chat.Groups.Add(toUserCId, group.GroupName);
            }

            //持久化操作
            GroupMember model = new GroupMember { ApproverId=Guid.Parse(uidA),GroupId=GroupId,Id=Guid.NewGuid(),MemberId=Guid.Parse(uidB) };
            //成员表添加数据
            _IGroupMemberDal.Add(model);
            //更新一下好友申请表
            UpdateGroupApplyResult("通过", applyId);
        }

        //更新入群申请操作
        private void UpdateGroupApplyResult(string result, string applyId)
        {
            Guid GroupApplyId = Guid.Parse(applyId);
            //更新一下好友申请
            JoinGroupApply apply = new JoinGroupApply {Id = GroupApplyId, ReplyTime = DateTime.Now, Result = result, HasReadResult = "未读" };
            _IJoinGroupApplyDal.UpdateResult(apply);
        }


        //拒绝操作
        public void RefuseGroupApply(string uidA, string applyUserId, string applyId)
        {
            //拿到组别Id
            Guid GroupId = _IJoinGroupApplyDal.GetItemById(Guid.Parse(applyId)).GroupId;

            //拿到组别实体
            Group group = _IGroupDal.GetItemByGroupId(GroupId);
             
            //回复模型
            GroupReplyViewModel ReplyViewModel = GroupReplyViewModel.Create(group, ReplyStatus.Decline, applyId, false);
            //检查申请人是否在线
            string Cid = _service.GetUserDetail(applyUserId).UserCId;
            if (!string.IsNullOrEmpty(Cid))
            {

                //尝试通知申请人
                Clients.Client(Cid).receiveGroupReplyResult(ReplyViewModel);
            }
            // 更新一下数据库，申请表
            string result = "拒绝";
            UpdateGroupApplyResult(result, applyId);

        }

        //获取未读群添加回复
        public void GetUnreadGroupReply() {
            string uid = User.UserData.UserDetailId.ToString();
            var list = _IJoinGroupApplyDal.GetGroupReplyByUId(Guid.Parse(uid));

            Clients.Caller.receiveGroupReplyList(list);
        }
        //获取未审核群添加申请
        public void GettUnapproveGroupApply() {
            string uid = User.UserData.UserDetailId.ToString();
            var list = _IJoinGroupApplyDal.GetGroupApplyByUId(Guid.Parse(uid));
            Clients.Caller.receiveGroupApplyList(list);
        }
        //获取未读添好友加回复
        public void GetUnreadFriendsReply()
        {
            string uid = User.UserData.UserDetailId.ToString();
            var list = _friendsApplyDal.GetFriendsReplyByUId(Guid.Parse(uid));
            Clients.Caller.receiveFriendReplyList(list);

        }
        //获取未审核好友添加申请
        public void GetUnapproveFriendsApply()
        {

            string uid = User.UserData.UserDetailId.ToString();
            var list = _friendsApplyDal.GetFriendsApplyByUId(Guid.Parse(uid));
            Clients.Caller.receiveFriendApplyList(list);
        }
    }
}