function getNum(text) {
    var value = text.replace(/[^0-9]/ig, "");
    return value;
}

function getTime(text, type) {
    var date = new Date(text);
    var year = date.getYear();
    var month =eval( date.getMonth()+1);
    var day = date.getDate();



    var hours = date.getHours();
    var minutes = date.getMinutes();
    var getSeconds = date.getSeconds();

    if (type == "ext") {
        return hours + ":" + minutes;

    }
    else {
        return month + "-" + day + " " + hours + ":" + minutes;

    }

}

function getTimeFromBackGroung(text, type) {
    var arr = text.split('T');
    if (arr.length <= 1) { return; }

    var date = arr[0];
    var dateArr = date.split('-');

    var year = dateArr[0];
    var month = dateArr[1];
    var day = dateArr[2];

    var time = arr[1];
    var timeArr = time.split(':');

    var hours = timeArr[0];
    var minutes = timeArr[1];
    var getSeconds = timeArr[2];


    if (type == "ext") {
        return hours + ":" + minutes;

    }
    else {
        return month + "-" + day + " " + hours + ":" + minutes;

    }

}


//判断两个object 的属性值是否完全相同
function IsObjectValueEqual(a, b) {
    // Of course, we can do it use for in 
    // Create arrays of property names
    var aProps = Object.getOwnPropertyNames(a);
    var bProps = Object.getOwnPropertyNames(b);

    // If number of properties is different,
    // objects are not equivalent
    if (aProps.length != bProps.length) {
        return false;
    }

    for (var i = 0; i < aProps.length; i++) {
        var propName = aProps[i];

        // If values of same property are not equal,
        // objects are not equivalent
        if (a[propName] !== b[propName]) {
            return false;
        }
    }

    // If we made it this far, objects
    // are considered equivalent
    return true;
}


Array.prototype.Contain = function (item) {

    for (var i = 0; i < Array.length; i++) {

        if (IsObjectValueEqual(Array[i], item)) { return true; }
        else { return false; }

    }

}


//重写PushSeesionStorage，将item添加到指定key的列表里
function PushSeesionStorage(key, item) {

    //获得消息集合的字符串
    var listStr = sessionStorage.getItem(key);

    //定义一个消息数组
    var list = [];
    if (listStr != null) {
        // 将字符串反序列化为消息数组
        list = JSON.parse(listStr);
    }
    //flag标识 表示是否存在
    list.push(item);

    //将数组序列化为字符串
    listStr = JSON.stringify(list);

    sessionStorage.setItem(key, listStr);
}
//根据主键Id检查是否已经存在相同的消息

function GetSeesionStorageList(key) {

    //获得消息集合的字符串
    var listStr = sessionStorage.getItem(key);
    //定义一个消息数组
    var list = [];
    if (listStr != null) {
        // 将字符串反序列化为消息数组
        list = JSON.parse(listStr);
    }
    return list;

}

//将List 添加到指定的key里
function PushArrInSessionStroage(key, Arr) {

    //将数组序列化为字符串
    var ArrStr = JSON.stringify(Arr);

    sessionStorage.setItem(key, ArrStr);
}

function RemoveByKey(key)
{
    sessionStorage.removeItem(key);

}

// 播放音频文件，文件要带后缀名
function AudioPlay(AudioName) {

    
    //游戏音效变量初始化
    var audio = document.createElement("audio");
    audio.src = AudioName;
    audio.play();
}

function AudioPlayForNewMessage()
{
    var open = ChatRoomConfig.AudioState;
    if (open == "On")
    {
        AudioPlay("/Content/audio/Bongo.mp3");
        
    }

}

//唯一标识
function generateUUID() {
    var d = new Date().getTime();
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
    return uuid;
};
