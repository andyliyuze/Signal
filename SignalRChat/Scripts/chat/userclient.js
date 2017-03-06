function registerClientMethodsForUser(userHub) {

    userHub.client.searchResultReceived = function (model) {
        $(".searchresult_info").show();
        $("#searchresult_avatar").attr("src", model.AvatarPic);
        $("#searchresult_uid").val(model.UserDetailId);
        $("#searchresult_name").text(model.UserName);

        if (model.IsOnline == true) { $("#searchresult_status").text("在线"); }
        else { $("#searchresult_status").text("离线"); }
        //‘添加好友’按钮激活
        $("#sendreply").removeAttr("disabled");

    }


    userHub.client.applyResult = function (result) {

        //申请成功回调
        if (result == ApplyStatus.Success) {
            alert("已发送申请");
            $("#searchVal").val('');
            $("#sendreply").attr("disabled", "disabled");
            $("#searchresult_uid").val('');
            $(".searchresult_info").css('display', 'none');
            return;

        }
        if (result == ApplyStatus.UnAuthorize) {
            alert("无效的申请")
            $("#searchVal").val('');
            $("#sendreply").attr("disabled", "disabled");
            $("#searchresult_uid").val('');
            $(".searchresult_info").css('display', 'none');
            return;

        }
        if (result == ApplyStatus.Failed) {

            alert("申请失败,请重新申请");
            return;
        }
        if (result == ApplyStatus.Friended) {
            alert("对方已经是你好友");
            return;
        }
        if (result = ApplyStatus.YourSelf) {
            alert("不能添加自己为好友");
            return;
        }


    }

    //收到添加好友的申请的回复
    userHub.client.receiveReplyResult = function (replyModel) {
        //将数据缓存到SeesionStorage       
        PushSeesionStorage("FriendReplys", replyModel);
        var IsOnline = true;
        var Id = "Replys";
        var Name = "申请回复";
        var avatarPic = "Images/usericon.jpg";
        //防添加止重复
        if ($(".reply_ul_item").length <= 0) {
            //append一条好友申请的回复
            AddUser(Id, Name, avatarPic, IsOnline);
        }
        //append一条好友头像信息
        AddUser(replyModel.ReplyUserId, replyModel.ReplyUserName, replyModel.ReplyUserAvatar, IsOnline);
        $("#ul_item_Replys").find(".avatar .icon").addClass("web_wechat_reddot");
    }
    //收到后台发来的好友添加申请
    userHub.client.recevieApply = function (applyModel) {
        //将数据缓存到SeesionStorage

        PushSeesionStorage("FriendsApplys", applyModel);


        if ($(".apply_ul_item").length <= 0) {
            var id = "Applys";
            var name = "添加申请";
            var avatarPic = "Images/usericon.jpg";
            var IsOnline = true;
            AddUser(id, name, avatarPic, IsOnline);
        }


    }
    //审过别人的好友申请时，后台回传一个改好友当前信息，将此添加到当前
    userHub.client.appendFriends = function (usermodel) {

        AddUser(usermodel.UserDetailId, usermodel.UserName, usermodel.AvatarPic, usermodel.IsOnline);
    }
}