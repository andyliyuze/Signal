﻿function AddUser(id, name, avatarPic, IsOnline) {
    var str =
           " <div data-uid=" + id + " class='chat_ul_item OffLine friends_item'id='friends_item_" + id + "'>" +
              
                "<div class='avatar'>" +

                    "<img class='img' id='img_" + id + "' src='" + avatarPic + "' />" +

                    "<i class='icon ng-scope'></i>" +



               " </div>" +
                "<div class='chat_item_info'>" +
                    "<h3 class='nickname'>" +
                        "<span class='nickname_text ng-binding friends_item_nickname'>" + name + "</span>" +
                      
                    "</h3>" +

                   "</div></div>";

    if (IsOnline == true) {
        str = str.replace("class='img gray'", "class='img'");
        str = str.replace("chat_ul_item OffLine", "chat_ul_item OnLine");
    }
 

   
    $(".friends_ul .chat_ul_div").append(str); return;


}







function AddUserForChat(id, name, avatarPic, IsOnline ,type) {

    var str =
               " <div data-uid=" + id + " class='chat_ul_item OffLine chat_item "+type+" 'id='ul_item_" + id + "'>" +
                "<div class='ext'>" +
                    "<p class='attr ng-binding'>" + getTime( Date.now(), "ext") + "</p>" +
                   " <p class='attr ng-scope'>" +
                       " <i class='web_wechat_no-remind'></i>" +
                    "</p>" +
               " </div>" +
                "<div class='avatar'>" +

                    "<img class='img gray' id='img_" + id + "' src='" + avatarPic + "' />" +

                    "<i class='icon ng-scope'></i>" +



               " </div>" +
                "<div class='chat_item_info'>" +
                    "<h3 class='nickname'>" +
                        "<span class='nickname_text ng-binding'>" + name + "</span>" +
                     
                    "</h3>" +
                   " <p class='msg ng-scope' >"+
                   "<span class='ng-binding content' data-creatime='2017-04-11T12:15:40.528+08:00'></span>"+
                  
              "  </p>"
                   "</div></div>";

    if (IsOnline == true) {
        str = str.replace("class='img gray'", "class='img'");
        str = str.replace("chat_ul_item OffLine", "chat_ul_item OnLine");
    }
    //如果Id为applys说明是好友申请的消息
    if (id == "Applys") {
        str = str.replace("chat_ul_item OnLine", "apply_ul_item chat_ul_item OnLine");

        $(".chat_ul .chat_ul_div").prepend(str);
        return;
    }
    if (id == "Replys") {
        str = str.replace("chat_ul_item OnLine", "reply_ul_item chat_ul_item OnLine");

        $(".chat_ul .chat_ul_div").prepend(str);
        return;
    }
    else { $(".chat_ul .chat_ul_div").append(str); return; }

}













function UserIsOnlined(id) {
    if ($("#ul_item_" + id + "").length > 0)
    {
        $("#ul_item_" + id).find(".avatar img").removeClass("gray");
        var str = $("#ul_item_" + id + "").html();
        $("#ul_item_" + id + "").remove();
        str = "<div class='chat_ul_item chat_item Online' id='ul_item_" + id + "' data-uid='" + id + "'>" + str + "</div>";
        if ($(".chat_item.OffLine").length != 0) { $(".chat_item.OffLine").before(str); return; }
        if ($(".chat_item.Online").length != 0) { $(".chat_item.Onlie").after(str); return; }
        else { $(".chat_ul .chat_ul_div").append(str); }
    }
    //更改一下sessionstroage的值
    UpdateIsOnlineForSessionStroage(id,true);

}
function UserIsOfflined(id)
{
    $("#ul_item_" + id).find(".avatar img").addClass("gray");
    //更改一下sessionstroage的值
    UpdateIsOnlineForSessionStroage(id,false);
}

function IsCurrentUserOrNotUser(e) {

    if ($(e).hasClass("apply_ul_item")) { return true; }
    if ($(e).hasClass("reply_ul_item")) { return true; }
    if ($(e).attr("data-uid").trim() == $("#currentUserName").attr("data-uid")) { return true; }
}

function IsCurrentGroupOrNotUser(e) {

    if ($(e).attr("data-uid").trim() == $("#currentUserName").attr("data-uid")) { return true; }

}

