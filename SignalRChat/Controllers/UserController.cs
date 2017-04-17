
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Model;
using DAL;
using MeassageCache;
using DAL.Interface;

using SignalRChat.Models;
using Common;
using SignalRChat.Extend;
using MeassageCache.Interface;
using Microsoft.AspNet.SignalR;

namespace SignalRChat.Controllers
{
    public class UserController : Controller
    {

        private static IUserDetail_DAL _DALService = new UserDetail_DAL();
        private static IUserInfo_DAL _InfoDALService = new UserInfo_DAL();
        private static IUserService _cache = new UserService();
        // GET: User
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]

        public ActionResult RegisterConfire(RegisterModel model)
        {
            string pic = Request.Form["img-data"];
            string name = DateTime.Now.ToString("yyyMMddHHmmss") + ".Jpeg";
            //存入本地的绝对路径
            var localPath = HttpContext.Server.MapPath(UploadConfig.AvatarPath) + name;
            //存入数据库的相对路径
            model.AvatarPic = UploadConfig.AvatarPath + name;
            UploadHelper.SaveImageFromBase64(pic, localPath);
            if (ModelState.IsValid)
            {
                Guid Id = Guid.NewGuid();
                var user = new UserDetail { UserDetailId = Id, AvatarPic = model.AvatarPic, UserName = model.UserName, IsOnline = false };
                var userInfo = new UserInfo { UserId = Id, UserName = model.UserName, AddTime = DateTime.Now, Pwd = model.Pwd };
                _DALService.Add(user);
                _InfoDALService.Add(userInfo);
                var result = _cache.Register(Id.ToString(), model.UserName, model.Pwd, model.AvatarPic);

            }

            return RedirectToAction("Login", "User");
           
        }

        public ActionResult _SearchUserPage()
        {
            return View();
        }

        public ActionResult _FriensApplysPage()
        {
            return View();
        }

        public ActionResult _ReplyResultPage()
        {
            return View();
        }

        public ActionResult _CreateGruopPage() {

            return View();
        
        }

       
        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(string pwd,string name)
        {
            string uid = CheckUserInRedis(name);
            if (!string.IsNullOrEmpty(uid)&& TryLogin(uid, pwd))
            {
                GetUserDetail(uid);
                return RedirectToAction("Index", "Chat");
            }
           else{
                return Content("<script>alert('密码或用户名错误！');location.href='/User/Login';</script>");

            }
        }
        private enum LoginStatus
        {
            Success = 0,
            UserUnExist = 1,
            Failed = 2
        }

        //检查用户名是否存在，并且保存UserId字段的值
        private string CheckUserInRedis(string userName)
        {
            string uid= _cache.GetUserIdByName(userName);
            return uid;
        }


        private bool TryLogin(string userId, string Pwd)
        {
            return _cache.Login(userId, Pwd);
            

        }

        //获取CurrentUser字段值
        private void GetUserDetail(string UserId)
        {
            try
            {
                UserDetail    CurrentUser = _cache.GetUserDetail(UserId);
                MyFormsPrincipal<UserDetail>.SignIn(CurrentUser.UserName, CurrentUser, 60);


            }
            catch(Exception e)
            {
                throw (e);
            }

        }
    }
}