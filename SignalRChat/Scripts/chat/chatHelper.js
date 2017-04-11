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
    //首先检查当前用户是否在聊天列表
    var uid = model.Message.ChattingId;

    if ($("#ul_item_" + uid + "").length <= 0)
    {
        var user = GetSeesionStorageList("FriendsList");
       user= user.filter(function (index) {
            if(index["UserDetailId"].trim()==uid)
            return true;
        });
        user = user[0];
        AddUserForChat(uid, user.UserName, user.AvatarPic, user.IsOnline);

    }
   
    
    //如果未读消息大于0
    if (model.UnReadMsgCount > 0) {
        $("#ul_item_" + uid + "").find(".avatar .icon").addClass("web_wechat_reddot");
         var    unreadhtml = "<span class='ng-binding ng-scope unreadcount'>[" + model.UnReadMsgCount + "条]</span>";
        $("#ul_item_" + uid + "").addClass("NeedGetMsgFromBack");
        $("#ul_item_" + uid + "").attr("data-unreadCount", model.UnReadMsgCount);
        $("#ul_item_" + uid + "").attr("data-unreadMsgId", model.Message.MessageId);
        $("#ul_item_" + uid + "").find(".chat_item_info p.msg").prepend(unreadhtml);
    }



    $("#ul_item_" + uid + "").find(".chat_item_info p.msg").find(".content").attr("data-creatime", model.Message.CreateTime);
    $("#ul_item_" + uid + "").find(".chat_item_info p.msg").find(".content").text(model.Message.content);

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
    var bubble_default = "bubble_default";
    var me = "";
    var nickName = "";
    var MyId = $("#hdUserName").attr("data-uid");
    //接收者id不等于我的Id，说明我是发送者
    if (message.SenderId.trim() === MyId.trim()) {    
        bubble_default = "bubble_primary ";
        me = "me";
    }
    if (message.type == "group") {
         nickName = "<h4 class='nickname' >" + message.SenderName + "</h4>";
    }
   
    //绑定聊天对话框消息，前端
    var str =
   " <div class='message_item message " + me + "' style=' margin-bottom: 16px; float: left; width: 100%;'>" +
                       " <p class='message_system ng-scope'>" +
                        " <span class='content ng-binding'>" + getTime(message.CreateTime, "") + "</span>" +
                      "</p>" +
                       " <img class='avatar' src='" + message.SenderAvatar + "' title='" + message.SenderName + "'>" +
                        "<div class='content'>" +
                        nickName+
                         "<div class='bubble js_message_bubble ng-scope " + bubble_default + " left'>" +
                          "<div class='bubble_cont ng-scope'>" +
                           "<div class='plain'>" +
                            "            <pre class='js_message_plain ng-binding'>" + message.content + "</pre>" +
                             " </div>" +
                              "  </div</div></div></div>";
    $(".message_ul").append(str);
     
  
}


function MessageHandler(message, ChattingId)
{
  
    //将消息push进Storage，本地存储
    var key = "MessageListWith_" + ChattingId;
    PushSeesionStorage(key, message);



    if ($("#ul_item_" + ChattingId + "").length <= 0)
    {
        var user = GetUserByUserId(ChattingId);
        AddUserForChat(ChattingId, user.UserName, user.AvatarPic, user.IsOnline);

    }
    //绑定消息在好友列表处
    $("#ul_item_" + ChattingId + "").find(".ext:eq(0)").text(getTime(message.CreateTime));
    $("#ul_item_" + ChattingId + "").find(".chat_item_info p.msg").find(".content").text(message.content);

    //绑定消息在聊天对话框框
    if (ChattingId.trim() === $("#currentUserName").attr("data-uid").trim())
    {
        AddMessage(message);
        chatHub.server.messageConfirm($("#currentUserName").attr("data-uid").trim());
        return;
    }
  

    
     
  
    //之前已存在未读
    if ($("#ul_item_" + ChattingId + "").find(".avatar .icon").hasClass("web_wechat_reddot")) {
        var $unreadText = $("#ul_item_" + ChattingId + "").find(".chat_item_info p.msg").find(".unreadcount");
        var unreadcount = $unreadText.text();
        unreadcount = eval(getNum($unreadText.text())) + 1;
        $unreadText.text("[" + unreadcount + "条]");

  
        return;
    }
        //绑定消息在好友列表处，之前不存在未读消息
    else {
        $("#ul_item_" + ChattingId + "").find(".avatar .icon").addClass("web_wechat_reddot");
        var unreadhtml = "<span class='ng-binding ng-scope unreadcount'>[1条]</span>" ;
        $("#ul_item_" + ChattingId + "").find(".chat_item_info p.msg").prepend(unreadhtml);
        return;
    }
}


