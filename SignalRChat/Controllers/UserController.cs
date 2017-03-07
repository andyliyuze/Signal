
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

namespace SignalRChat.Controllers
{
    public class UserController : Controller
    {

        private static IUserDetail_DAL _DALService = new UserDetail_DAL();
        private static IUserInfo_DAL _InfoDALService = new UserInfo_DAL();
        private static ICacheService _cache = new CacheService();
        // GET: User
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]

        public void RegisterConfire(RegisterModel model)
        {
            string pic = Request.Form["img-data"];
            string name = DateTime.Now.ToString("yyyMMddHHmmss") + ".Jpeg";
            //存入本地的绝对路径
            var localPath = HttpContext.Server.MapPath(UploadConfig.AvatarPath) + name;
            //存入数据库的相对路径
            model.AvatarPic = UploadConfig.AvatarPath + name;
            UploadHelper.SaveImage(pic, localPath);
            if (ModelState.IsValid)
            {
                Guid Id = Guid.NewGuid();
                var user = new UserDetail { UserDetailId = Id, AvatarPic = model.AvatarPic, UserName = model.UserName, IsOnline = false };
                var userInfo = new UserInfo { UserId = Id, UserName = model.UserName, AddTime = DateTime.Now, Pwd = model.Pwd };
                _DALService.Add(user);
                _InfoDALService.Add(userInfo);
                var result = _cache.Register(Id.ToString(), model.UserName, model.Pwd, model.AvatarPic);

            }


            Response.Redirect("/Chat.html");
        }


        public ActionResult _SearchUserPage() { return View(); }
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
    }
}