//绑定添加申请记录到弹框页面
function bindingApplys(FriendsApplys) {


    for (var i = 0; i < FriendsApplys.length; i++) {
        var item = FriendsApplys[i];
        var RightParttext = "";
        if (item.replyResult != "" && item.replyResult != undefined) {
            RightParttext =
                   " <div class='btn-group  btn-group-sm' style='float: right;display:inline;padding-top: 11px;'>" +
                item.replyResult+
            "  </div>";
        }
        else {

            RightParttext =
                " <div class='btn-group  btn-group-sm' style='float: right;display:inline;'>" +
           
                "<button type='button' class='reply-pass friend btn btn-primary'>通过</button>" +
                    "  <button type='button' class='reply-refuse friend btn btn-danger'>拒绝</button>" +
                  "    <button type='button' class='reply-ignore friend btn btn-warning'>忽略</button>" +
            "  </div>"  ;

        }
        var str =
           "    <li style='' class='list-group-item'>" +
           "       <div style='    display: inline-block;width: 50%;'>" +
               "<input class='replyUserIsOnline' value='" + item.IsOnline + "' style='display:none' >" +
              "        <img class='reply-avatar' style=' width: 50px; display: block; float: left;   margin-right: 10px;' src='" + item.ApplyUserAvatar + "' />" +
                  "    <span class='reply-name' data-uid='" + item.ApplyUserId + "'data-applyid='" + item.FriendsApplyId + "' style='font-family:'微软雅黑','黑体','宋体'' ;padding-top 5px;display block;>" + item.ApplyUserName + "</span>" +
                   "   <span class='reply-datetime' style='display: block;font-family: '微软雅黑','黑体','宋体';padding-top: 5px;6font-size: 10px;'>" + getTimeFromBackGroung(item.ApplyTime, "") + "</span></div>" +
 
                  RightParttext+
     
        "  <div style=' clear: both;'></div>" +
     " </li>"
        ;

        //在选中元素之前插入元素
        $(".friendsApply").prepend(str);
    }





}




function bindingGroupApplys(GroupApplys) {


    for (var i = 0; i < GroupApplys.length; i++) {
        var item = GroupApplys[i];
        var RightParttext = "";
        if (item.replyResult != "" && item.replyResult != undefined) {
            RightParttext =
                   " <div class='btn-group  btn-group-sm' style='float: right;display:inline;padding-top: 11px;'>" +
                item.replyResult +
            "  </div>";
        }
        else {

            RightParttext =
                " <div class='btn-group  btn-group-sm' style='float: right;display:inline;'>" +

                "<button type='button' class='reply-pass group btn btn-primary'>通过</button>" +
                    "  <button type='button' class='reply-refuse group btn btn-danger'>拒绝</button>" +
                  "    <button type='button' class='reply-ignore group btn btn-warning'>忽略</button>" +
            "  </div>";

        }
        var middletext = "<div style=' bottom: 5px;position: relative;  display: inline;font-weight: 600; color: #337ab7;'>申请进入" + item.GroupName+ "</div>"

        var str =
           "    <li style='' class='list-group-item'>" +
           "       <div style='    display: inline-block;width: 39%;'>" +
               "<input class='replyUserIsOnline' value='" + item.IsOnline + "' style='display:none' >" +
              "        <img class='reply-avatar' style=' width: 50px; display: block; float: left;   margin-right: 10px;' src='" + item.ApplyUserAvatar + "' />" +
                  "    <span class='reply-name' data-uid='" + item.ApplyUserId + "'data-applyid='" + item.GroupApplyId + "' style='font-family:'微软雅黑','黑体','宋体'' ;padding-top 5px;display block;>" + item.ApplyUserName + "</span>" +
                   "   <span class='reply-datetime' style='display: block;font-family: '微软雅黑','黑体','宋体';padding-top: 5px;6font-size: 10px;'>" + getTimeFromBackGroung(item.ApplyTime, "") + "</span></div>" +
                   middletext+
           
                  RightParttext+
        
        "  <div style=' clear: both;'></div>" +
     " </li>"
        ;

        //在选中元素之前插入元素
        $(".groupApply").prepend(str);
    }





}
//



