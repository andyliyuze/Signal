using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Autofac;
using MeassageCache;
using Model;
using DAL;
using System.Web.SessionState;

using System.Threading.Tasks;
using System.Diagnostics;
using DAL.Interface;
using Microsoft.AspNet.SignalR.Hubs;
using SignalRChat.Extend;

namespace SignalRChat
{
       
    public class MyBaseHub : Hub
    {
         private readonly ICacheService _service ;
        private readonly IGroup_DAL _IGroupDal;
        private readonly IMessageService _Msgservice;
        private readonly ILifetimeScope _hubLifetimeScope;
        public MyBaseHub(ILifetimeScope lifetimeScope)
        {
            _hubLifetimeScope = lifetimeScope.BeginLifetimeScope();
            _service = _hubLifetimeScope.Resolve<ICacheService>();
            _IGroupDal = _hubLifetimeScope.Resolve<IGroup_DAL>();
            _Msgservice = _hubLifetimeScope.Resolve<IMessageService>();
        }
        public MyBaseHub()
        {
            string a = "1";
        }
        protected virtual MyFormsPrincipal<UserDetail> User
        {
            get {
                MyFormsPrincipal<UserDetail> Principal;
                try
                {                
                    if (HttpContext.Current != null)
                    {

                        Principal = MyFormsPrincipal<UserDetail>.TryParsePrincipal(HttpContext.Current.Request);
                    }
                    else
                    {
                        Principal = MyFormsPrincipal<UserDetail>.TryParsePrincipalV2(Context.RequestCookies);
                    }
                    return Principal;
                }
                catch {
                    return null;
                }      
            }
        }

        protected  string UserId { get; set; }

        public void SetUserCId(string cid)
        {


            User.UserData.UserCId = cid;
        }


    }




 
}