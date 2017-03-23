using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
namespace MeassageCache
{
    public interface ICacheService
    {
        bool AddNewUser(string id, string cid, string username);
        
        bool AddBroadcastMessage(List<BroadcastMessage> list);

        bool AddPrivateMessage(List<PrivateMessage> list);
        string GetUserIdByCId(string cid);
        string GetUserCId(string uid);
        bool UpdateUserCId(string Uid, string Cid);
       

        string LogOut(string uid);

        bool Register(string id, string UserName, string Pwd, string avatarPic);
         
        //登录检验
       bool Login(string Id, string pwd);
         
        //获取所有好友id
       UserDetail GetUserDetail(string Id);
         
        //获取所有好友信息
       string GetUserIdByName(string name);


       List<string> GetFriendsIds(string id);
         

         List<UserDetail> GetMyFriendsDetail(List<string> ids);

         bool UpdateUserField(string key, string value,string uid);
         string AfterLogin(string Id, string cid);
        
    }
}