//绑定添加好友回复的消息到弹框页面
function bindingReplys(Replys) {

    for (var i = 0; i < Replys.length; i++) {
        var item = Replys[i];
        var result = GetReplysResultStr(item.ReplyStatus);

        var str =
           "<li style='' class='list-group-item'>" +
           "<div style='    display: inline-block;width: 50%;'>" +
              " <img class='reply-avatar' style=' width: 50px; display: block; float: left;   margin-right: 10px;' src='" + item.ReplyUserAvatar + "' />" +
                  "<span class='reply-name' data-uid='" + item.ReplyUserId + "'data-applyid='"
                  + item.FriendsApplyId + "' style='font-family:'微软雅黑','黑体','宋体'' ;padding-top 5px;display block;>"
                  + item.ReplyUserName + "</span>" +
                   "<span class='reply-datetime' style='display: block;font-family: '微软雅黑','黑体','宋体';padding-top: 5px;6font-size: 10px;'>" + getTimeFromBackGroung(item.ReplyTime, "") + "</span></div>" +
                   "<div class='btn-group  btn-group-sm' style='float: right;display:inline;padding-top: 11px;'>" +
                  result
        "</div>" +
        "<div style=' clear: both;'></div>" +
     " </li>"
        ;

        //在选中元素之前插入元素
        $(".friendsReply").prepend(str);
    }

}

//绑定添加群添加回复的消息到弹框页面
function bindingGroupReplys(Replys) {

    for (var i = 0; i < Replys.length; i++) {
        var item = Replys[i];
        var result = GetReplysResultStr(Replys[i].ReplyStatus);

        var str =
           "<li style='' class='list-group-item'>" +
           "<div style='    display: inline-block;width: 50%;'>" +
              " <img class='reply-avatar' style=' width: 50px; display: block; float: left;   margin-right: 10px;' src='" + item.ReplyGroupAvatar + "' />" +
                  "<span class='reply-name' data-groupid='" + item.ReplyGroupId + "'data-applyid='" + item.AppyId + "' style='font-family:'微软雅黑','黑体','宋体'' ;padding-top 5px;display block;>" + item.ReplyGroupName + "</span>" +
                   "<span class='reply-datetime' style='display: block;font-family: '微软雅黑','黑体','宋体';padding-top: 5px;6font-size: 10px;'>" + getTimeFromBackGroung(item.ReplyTime, "") + "</span></div>" +
                   "<div class='btn-group  btn-group-sm' style='float: right;display:inline;padding-top: 11px;'>" +
                  result
        "</div>" +
        "<div style=' clear: both;'></div>" +
     " </li>"
        ;

        //在选中元素之前插入元素
        $(".groupReply").prepend(str);
    }

}

 
//根据申请消息Id从SeesionStorage获取改item
function UpdateItemById(id, result, key,IdName) {
    var list = GetSeesionStorageList(key);
    var temp;
    list.filter(function (index) {      
        index = filterForApplys(index, id, result,IdName);
        return true;
    });
    sessionStorage.removeItem(key);
    for (var i = 0; i < list.length; i++) {
        var item = list[i];
        PushSeesionStorage(key, item);
    }

}
//更改申请消息的item sessionstorage
function filterForApplys(index, id, result, IdName) {

    if (index[IdName] != undefined && index[IdName] == id)
    {
        index.replyResult = result;
    }
   
    return index;
}
//更改回复消息的item sessionstorage
function filterForReplys(index,id,result) {

    if (index.AppyId == id) {
        index.HasReadResult = result;
    }
    return index;
}
//根据枚举值获取申请结果字符串
function GetReplysResultStr(replyStatus) {
    if (replyStatus == ReplyStatus.Pass) {
        return "已通过";
    }
    if (replyStatus == ReplyStatus.Decline) {
        return "已拒绝";
    }
    if (replyStatus == ReplyStatus.Ignore) {
        return "已忽略";
    }


}

//根据鼠标点击结果获取reply数据
function createReplyModel(e) {
    var applyuid = $(e).parent().parent(".list-group-item").find(".reply-name").attr("data-uid");
    var ReceiverUserId = $("#hdUserName").attr("data-uid");
    var IsOnlineStr = $(e).parent().parent(".list-group-item").find(".replyUserIsOnline").val();
    var IsOnline = false;
    if (IsOnlineStr == "true") {
        IsOnline = true;
    }
    var ApplyId = $(e).parent().parent(".list-group-item").find(".reply-name").attr("data-applyid");
    var ApplyName = $(e).parent().parent(".list-group-item").find(".reply-name").text();
    var avatar = $(e).parent().parent(".list-group-item").find(".reply-avatar").attr("src");
    var ReplyModel = {

        ApplyUid: applyuid,
        ReceiverUserId: ReceiverUserId,
        ApplyName: ApplyName,
        IsOnline: IsOnline,
        ApplyId: ApplyId,
        Avatar: avatar,
    }
    return ReplyModel;
}

