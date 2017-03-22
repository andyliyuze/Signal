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


        protected virtual MyFormsPrincipal<UserDetail> User
        {
            get { return HttpContext.Current.User as MyFormsPrincipal<UserDetail>; }
        }

        protected  string UserId { get; set; }

        public void SetUserCId(string cid)
        {


            User.UserData.UserCId = cid;
        }


        public MyBaseHub()
        {
        
      
        
        }
    }




 
}