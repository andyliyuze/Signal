using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SignalRChat.Extend
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {

            if(httpContext.User==null || !httpContext.User.Identity.IsAuthenticated)
            { return false; }
          else  return true;

        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
           
          filterContext.Result= new RedirectResult("/User/Login");
        }
    }
}