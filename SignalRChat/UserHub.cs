using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Autofac;
using MeassageCache;
using Model;
using DAL.Interface;
using Model.ViewModel;
using Newtonsoft.Json.Linq;

namespace SignalRChat
{

    public class UserHub : Hub
    {

        #region Data Members




        private readonly ILifetimeScope _hubLifetimeScope;
        private readonly IUserCacheService _Userservice;
        private readonly IFriendsApply_DAL _friendsApplyDal;
        private readonly IFriends_DAL _friendsDal;
        private readonly ICacheService _service;
        private readonly IGroup_DAL _IGroupDal;
        private readonly IGroupMember_DAL _IGroupMemberDal;
        private readonly IJoinGroupApply_DAL _IJoinGroupApplyDal;
        #endregion
        public UserHub(ILifetimeScope lifetimeScope)
        {
            // Create a lifetime scope for the hub.
            _hubLifetimeScope = lifetimeScope.BeginLifetimeScope();

            // Resolve dependencies from the hub lifetime scope.
            _Userservice = _hubLifetimeScope.Resolve<IUserCacheService>();
            _friendsApplyDal = _hubLifetimeScope.Resolve<IFriendsApply_DAL>();
            _friendsDal = _hubLifetimeScope.Resolve<IFriends_DAL>();

            _service = _hubLifetimeScope.Resolve<ICacheService>();

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
            string toUserCId = _service.GetUserCId(uidB);
            //回复模型
            FriendsReplyViewModel ReplyViewModel = FriendsReplyViewModel.Create(respondserModel, ReplyStatus.Pass,applyId,false);
            //接受者在线说明存在cid
            if (!string.IsNullOrEmpty(toUserCId))
            {
                //通知申请人
                Clients.Client(toUserCId).receiveReplyResult(ReplyViewModel);
            }
            //自己的客户端也要更新一下用户好友表
            Clients.Caller.onNewUserConnected(applicantModel);

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
            string toUserCId = _service.GetUserCId(uidB);
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
            if (uid != Clients.CallerState.Uid) { Clients.Caller.applyResult(ApplyStatus.UnAuthorize); return false; }
            else
            {
                return true;
            }
            #endregion
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
            YourSelf=4
       }

        //拒绝操作
        public void  RefuseApply(string uidA,string applyUserId, string applyId)
        {

            //通过者model
            UserDetail respondserModel = _service.GetUserDetail(uidA);
            //回复模型
            FriendsReplyViewModel ReplyViewModel = FriendsReplyViewModel.Create(respondserModel, ReplyStatus.Decline, applyId, false);
            //检查申请人是否在线
            string Cid = _service.GetUserCId(applyUserId);
            if (!string.IsNullOrEmpty(Cid))
            {

                //尝试通知申请人
                Clients.Client(Cid).receiveReplyResult(ReplyViewModel);
            }
            // 更新一下数据库，申请表
            string result = "拒绝";
            UpdateApplyResult(result, applyId);
           
        }


        private void UpdateApplyResult(string result, string applyId) 
        {


            //更新一下好友申请
            FriendsApply apply = new FriendsApply { FriendsApplyId = Guid.Parse(applyId), ReceiverUserId = Guid.Parse(Clients.CallerState.Uid), ReplyTime = DateTime.Now, Result = result, HasReadResult = "未读" };
             _friendsApplyDal.UpdateResult(apply);
        }


        //发送添加群操作
        public void SendAdGroupReply(string uidA,string uidB)
        {
            if (!CheckIsvalid(uidA) || CheckIsInGruop(uidA, uidB)) { return; };
            try
            {
                //将添加请求持久化到sqlserver
                var ApplyId = Guid.NewGuid();
                JoinGroupApply model = new JoinGroupApply
                {
                    ApplyTime = DateTime.Now,
                    ApplyUserId = Guid.Parse(uidA),
                    GroupId = Guid.Parse(uidB),
                    HasReadResult = "待回复",
                    Id= ApplyId,
                    ReplyTime=DateTime.Now,
                    Result="待审"
                };
                _IJoinGroupApplyDal.Add(model);
            
                Clients.Caller.applyResult(ApplyStatus.Success);
                //尝试通知接受者
                TryTellReceiverForGroupApply(uidA, uidB, ApplyId);
            }
            catch
            {
                Clients.Caller.applyResult(ApplyStatus.Failed);
            }
        }


        //检查是否添加人已经在群里
        private bool CheckIsInGruop(string uidA,string  uidB)
        {
            if (_IGroupMemberDal.GetItemByMemberId(Guid.Parse(uidA)) != null)
            {
                return true;

            }
            else { return false; }
        }

        private void TryTellReceiverForGroupApply(string uidA, string uidB, Guid ApplyId)
        {
            //找出组别信息
            var group = _IGroupDal.GetItemByGroupId(Guid.Parse(uidB));
            //uidB为群Id，需要先找出群住Id
            string ownerId= group.OwnerId.ToString();
            //接受者uid
            string toUserCId = _service.GetUserCId(ownerId);
            if (!string.IsNullOrEmpty(toUserCId))
            {
                //发送人信息
                var user = _service.GetUserDetail(uidA);             
                GruopApplyViewModel viewmodel = GruopApplyViewModel.Create(user, ApplyId, group);       
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
            string toUserCId = _service.GetUserCId(uidB);
            //回复模型
           GroupReplyViewModel ReplyViewModel = GroupReplyViewModel.Create(group, ReplyStatus.Pass, applyId, false);
            //接受者在线说明存在cid
            if (!string.IsNullOrEmpty(toUserCId))
            {
                //通知申请人
                Clients.Client(toUserCId).receiveGroupReplyResult(ReplyViewModel);
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
    }

 
}