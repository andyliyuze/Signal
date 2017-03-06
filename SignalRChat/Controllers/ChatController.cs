using SignalRChat.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalRChat.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        [MyAuthorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}