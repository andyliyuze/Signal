﻿using Model;
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


            MyFormsPrincipal<UserDetail> Principal = MyFormsPrincipal<UserDetail>.TryParsePrincipal(HttpContext.Current.Request);
            return true;

        }
    }
}