function CreateMesModel()
{
    var ChattingId = $("#currentUserName").attr("data-uid");
    var msg = $("#txtPrivateMessage").text();
    var SenderName = $("#hdUserName").text();
    var SenderId = $("#hdUserName").attr("data-uid").trim();
    var message = new Object();
    message.SenderId = SenderId;
    message.SenderName = SenderName;
    message.ChattingId = ChattingId;
    message.SenderAvatar = $("#myheadsrc").attr("src");
    message.content = msg;
    var time = new Date();
    message.CreateTime = time;
    return message;
}


function SendMsgHandlerForView(message)
{

    $("#txtPrivateMessage").text("");
    //将消息push进Storage，本地存储
    var key = "MessageListWith_" + message.ChattingId;
    PushSeesionStorage(key, message);
    //将消息绑定要页面显示，前端
    AddMessage(message);
    //好友列表出也要绑定最新消息以及时间
    $("#ul_item_" + message.ChattingId + "").find(".ext:eq(0)").text(getTime(message.CreateTime));
    $("#ul_item_" + message.ChattingId + "").find(".chat_item_info p.msg").find(".content").attr("data-creatime", message.CreateTime);
    $("#ul_item_" + message.ChattingId + "").find(".chat_item_info p.msg").find(".content").text(message.content);
    
}

function BindingUserInfo(e)
{

    if (IsCurrentUserOrNotUser(e)) { return; }
    //
  
    //获取当前用户用户名
    var username = $(e).find(".nickname_text").text();
    //更改聊天窗口的用户名
    $("#currentUserName").text(username);
    //获取当前聊天用户的Id
    var id = $(e).attr("data-uid");




    //更改聊天窗口的用户Id
    $("#currentUserName").attr("data-uid", id);
    //清空聊天对话框的消息
    $(".message_ul .message_item ").remove();
    if ($(e).hasClass("NeedGetMsgFromBack")) {
        //清除需要从后台获取数据的标识
        $(e).removeClass("NeedGetMsgFromBack");
        //
        var MsgId = $(e).attr("data-unreadMsgId");
        //如果存在未读消息，则从去后台获取未读消息数据
        var unreadcount = $(e).attr("data-unreadcount");
       
        if (unreadcount > 0)
        {
            chatHub.server.getUnreadMsg(id, MsgId, unreadcount);
        }
    }
    else {
        //将浏览器的本地缓存消息加载到聊天窗口中
        GetStrogeMessage(id);
    }




    //让未读标志的红点消失
    $(e).find("i.icon").removeClass("web_wechat_reddot");
    //让未读条数变为0，前端
    $("#ul_item_" + id + "").find(".chat_item_info p.msg span.unreadcount").remove();
    //让发送消息按钮标记为单人消息
    $(".btn_send").removeClass("group").addClass("single");


    $(".chat_ul_item").removeClass("selected");
    $("#ul_item_" + id + "").addClass("selected");

}

function BindingUserInfoInBox(user)
{
    $(".right .profile .nickname_area .nickname").text(user.UserName);
    $(".right .profile .avatar img").attr("src",user.AvatarPic);
}

function BindingUserInSearchBox(users)
{
    $(".search_bar .mmpop").find(".search_item").remove();
    if (users.length<= 0) {

        $("#mmpop5").slideUp();
        return;
    }
    $("#mmpop5").slideDown();
    var html = "<div class='ng-scope'> <h4 class='search_item contact_title ng-binding ng-scope first'>好友</h4> </div>";
    for (var i = 0; i < users.length; i++)
    {
     var item= " <div class='ng-scope search_item'>"+
           " <div  data-class-name='on' class='ng-isolate-scope'>"+
                                "<div class='contact_item'  >"+
                                 "   <div class='avatar'>"+
                                       " <img class='img lazy'alt='' src="+users[i].AvatarPic+">"+
                                    "</div>"+
                                    "<div class='info'>"+
                                       " <h4 class='nickname ng-binding' data-uid=" + users[i].UserDetailId + ">" + users[i].UserName + "</h4>" +
                                 "   </div></div> </div> </div>";
                                    html = html + item;
    }
    $(".search_bar .mmpop").find(".bottom-placeholder").before(html);
}