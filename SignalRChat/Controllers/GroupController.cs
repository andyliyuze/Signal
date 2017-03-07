using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using System.Drawing;
using System.IO;
using Common;

namespace SignalRChat.Controllers
{
    public class GroupController : Controller
    {
        // GET: Gruop
        public ActionResult _CreateGroupPage()
        {
            return View();
        }

        [HttpPost]
 
        public bool CreateGruop()
        {
           string GroupName= Request.Form["GroupName"];
           string data= Request.Form["Avatar"];

           
            string name = DateTime.Now.ToString("yyyMMddHHmmss") + ".Jpeg";
            var localPath = HttpContext.Server.MapPath(UploadConfig.AvatarPath) + name;

            string path = UploadConfig.AvatarPath + name;
            UploadHelper.SaveImage(data, path); 
            return true;
        }

      
    }
}