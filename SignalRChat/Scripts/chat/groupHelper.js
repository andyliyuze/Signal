function AddGroup(id, name, avatarPic) {
    var str =
           " <div data-uid=" + id + " class='chat_ul_item 'id='ul_item_" + id + "'>" +
                "<div class='ext'>" +
                    "<p class='attr ng-binding'></p>" +
                   " <p class='attr ng-scope'>" +
                       " <i class='web_wechat_no-remind'></i>" +
                    "</p>" +
               " </div>" +
                "<div class='avatar'>" +
                    "<img class='img' id='img_" + id + "' src='" + avatarPic + "' />" +
                    "<i class='icon ng-scope'></i>" +
               " </div>" +
                "<div class='chat_item_info'>" +
                    "<h3 class='nickname'>" +
                        "<span class='nickname_text ng-binding'>" + name + "</span>" +
                       "<p class='msg ng-scope'></p>" +
                    "</h3>" +
                   "</div></div>";
    $(".groups_ul .chat_ul_div").append(str);
}