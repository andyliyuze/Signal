function registerClientMethodsForUser(userHub) {

    userHub.client.searchResultReceived = function (json) {
        $(".searchresult_status_span ,.searchresult_owner_span").hide();
        $(".searchresult_info").show();
        //‘添加好友’按钮激活
        $("#sendreply").removeAttr("disabled");
        if (json.type == "User") {
           
            $("#searchresult_avatar").attr("src", json.result.AvatarPic);
            $("#searchresult_uid").val(json.result.UserDetailId);
            $("#searchresult_uid").attr("data-type", "User");
            $("#searchresult_name").text(json.result.UserName);
            $(".searchresult_status_span").show();
            if (json.result.IsOnline == true) { $("#searchresult_status").text("在线"); }
            else { $("#searchresult_status").text("离线"); }
           
          
        }
        else {
            $("#searchresult_avatar").attr("src", json.result.Group.GroupAvatar);
            $("#searchresult_uid").val(json.result.Group.GroupId);
            //设置一下该Id的类型，是群还是好友，用于发送添加申请时，定位到不同方法中
            $("#searchresult_uid").attr("data-type", "Group");
            $("#searchresult_name").text(json.result.Group.GroupName);
            $(".searchresult_owner_span").show();
            $("#searchresult_owner").text(json.result.OwnerName)
        }

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


    //收到添加群的申请的回复
    userHub.client.receiveGroupReplyResult = function (replyModel) {
        //将数据缓存到SeesionStorage       
        PushSeesionStorage("GroupReplys", replyModel);
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
    
        AddGroup(replyModel.ReplyGroupId, replyModel.ReplyGroupName, replyModel.ReplyGroupAvatar);
        $("#ul_item_Replys").find(".avatar .icon").addClass("web_wechat_reddot");
    }






    //收到后台发来的好友添加申请
    userHub.client.recevieFriendApply = function (applyModel) {
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

    //收到后台发来的群添加申请
    userHub.client.recevieGroupApply = function (applyModel) {
        //将数据缓存到SeesionStorage

        PushSeesionStorage("GroupApplys", applyModel);
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

    userHub.client.beGroupMember = function ()
    {

    }
}