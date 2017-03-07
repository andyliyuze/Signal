using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using System.Drawing;
using System.IO;
using Common;
using DAL.Interface;
using DAL;

namespace SignalRChat.Controllers
{
    public class GroupController : Controller
    {
        private readonly IGroup_DAL _Groupservice = new Group_DAL();
        //public GroupController(IGroup_DAL Groupservice) {

        //    _Groupservice = Groupservice;
        //}

     
        // GET: Gruop
        public ActionResult _CreateGroupPage()
        {
            return View();
        }

        [HttpPost]
 
        public JsonResult CreateGruop()
        {
            string GroupName = Request.Form["GroupName"];
            string data = Request.Form["Avatar"];
            //到时候会替换为本地获取
            //LoginUserInfo.UserID.ToString();
            string uid = Request.Form["UserId"];
            string name = DateTime.Now.ToString("yyyMMddHHmmss") + ".Jpeg";
            //存入本地的绝对路径
            var localPath = HttpContext.Server.MapPath(UploadConfig.AvatarPath) + name;
            //存入数据库的相对路径
            string path = UploadConfig.AvatarPath + name;
            UploadHelper.SaveImage(data, localPath);
            Guid GroupId = Guid.NewGuid();
            var flag = _Groupservice.Create(new Group { OwnerId = Guid.Parse(uid), GroupAvatar = path, GroupId = GroupId, GroupName = GroupName });

            JsonResult json = new JsonResult();
            if (flag)
            {
                json.Data = new { result = flag, groupName = GroupName, groupAvatar = path, groupId = GroupId };
            }
            else
            {
                json.Data = new { result = flag };
            }
            return json;
        }

        
      
    }
}