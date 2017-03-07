﻿function AddUser(id, name, avatarPic, IsOnline) {
    var str =
           " <div data-uid=" + id + " class='chat_ul_item OffLine'id='ul_item_" + id + "'>" +
                "<div class='ext'>" +
                    "<p class='attr ng-binding'>22:29</p>" +
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
                       "<p class='msg ng-scope'></p>" +
                    "</h3>" +

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
    $("#img_" + id + "").removeClass("gray");
    var str = $("#ul_item_" + id + "").html();
    $("#ul_item_" + id + "").remove();

    str = "<div class='chat_ul_item'id='ul_item_" + id + "' data-uid='" + id + "'>" + str + "</div>";
    if ($(".chat_ul_item.OffLine").length != 0) { $(".chat_ul_item.OffLine").before(str); return; }
    if ($(".chat_ul_item.Online").length != 0) { $(".chat_ul_item.Onlie").after(str); return; }
    else { $(".chat_ul .chat_ul_div").append(str); }
}


function IsCurrentUserOrNotUser(e) {

    if ($(e).hasClass("apply_ul_item")) { return true; }
    if ($(e).hasClass("reply_ul_item")) { return true; }
    if ($(e).attr("data-uid").trim() == $("#currentUserName").attr("data-uid")) { return true; }
}

//绑定添加申请记录到弹框页面
function bindingApplys(FriendsApplys) {


    for (var i = 0; i < FriendsApplys.length; i++) {
        var item = FriendsApplys[i];
        var RightParttext = "";
        if (item.replyResult != "" && item.replyResult != undefined) {
            RightParttext = item.replyResult;
        }
        else {

            RightParttext = "<button type='button' class='reply-pass btn btn-primary'>通过</button>" +
                    "  <button type='button' class='reply-refuse btn btn-danger'>拒绝</button>" +
                  "    <button type='button' class='reply-ignore btn btn-warning'>忽略</button>";

        }
        var str =
           "    <li style='' class='list-group-item'>" +
           "       <div style='    display: inline-block;width: 50%;'>" +
               "<input class='replyUserIsOnline' value='" + item.IsOnline + "' style='display:none' >" +
              "        <img class='reply-avatar' style=' width: 50px; display: block; float: left;   margin-right: 10px;' src='" + item.ApplyUserAvatar + "' />" +
                  "    <span class='reply-name' data-uid='" + item.ApplyUserId + "'data-applyid='" + item.FriendsApplyId + "' style='font-family:'微软雅黑','黑体','宋体'' ;padding-top 5px;display block;>" + item.ApplyUserName + "</span>" +
                   "   <span class='reply-datetime' style='display: block;font-family: '微软雅黑','黑体','宋体';padding-top: 5px;6font-size: 10px;'>" + getTimeFromBackGroung(item.ApplyTime, "") + "</span></div>" +

              " <div class='btn-group  btn-group-sm' style='float: right;display:inline;padding-top: 11px;'>" +
                  RightParttext
        "  </div>" +
        "  <div style=' clear: both;'></div>" +
     " </li>"
        ;

        //在选中元素之前插入元素
        $(".friendsApply").prepend(str);
    }





}
//绑定添加回复的消息到弹框页面
function bindingReplys(Replys) {

    for (var i = 0; i < Replys.length; i++) {
        var item = Replys[i];
        var result = GetReplysResultStr(Replys[i].ReplyStatus);

        var str =
           "<li style='' class='list-group-item'>" +
           "<div style='    display: inline-block;width: 50%;'>" +
              " <img class='reply-avatar' style=' width: 50px; display: block; float: left;   margin-right: 10px;' src='" + item.ReplyUserAvatar + "' />" +
                  "<span class='reply-name' data-uid='" + item.ReplyUserId + "'data-applyid='" + item.FriendsApplyId + "' style='font-family:'微软雅黑','黑体','宋体'' ;padding-top 5px;display block;>" + item.ReplyUserName + "</span>" +
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

 
//根据申请消息Id从SeesionStorage获取改item
function UpdateItemById(id, result, key) {
    var list = GetSeesionStorageList(key);
    var temp;
    list.filter(function (index) {
      
            index = filterForApplys(index, id, result);
      
         
        return true;
    });
    sessionStorage.removeItem(key);
    for (var i = 0; i < list.length; i++) {
        var item = list[i];
        PushSeesionStorage(key, item);
    }

}
//更改申请消息的item sessionstorage
function filterForApplys(index, id, result) {

    if (index.FriendsApplyId == id) {
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
    var ReplyName = $(e).parent().parent(".list-group-item").find(".reply-name").text();
    var avatar = $(e).parent().parent(".list-group-item").find(".reply-avatar").attr("src");
    var ReplyModel = {

        ApplyUid: applyuid,
        ReceiverUserId: ReceiverUserId,
        ReplyName: ReplyName,
        IsOnline: IsOnline,
        ApplyId: ApplyId,
        Avatar: avatar,
    }
    return ReplyModel;
}