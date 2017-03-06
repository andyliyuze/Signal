﻿using SignalRChat.Models;
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
namespace SignalRChat.Controllers
{
    public class UserController : Controller
    {

        private static IUserDetail_DAL _DALService = new UserDetail_DAL();
        private static ICacheService _cache = new CacheService();
        // GET: User
          [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
       
        public ActionResult RegisterConfire( RegisterModel model)
        {
          string pic=  Request.Form["preview"].ToString();
            if (ModelState.IsValid)
            {
                Guid Id = Guid.NewGuid();
                var user = new UserDetail { UserName = model.UserName, IsOnline = false,UserDetailId=Guid.NewGuid() };

                var result = _cache.Register(Id.ToString(),model.UserName,model.Pwd,model.AvatarPic);
                
            }

            
            return View(model);
        }
    }
}