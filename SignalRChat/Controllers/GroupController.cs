using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
namespace SignalRChat.Controllers
{
    public class GroupController : Controller
    {
        // GET: Gruop
        public ActionResult _CreateGroupPage()
        {
            return View();
        }

        //[HttpPost]
        //[Authorize]
        //public bool CreateGruop(Gruop model) 
        //{

        //    return true;
        //}
    }
}