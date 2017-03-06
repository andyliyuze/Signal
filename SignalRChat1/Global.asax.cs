using Autofac;
using Autofac.Integration.SignalR;
using MeassageCache;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using DAL;
using System.Web.Mvc;
using System.Web.Optimization;
using DAL.Interface;
 
namespace SignalRChat
{
    public class Global : System.Web.HttpApplication
    {
         

        protected void Application_Start(object sender, EventArgs e)
        {

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application["OnlineUserCount"] = 0;
            // Register the default hubs route: ~/signalr/hubs
         //    RouteTable.Routes.
            var builder = new ContainerBuilder();
            // You can register hubs all at once using assembly scanning...
            //   builder.RegisterHubs(Assembly.GetExecutingAssembly());

            // ...or you can register individual hubs manually.
            //signalR的特殊注册方法
            builder.RegisterType<ChatHub>().ExternallyOwned();
            builder.RegisterType<CacheService>().As<ICacheService>();
            builder.RegisterType<MessageService>().As<IMessageService>();
            builder.RegisterType<UserDetail_DAL>().As<IUserDetail_DAL>();
            builder.RegisterType<BroadcastMessages_DAL>().As<IBroadcastMessages_DAL>();
            builder.RegisterType<PrivateMessages_DAL>().As<IPrivateMessages_DAL>();
            //builder.RegisterType<PopCache>();
            var container = builder.Build();
            GlobalHost.DependencyResolver = new AutofacDependencyResolver(container);

           
        }

        protected void Session_Start(object sender, EventArgs e)
        {
           
            Application.Lock();
            Application["OnlineUserCount"] = (int)Application["OnlineUserCount"] + 1;
            Application.UnLock();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
         
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }
        protected void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            Application["OnlineUserCount"] = (int)Application["OnlineUserCount"] - 1;
            Application.UnLock();
        }

 

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }


 
}