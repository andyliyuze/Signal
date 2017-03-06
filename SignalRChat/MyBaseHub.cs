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
namespace SignalRChat
{
       
    public class MyBaseHub : Hub 
    {


        protected  UserDetail CurrentUser { get; set; }

        protected  string UserId { get; set; }


        public MyBaseHub()
        {

      
        
        }
    }




 
}