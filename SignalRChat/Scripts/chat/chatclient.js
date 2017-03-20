function registerClientMethodsForChat(chatHub) {

    // Calls when user successfully logged in
    chatHub.client.onConnected = function (user, allUsers,grouplist, hisMsglist) {

        setScreen(true);
        userHub.state.Uid = chatHub.state.Uid;
        userHub.state.CurrentUserInfo = chatHub.state.CurrentUserInfo;


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
        }

        for (i = 0; i < hisMsglist.length; i++) {
            bindingMsg(hisMsglist[i]);

        }
        //延时获取申请消息，回复消息
        setTimeout(function () { userHub.server.getUnreadGroupReply();  }, 1000);
        setTimeout(function () { userHub.server.gettUnapproveGroupApply(); }, 2000);
        setTimeout(function () { userHub.server.getUnreadFriendsReply(); }, 3000);
        setTimeout(function () { userHub.server.getUnapproveFriendsApply(); }, 4000);
    }

    // On New User Connected
    chatHub.client.onNewUserConnected = function (uid, name) {
        alert("有人来啦");
        UserIsOnlined(uid);
        //AddUser(chatHub, id, name);
    }


    // On User Disconnected
    chatHub.client.onUserDisconnected = function (id, userName) {
        alert(userName + "下线啦！");
        $("#img_" + id + "").addClass("gray");

    }
    //接收后台传来的单条消息
    //废弃的
    chatHub.client.messageReceived = function (userName, message) {
        var key = "MessageListWith_" + message.SenderId;
        PushSeesionStorage(key, message);
        AddMessage(userName, message);
    }
    //接收后台传来的消息集合
    chatHub.client.messageListReceived = function (MessageList) {
        for (var i = 0; i < MessageList.length; i++) {
            //将消息push进Storage，本地存储
            var key = "MessageListWith_" + MessageList[i].SenderId;
            PushSeesionStorage(key, MessageList[i]);
            AddMessage(MessageList[i]);

        }

    }

    //接收后台传来的单条消息
    chatHub.client.receivePrivateMessage = function (message) {

        var uid = message.SenderId;
        //将消息push进Storage，本地存储
        var key = "MessageListWith_" + message.SenderId;
        PushSeesionStorage(key, message);

        //绑定消息在聊天对话框框
        if (message.SenderId.trim() === $("#currentUserName").attr("data-uid").trim()) {

            AddMessage(message);
            $("#ul_item_" + uid + "").find(".ext:eq(0)").text(getTime(message.CreateTime));
            $("#ul_item_" + uid + "").find(".chat_item_info p.msg").find("span:eq(0)").text(message.content);
            chatHub.server.messageConfirm($("#currentUserName").attr("data-uid").trim());
            return;
        }
        //绑定消息在好友列表处,之前已存在未读
        if ($("#ul_item_" + uid + "").find(".avatar .icon").hasClass("web_wechat_reddot")) {
            var $unreadText = $("#ul_item_" + uid + "").find(".chat_item_info p.msg").find(".unreadcount");
            var unreadcount = $unreadText.text();
            unreadcount = eval(getNum($unreadText.text())) + 1;
            $unreadText.text("[" + unreadcount + "条]");

            $("#ul_item_" + uid + "").find(".ext:eq(0)").text(getTime(message.CreateTime));
            $("#ul_item_" + uid + "").find(".chat_item_info p.msg").find("span:eq(1)").text(message.content);
            return;
        }
            //绑定消息在好友列表处，之前不存在未读消息
        else {
            //先请掉之前的消息
            $("#ul_item_" + uid + "").find(".chat_item_info p.msg").find("span").remove();
            //绑定历史最新消息
            var unreadhtml = "<span class='ng-binding' data-creatime='"
                + message.CreateTime + "'>"
              + message.content + "</span>";
            //如果未读消息大于0
            $("#ul_item_" + uid + "").find(".ext:eq(0)").text(getTime(message.CreateTime));
            $("#ul_item_" + uid + "").find(".avatar .icon").addClass("web_wechat_reddot");
             unreadhtml = "<span class='ng-binding ng-scope unreadcount'>[1条]</span>" + unreadhtml;
            $("#ul_item_" + uid + "").find(".chat_item_info p.msg").append(unreadhtml);
            return;
        }


    }
    //登陆结果
    chatHub.client.loginResult = function (result) {

        if (result === LoginStatus.Success) {
            return;

        }
        if (result === LoginStatus.Failed) { alert("密码错误"); }
        if (result === LoginStatus.UserUnExist) { alert("用户名不存在"); }

    }
    chatHub.client.OutLogin = function () {


        $.connection.hub.stop();
        alert("你的账号在别处登录，被迫下线！");
        location.reload();


    }

    chatHub.client.change = function (oldcid, newcid) {

        $("#" + oldcid + "").attr("id", newcid);

    }
    chatHub.client.updateCount = function (hitCount) {
        $("#counter div").html(hitCount);
    };

    chatHub.client.AppendLastMessage = function (list) {
        for (var i = 0; i < list.length; i++) {
            $('#divChatWindow').prepend('<div class="message"><span class="userName">' + list[i].UserName + '</span>: ' + list[i].Message + '</div>');
        }
        var height = $('#divChatWindow')[0].scrollHeight;
        $('#divChatWindow').scrollTop(height);
        $('.waterfllow-loading.active').removeClass('active');
    }


}