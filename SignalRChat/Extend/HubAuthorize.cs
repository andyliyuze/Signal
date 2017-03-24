using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
 

namespace SignalRChat.Extend
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class HubAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool UserAuthorized(System.Security.Principal.IPrincipal user)
        {
          
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return false;
            }
            else
            {                       
                return true;
            }
        }


      
    }




    
    
}