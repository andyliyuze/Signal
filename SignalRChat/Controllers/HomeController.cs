using SignalRChat.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SignalRChat.Controllers
{
    public class HomeController : AsyncController
    {
        [MyAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        public  ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
              Test1();
           //Task<string> s= Test1();
            //     test2();

         //   ViewBag.s = s.Result.ToString(); ;
         
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            Thread.Sleep(1000);
            return View();
        }

        public  void test2()
        {


              Thread.Sleep(5000);

        }

        public  Task<string> Test1()
        {
          //  Thread.Sleep(1000);
          

            return Task.Run(() =>
            {
                Thread.Sleep(3000);
                string aa = "321231";
                return aa;
            });
        }

    }





}