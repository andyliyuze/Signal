using Autofac;
using Autofac.Integration.SignalR;
using DAL;
using DAL.Interface;
using MeassageCache;
using Microsoft.AspNet.SignalR;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SignalRChat
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
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
            builder.RegisterType<UserHub>().ExternallyOwned();
            builder.RegisterType<CacheService>().As<ICacheService>();

            
            builder.RegisterType<MessageService>().As<IMessageService>();
            builder.RegisterType<UserCacheService>().As<IUserCacheService>();

            builder.RegisterType<FriendsApply_DAL>().As<IFriendsApply_DAL>();
           
            builder.RegisterType<Friends_DAL>().As<IFriends_DAL>();
            builder.RegisterType<UserDetail_DAL>().As<IUserDetail_DAL>();
            builder.RegisterType<BroadcastMessages_DAL>().As<IBroadcastMessages_DAL>();
            builder.RegisterType<PrivateMessages_DAL>().As<IPrivateMessages_DAL>();
            builder.RegisterType<GroupMember_DAL>().As<IGroupMember_DAL>();
            
            builder.RegisterType<Group_DAL>().As<IGroup_DAL>();

            builder.RegisterType<JoinGroupApply_DAL>().As<IJoinGroupApply_DAL>();
            
            //builder.RegisterType<PopCache>();
            var container = builder.Build();
            GlobalHost.DependencyResolver = new AutofacDependencyResolver(container);
        }
    }
}
