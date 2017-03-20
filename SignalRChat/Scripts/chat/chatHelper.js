$(function () {
    $('.image-editor').cropit({
        imageState: {
            src: '1.jpg'
        }
    });

    $('.rotate-cw').click(function () {
        $('.image-editor').cropit('rotateCW');
    });
    $('.rotate-ccw').click(function () {
        $('.image-editor').cropit('rotateCCW');
    });

    $('.export').click(function () {
        var imageData = $('.image-editor').cropit('export');
        $("#pre").attr("src", imageData);
        $("#preview").attr("src", imageData);
        var data = imageData.replace("data:image/png;base64,", "");
        $("#img-data").val(data);
    });
});





//绑定聊天消息到聊天面板
function bindingMsg(model) {
    var uid = model.Message.SenderId;
    //绑定历史最新消息
    var unreadhtml = "<span class='ng-binding' data-creatime='"
        + model.Message.CreateTime + "'>"
    + model.Message.content + "</span>";
    //如果未读消息大于0
    if (model.UnReadMsgCount > 0) {
        $("#ul_item_" + uid + "").find(".avatar .icon").addClass("web_wechat_reddot");
         unreadhtml = "<span class='ng-binding ng-scope unreadcount'>[" + model.UnReadMsgCount + "条]</span>" + unreadhtml;
        $("#ul_item_" + uid + "").addClass("NeedGetMsgFromBack");
        $("#ul_item_" + uid + "").attr("data-unreadCount", model.UnReadMsgCount);
        $("#ul_item_" + uid + "").attr("data-unreadMsgId", model.Message.PrivateMessageId);
    }




    $("#ul_item_" + uid + "").find(".chat_item_info p.msg").append(unreadhtml);

}


function GetStrogeMessage(chatingId) {
    var key = key = "MessageListWith_" + chatingId;
    var str = sessionStorage.getItem(key);
    if (str !== null) {
        var list = JSON.parse(str);
        for (var i = 0; i < list.length; i++) {

            AddMessage(list[i]);

        }
    }
    return;
}

function AddMessage(message) {
    var chatingid = $("#currentUserName").attr("data-uid");
    var src = $("#img_" + chatingid + "").attr("src");
    var bubble_default = "bubble_default";
    var me = "";
    var MyId = $("#hdUserName").attr("data-uid");
    //接收者id不等于我的Id，说明我是发送者
    if (message.SenderId.trim() === MyId.trim()) {
        src = $("#myheadsrc").attr("src");
        bubble_default = "bubble_primary ";
        me = "me";
    }
    //绑定聊天对话框消息，前端
    var str =
   " <div class='message_item message " + me + "' style=' margin-bottom: 16px; float: left; width: 100%;'>" +
                       " <p class='message_system ng-scope'>" +
                        " <span class='content ng-binding'>" + getTime(message.CreateTime, "") + "</span>" +
                      "</p>" +
                       " <img class='avatar' src='" + src + "' title='" + message.SenderName + "'>" +
                        "<div class='content'>" +
                         "<div class='bubble js_message_bubble ng-scope " + bubble_default + " left'>" +
                          "<div class='bubble_cont ng-scope'>" +
                           "<div class='plain'>" +
                            "            <pre class='js_message_plain ng-binding'>" + message.content + "</pre>" +
                             " </div>" +
                              "  </div</div></div></div>";
    $(".message_ul").append(str);
    //绑定好友列表出的聊天消息，前端
    if ($("#ul_item_" + message.RecevierId + "").find(".chat_item_info p.msg").find("span").length === 0) {

        var unreadhtml = "<span class='ng-binding' data-creatime='" + message.CreateTime + "'>"
        + message.content + "</span>";
        $("#ul_item_" + message.RecevierId + "").find(".chat_item_info p.msg").append(unreadhtml);
    }

}