//更新stroage和数据库的未读消息
function UpdateUnreadFriendsApply(Applys) {
    //获取所有未读回复的item
    var ApplyIdlist = [];
    for (var i = 0; i < Applys.length; i++) {
        var item = Applys[i];
        if (item.HasReadResult == false) {
            //移除一个元素
         //   Applys.pop(item);
            //更改Item属性
            item.HasReadResult = true;
            //向数组开头添加元素
           // Applys.unshift(item);
            //该数组是数据库需要更新的数组
            ApplyIdlist.push(item.AppyId);
        }
    }
    if (ApplyIdlist.length > 0) {
        //更新一下Stroage
        PushArrInSessionStroage("FriendReplys", Applys);
        //跟新一下数据库
        userHub.server.friendReplyHaveRead(ApplyIdlist);

    }

}
function UpdateUnreadGroupApply(Replys) {
    //获取所有未读回复的item
    var ApplyIdlist = [];
    for (var i = 0; i < Replys.length; i++) {
        var item = Replys[i];
        if (item.HasReadResult == false) {
            //移除一个元素
            Replys.pop(item);
            //更改Item属性
            item.HasReadResult = true;
            //向数组开头添加元素
            Replys.unshift(item);
            //该数组是数据库需要更新的数组
            ApplyIdlist.push(item.AppyId);
        }
    }
    if (ApplyIdlist.length > 0) {
        //更新一下Stroage
        PushArrInSessionStroage("GroupReplys", Replys);
        //跟新一下数据库
        userHub.server.groupReplyHaveRead(ApplyIdlist);

    }

}


function ShowReplyModalBeforeCheck(FriendsReplysLength, GroupReplysLength)
{
    if (FriendsReplysLength == 0) {
        $(".myTab_li_friendsReoly").hide();
        $(".myTab_li_friendsReoly").removeClass("active");
        $(".myTab_li_groupReoly").addClass("active");

        $("#friend_tab").removeClass("in active");
        $("#group_tab").addClass("in active");

    }
    if (GroupReplysLength == 0) {
        $(".myTab_li_groupReoly").hide();
        $(".myTab_li_groupReoly").removeClass("active");
        $(".myTab_li_friendsReoly").addClass("active");

        
        $("#group_tab").removeClass("in active");
        $("#friend_tab").addClass("in active");

    }
}

//好友，群通用
function AppendApplyMsgIntoUl()
{
    if ($(".apply_ul_item").length <= 0) {
        var id = "Applys";
        var name = "添加申请";
        var avatarPic = "/Images/usericon.jpg";
        var IsOnline = true;
        AddUserForChat(id, name, avatarPic, IsOnline,"apply");
    }
}
//好友，群通用
function AppendReplyMsgIntoUl()
{
   
    //防添加止重复
    if ($(".reply_ul_item").length <= 0) {
        var IsOnline = true;
        var Id = "Replys";
        var Name = "申请回复";
        var avatarPic = "/Images/usericon.jpg";
        //append一条好友申请的回复
        AddUserForChat(Id, Name, avatarPic, IsOnline,"reply");
    }
}

//获取用户
function GetUserByUserId(Id)
{
    var user = GetSeesionStorageList("FriendsList");
    user = user.filter(function (index) {
        return index["UserDetailId"].trim() == Id;
    });
    user = user[0];
    return user;
}

//更新用户在线状态
function UpdateIsOnlineForSessionStroage(Id, IsOnline)
{
    var friends = GetSeesionStorageList("FriendsList");
    var user = JSLINQ(friends).Where(item=> item.UserDetailId == Id).items[0];
    
    var index = friends.indexOf(user);
    user.IsOnline = IsOnline;
    friends.splice(index, user);
    RemoveByKey("FriendsList");
    PushArrInSessionStroage("FriendsList", friends);
}
//获取用户
function GetUserByKeyword(keyword) {
    var users = [];
    if ($.trim(keyword) == "") { return users; }
    var friends = GetSeesionStorageList("FriendsList");
     users = JSLINQ(friends).Where(item=> item.UserName.indexOf(keyword) > -1).items;
    return users;
}