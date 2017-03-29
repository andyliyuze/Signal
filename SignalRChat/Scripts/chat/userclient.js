function registerClientMethodsForUser(userHub) {


    // Calls when user successfully logged in
    userHub.client.onConnected = function (user, allUsers, grouplist, hisMsglist) {

        setScreen(true);
     

        $('#hdUserName').attr("data-uid", user.UserDetailId);
        $('#hdUserName').text(user.UserName);
        $("#myheadsrc").attr("data-uid", user.Id);
        $("#myheadsrc").attr("src", user.AvatarPic);
        // Add All Users
        for (i = 0; i < allUsers.length; i++) {

            AddUser(allUsers[i].UserDetailId, allUsers[i].UserName, allUsers[i].AvatarPic, allUsers[i].IsOnline);
        }
        // Add All Groups
        for (i = 0; i < grouplist.length; i++) {

            var group = grouplist[i];
            AddGroup(group.GroupId, group.GroupName, group.GroupAvatar);
            $("#ul_item_" + group.GroupId).addClass("group_item");
        }

        for (i = 0; i < hisMsglist.length; i++) {
            bindingMsg(hisMsglist[i]);

        }
       // 延时获取申请消息，回复消息
        setTimeout(function () { userHub.server.getUnreadGroupReply(); }, 1000);
        setTimeout(function () { userHub.server.gettUnapproveGroupApply(); }, 2000);
        setTimeout(function () { userHub.server.getUnreadFriendsReply(); }, 3000);
        setTimeout(function () { userHub.server.getUnapproveFriendsApply(); }, 4000);
    };

    // On New User Connected
    userHub.client.onNewUserConnected = function (uid, name) {
        alert(name+"来啦");
        UserIsOnlined(uid);
        //AddUser(chatHub, id, name);
    };


    // On User Disconnected
    userHub.client.onUserDisconnected = function (id, userName) {
        alert(userName + "下线啦");
        $("#img_" + id + "").addClass("gray");

    };





    userHub.client.OutLogin = function () {


        $.connection.hub.stop();
        alert("你的账号在别处登录，被迫下线！");
        location.href = '/User/Login';


    };











    userHub.client.searchResultReceived = function (json) {
        $(".searchresult_status_span ,.searchresult_owner_span").hide();
        $(".searchresult_info").show();
        //‘添加好友’按钮激活
        $("#sendreply").removeAttr("disabled");
        if (json.type === "User") {

            $("#searchresult_avatar").attr("src", json.result.AvatarPic);
            $("#searchresult_uid").val(json.result.UserDetailId);
            $("#searchresult_uid").attr("data-type", "User");
            $("#searchresult_name").text(json.result.UserName);
            $(".searchresult_status_span").show();
            if (json.result.IsOnline === true) { $("#searchresult_status").text("在线"); }
            else { $("#searchresult_status").text("离线"); }


        }
        else {
            $("#searchresult_avatar").attr("src", json.result.Group.GroupAvatar);
            $("#searchresult_uid").val(json.result.Group.GroupId);
            //设置一下该Id的类型，是群还是好友，用于发送添加申请时，定位到不同方法中
            $("#searchresult_uid").attr("data-type", "Group");
            $("#searchresult_name").text(json.result.Group.GroupName);
            $(".searchresult_owner_span").show();
            $("#searchresult_owner").text(json.result.OwnerName);
        }

    };


    userHub.client.applyResult = function (result) {

        //申请成功回调
        if (result === ApplyStatus.Success) {
            alert("已发送申请");
            $("#searchVal").val('');
            $("#sendreply").attr("disabled", "disabled");
            $("#searchresult_uid").val('');
            $(".searchresult_info").css('display', 'none');
            return;

        }
        if (result === ApplyStatus.UnAuthorize) {
            alert("无效的申请");
            $("#searchVal").val('');
            $("#sendreply").attr("disabled", "disabled");
            $("#searchresult_uid").val('');
            $(".searchresult_info").css('display', 'none');
            return;

        }
        if (result === ApplyStatus.Failed) {

            alert("申请失败,请重新申请");
            return;
        }
        if (result === ApplyStatus.Friended) {
            alert("对方已经是你好友");
            return;
        }
        if (result === ApplyStatus.YourSelf) {
            alert("不能添加自己为好友");
            return;
        }
        if (result === ApplyStatus.BeenMember) {
            alert("您已经是该群成员");
            return;
        }


    };

    //收到添加好友的申请的回复
    userHub.client.receiveReplyResult = function (replyModel) {
        //将数据缓存到SeesionStorage       
        PushSeesionStorage("FriendReplys", replyModel);

        if (replyModel.ReplyStatus === ReplyStatus.Pass) {
            //append一条好友头像信息
            AddUser(replyModel.ReplyUserId, replyModel.ReplyUserName, replyModel.ReplyUserAvatar, IsOnline);
        }
        //在消息列表处添加回复消息，并添加红点
        AppendReplyMsgIntoUl();
        $("#ul_item_Replys").find(".avatar .icon").addClass("web_wechat_reddot");
    };


    //收到添加群的申请的回复
    userHub.client.receiveGroupReplyResult = function (replyModel) {
        //将数据缓存到SeesionStorage       
        PushSeesionStorage("GroupReplys", replyModel);


        if (replyModel.ReplyStatus === ReplyStatus.Pass) {
            //append一条好友头像信息
            AddGroup(replyModel.ReplyGroupId, replyModel.ReplyGroupName, replyModel.ReplyGroupAvatar);
            $("#ul_item_" + replyModel.ReplyGroupId).addClass("group_item");
        }
        //在消息列表处添加回复消息，并添加红点
        AppendReplyMsgIntoUl();
        $("#ul_item_Applys").find(".avatar .icon").addClass("web_wechat_reddot");
    };






    //收到后台发来的好友添加申请
    userHub.client.recevieFriendApply = function (applyModel) {
        //将数据缓存到SeesionStorage

        PushSeesionStorage("FriendsApplys", applyModel);
        //在消息列表处添加申请消息，并添加红点
        AppendApplyMsgIntoUl();
        $("#ul_item_Replys").find(".avatar .icon").addClass("web_wechat_reddot");

    };

    //收到后台发来的群添加申请
    userHub.client.recevieGroupApply = function (applyModel) {
        //将数据缓存到SeesionStorage

        PushSeesionStorage("GroupApplys", applyModel);
        //在消息列表处添加申请消息，并添加红点
        AppendApplyMsgIntoUl();
        $("#ul_item_Applys").find(".avatar .icon").addClass("web_wechat_reddot");

    };

   
   


    //收到添加群的申请的回复,属于历史消息
    userHub.client.receiveGroupReplyList = function (replyList) {
        if (replyList.length > 0) {
            //将数据缓存到SeesionStorage       
            for (var i = 0; i < replyList.length; i++) {
                var replyModel = replyList[i];
                PushSeesionStorage("GroupReplys", replyModel);
            }
            //在消息列表处添加回复消息，并添加红点
            AppendReplyMsgIntoUl();
            $("#ul_item_Replys").find(".avatar .icon").addClass("web_wechat_reddot");
        }
    };


    //收到添加好友的申请的回复,属于历史消息
    userHub.client.receiveFriendReplyList = function (replyList) {
        if (replyList.length > 0) {
            //将数据缓存到SeesionStorage       
            for (var i = 0; i < replyList.length; i++) {
                var replyModel = replyList[i];
                PushSeesionStorage("FriendReplys", replyModel);
            }
            //在消息列表处添加回复消息，并添加红点
            AppendReplyMsgIntoUl();
            $("#ul_item_Replys").find(".avatar .icon").addClass("web_wechat_reddot");
        }
    };




    //收到添加群的申请消息,属于历史消息
    userHub.client.receiveGroupApplyList = function (applyList) {
        if (applyList.length > 0) {
            //将数据缓存到SeesionStorage       
            for (var i = 0; i < applyList.length; i++) {
                var applyModel = applyList[i];
                PushSeesionStorage("GroupApplys", applyModel);
            }
            //在消息列表处添加回复消息，并添加红点
            AppendApplyMsgIntoUl();
            $("#ul_item_Applys").find(".avatar .icon").addClass("web_wechat_reddot");
        }
    };
    //收到添加好友的申请消息,属于历史消息
    userHub.client.receiveFriendApplyList = function (applyList) {
        if (applyList.length > 0) {
            //将数据缓存到SeesionStorage       
            for (var i = 0; i < applyList.length; i++) {
                var applyModel = applyList[i];
                PushSeesionStorage("FriendApplys", applyModel);
            }
            //在消息列表处添加回复消息，并添加红点
            AppendApplyMsgIntoUl();
            $("#ul_item_Applys").find(".avatar .icon").addClass("web_wechat_reddot");
        }
    };
}