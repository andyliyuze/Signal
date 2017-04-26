function registerClientMethodsForChat(chatHub) {
 
    //接收后台传来的历史消息集合
    chatHub.client.messageListReceived = function (MessageList) {

        if (MessageList.length == 0)
        {
            $(".panel-body .span_noMessage").show();
            $(".panel-body .ico_loading_HistoryMessage").hide();
            $(".panel-body  .a_loading_HistoryMessage").hide();
            return;
        }

        for (var i = 0; i < MessageList.length; i++) {
            var  ChattingId=GetChattingId(MessageList[i]);
            //将消息push进Storage，本地存储
            var key = "MessageListWith_" + ChattingId;
            MessageList[i].Status == SendMessageStatus.Success;
            if (i == MessageList.length - 1) { PushSeesionStorage(key, MessageList[i], "CreateTime"); }
            else {
                PushSeesionStorage(key, MessageList[i]);
            }
            PrependMessage(MessageList[i]);
        }
        $(".panel-body .ico_loading_HistoryMessage").hide();
        $(".panel-body  .a_loading_HistoryMessage").show();
    };

    //接收后台传来的未读消息集合
    chatHub.client.unreadMessageListReceived = function (MessageList) {
        for (var i = 0; i < MessageList.length; i++) {
            //将消息push进Storage，本地存储
            var ChattingId = GetChattingId(MessageList[i]);
            var key = "MessageListWith_" + ChattingId;
            MessageList[i].Status == SendMessageStatus.Success;
            //if (i == MessageList.length - 1) { PushSeesionStorage(key, MessageList[i], "CreateTime"); }
            //else {
                PushSeesionStorage(key, MessageList[i]);
         //   }
            AddMessage(MessageList[i]);
        }
        AudioPlayForNewMessage();
    };

    //接收后台传来的单条消息
    chatHub.client.receivePrivateMessage = function (message) {
        var ChattingId = message.SenderId;
        MessageHandler(message, ChattingId);
      
    }

    //接收后台传来的单条消息
    chatHub.client.receiveGroupMessage = function (message) {
        var ChattingId = message.GroupId;
       MessageHandler(message, ChattingId);
    }

    //登陆结果
    chatHub.client.loginResult = function (result) {

        if (result === LoginStatus.Success) {
            return;

        }
        if (result === LoginStatus.Failed) { alert("密码错误"); }
        if (result === LoginStatus.UserUnExist) { alert("用户名不存在"); }

    };
  
    chatHub.client.change = function (oldcid, newcid) {

        $("#" + oldcid + "").attr("id", newcid);

    };
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
    };
    chatHub.client.sendMessageResult = function (result, msgId,chattingId)
    {
        if (result == SendMessageStatus.Success)
        {
            UpdateMessageStatusForSessionStroage(chattingId, msgId, SendMessageStatus.Success);
            $(".right.chat .message_ul .message_item[data-messageIdForJs='" + msgId + "']").find("i").hide();
        }
        else
        {
            UpdateMessageStatusForSessionStroage(chattingId, msgId, SendMessageStatus.Failed);
            $(".right.chat .message_ul .message_item[data-messageIdForJs='" + msgId + "']").find(".ico_fail").show();
        }
         
    }
}