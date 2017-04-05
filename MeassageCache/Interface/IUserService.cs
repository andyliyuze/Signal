using System.Collections.Generic;
using Model;
namespace MeassageCache.Interface
{
    public interface IUserService
    {
        bool AddNewUser(string id, string cid, string username);
    
        bool UpdateUserCId(string Uid, string Cid);   

        string LogOut(string uid);

        bool Register(string id, string UserName, string Pwd, string avatarPic);
         
        //登录检验
        bool Login(string Id, string pwd);       
    
        UserDetail GetUserDetail(string Id);
               
        string GetUserIdByName(string name);

        //获取所有好友信息
        List<UserDetail> GetMyFriends(string Uid);

        bool UpdateUserField(string key, string value,string uid);

       
        
    }
}
